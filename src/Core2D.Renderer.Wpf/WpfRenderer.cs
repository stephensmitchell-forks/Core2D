﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using Spatial.Arc;
using W = System.Windows;
using WM = System.Windows.Media;
using WMI = System.Windows.Media.Imaging;

namespace Core2D.Renderer.Wpf
{
    /// <summary>
    /// Native Windows Presentation Foundation shape renderer.
    /// </summary>
    public class WpfRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private ICache<IShapeStyle, (WM.Brush, WM.Pen)> _styleCache;
        private ICache<IArrowStyle, (WM.Brush, WM.Pen)> _arrowStyleCache;
        private ICache<ILineShape, WM.PathGeometry> _curvedLineCache;
        private ICache<IArcShape, WM.PathGeometry> _arcCache;
        private ICache<ICubicBezierShape, WM.PathGeometry> _cubicBezierCache;
        private ICache<IQuadraticBezierShape, WM.PathGeometry> _quadraticBezierCache;
        private ICache<ITextShape, (string, WM.FormattedText, IShapeStyle)> _textCache;
        private ICache<string, WMI.BitmapImage> _biCache;
        private ICache<IPathShape, (Path.IPathGeometry, WM.StreamGeometry, IShapeStyle)> _pathCache;
        private readonly PathGeometryConverter _converter;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public WpfRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _styleCache = _serviceProvider.GetService<IFactory>().CreateCache<IShapeStyle, (WM.Brush, WM.Pen)>();
            _arrowStyleCache = _serviceProvider.GetService<IFactory>().CreateCache<IArrowStyle, (WM.Brush, WM.Pen)>();
            _curvedLineCache = _serviceProvider.GetService<IFactory>().CreateCache<ILineShape, WM.PathGeometry>();
            _arcCache = _serviceProvider.GetService<IFactory>().CreateCache<IArcShape, WM.PathGeometry>();
            _cubicBezierCache = _serviceProvider.GetService<IFactory>().CreateCache<ICubicBezierShape, WM.PathGeometry>();
            _quadraticBezierCache = _serviceProvider.GetService<IFactory>().CreateCache<IQuadraticBezierShape, WM.PathGeometry>();
            _textCache = _serviceProvider.GetService<IFactory>().CreateCache<ITextShape, (string, WM.FormattedText, IShapeStyle)>();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, WMI.BitmapImage>(bi => bi.StreamSource.Dispose());
            _pathCache = _serviceProvider.GetService<IFactory>().CreateCache<IPathShape, (Path.IPathGeometry, WM.StreamGeometry, IShapeStyle)>();
            _converter = new PathGeometryConverter(_serviceProvider);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        private static W.Point GetTextOrigin(IShapeStyle style, ref W.Rect rect, WM.FormattedText ft)
        {
            double ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.TopLeft.X;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - ft.Width;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rect.Width / 2.0) - (ft.Width / 2.0);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.TopLeft.Y;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - ft.Height;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2.0) - (ft.Height / 2.0);
                    break;
            }

            return new W.Point(ox, oy);
        }

        private static WM.Color ToColor(IColor color)
        {
            switch (color)
            {
                case IArgbColor argbColor:
                    return WM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
                default:
                    throw new NotSupportedException($"The {color.GetType()} color type is not supported.");
            }
        }

        private static WM.Brush ToBrush(IColor color)
        {
            switch (color)
            {
                case IArgbColor argbColor:
                    var brush = new WM.SolidColorBrush(ToColor(argbColor));
                    brush.Freeze();
                    return brush;
                default:
                    throw new NotSupportedException($"The {color.GetType()} color type is not supported.");
            }
        }

        private static WM.Pen ToPen(IBaseStyle style, double thickness)
        {
            var brush = ToBrush(style.Stroke);
            var pen = new WM.Pen(brush, thickness);
            switch (style.LineCap)
            {
                case LineCap.Flat:
                    pen.StartLineCap = WM.PenLineCap.Flat;
                    pen.EndLineCap = WM.PenLineCap.Flat;
                    pen.DashCap = WM.PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    pen.StartLineCap = WM.PenLineCap.Square;
                    pen.EndLineCap = WM.PenLineCap.Square;
                    pen.DashCap = WM.PenLineCap.Square;
                    break;
                case LineCap.Round:
                    pen.StartLineCap = WM.PenLineCap.Round;
                    pen.EndLineCap = WM.PenLineCap.Round;
                    pen.DashCap = WM.PenLineCap.Round;
                    break;
            }
            pen.DashStyle = new WM.DashStyle(
                StyleHelper.ConvertDashesToDoubleArray(style.Dashes),
                style.DashOffset);
            pen.DashStyle.Offset = style.DashOffset;
            pen.Freeze();
            return pen;
        }

        private static W.Rect CreateRect(IPointShape tl, IPointShape br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new W.Rect(
                new W.Point(tlx + dx, tly + dy),
                new W.Point(brx + dx, bry + dy));
        }

        private static void DrawLineInternal(WM.DrawingContext dc, double half, WM.Pen pen, bool isStroked, ref W.Point p0, ref W.Point p1)
        {
            if (!isStroked)
                return;

            var gs = new WM.GuidelineSet(
                new double[] { p0.X + half, p1.X + half },
                new double[] { p0.Y + half, p1.Y + half });
            dc.PushGuidelineSet(gs);
            dc.DrawLine(isStroked ? pen : null, p0, p1);
            dc.Pop();
        }

        private void DrawLineCurveInternal(WM.DrawingContext dc, double half, WM.Pen pen, ILineShape line, ref W.Point pt1, ref W.Point pt2, double dx, double dy)
        {
            double p1x = pt1.X;
            double p1y = pt1.Y;
            double p2x = pt2.X;
            double p2y = pt2.Y;
            LineShapeExtensions.GetCurvedLineBezierControlPoints(
                line.Style.LineStyle.CurveOrientation,
                line.Style.LineStyle.Curvature,
                line.Start.Alignment,
                line.End.Alignment,
                ref p1x, ref p1y,
                ref p2x, ref p2y);

            var pg = _curvedLineCache.Get(line);
            if (pg != null)
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new W.Point(pt1.X + dx, pt1.Y + dy);
                pf.IsFilled = false;
                var bs = pf.Segments[0] as WM.BezierSegment;
                bs.Point1 = new W.Point(p1x + dx, p1y + dy);
                bs.Point2 = new W.Point(p2x + dx, p2y + dy);
                bs.Point3 = new W.Point(pt2.X + dx, pt2.Y + dy);
                bs.IsStroked = line.IsStroked;
            }
            else
            {
                var pf = new WM.PathFigure()
                {
                    StartPoint = new W.Point(pt1.X + dx, pt1.Y + dy),
                    IsFilled = false
                };
                var bs = new WM.BezierSegment(
                        new W.Point(p1x + dx, p1y + dy),
                        new W.Point(p2x + dx, p2y + dy),
                        new W.Point(pt2.X + dx, pt2.Y + dy),
                        line.IsStroked);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new WM.PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                _curvedLineCache.Set(line, pg);
            }

            DrawPathGeometryInternal(dc, half, null, pen, line.IsStroked, false, pg);
        }

        private void DrawLineArrowsInternal(WM.DrawingContext dc, ILineShape line, IShapeStyle style, double halfStart, double halfEnd, double thicknessStart, double thicknessEnd, double dx, double dy, out W.Point pt1, out W.Point pt2)
        {
            // Start arrow style.
            GetCached(style.StartArrowStyle, thicknessStart, out var fillStartArrow, out var strokeStartArrow);

            // End arrow style.
            GetCached(style.EndArrowStyle, thicknessEnd, out var fillEndArrow, out var strokeEndArrow);

            // Line max length.
            double x1 = line.Start.X + dx;
            double y1 = line.Start.Y + dy;
            double x2 = line.End.X + dx;
            double y2 = line.End.Y + dy;

            line.GetMaxLength(ref x1, ref y1, ref x2, ref y2);

            // Arrow transforms.
            var sas = style.StartArrowStyle;
            var eas = style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(dc, halfStart, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(dc, halfEnd, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private static W.Point DrawLineArrowInternal(WM.DrawingContext dc, double half, WM.Pen pen, WM.Brush brush, double x, double y, double angle, IArrowStyle style)
        {
            W.Point pt;
            bool doRectTransform = angle % 90.0 != 0.0;
            var rt = new WM.RotateTransform(angle, x, y);
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new W.Point(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = rt.Transform(new W.Point(x - sx, y));
                        var rect = new W.Rect(x - sx, y - ry, sx, sy);
                        if (doRectTransform)
                        {
                            dc.PushTransform(rt);
                            DrawRectangleInternal(dc, half, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                            dc.Pop();
                        }
                        else
                        {
                            var bounds = rt.TransformBounds(rect);
                            DrawRectangleInternal(dc, half, brush, pen, style.IsStroked, style.IsFilled, ref bounds);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = rt.Transform(new W.Point(x - sx, y));
                        dc.PushTransform(rt);
                        var c = new W.Point(x - rx, y);
                        DrawEllipseInternal(dc, half, brush, pen, style.IsStroked, style.IsFilled, ref c, rx, ry);
                        dc.Pop();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt = rt.Transform(new W.Point(x, y));
                        var p11 = rt.Transform(new W.Point(x - sx, y + sy));
                        var p21 = rt.Transform(new W.Point(x, y));
                        var p12 = rt.Transform(new W.Point(x - sx, y - sy));
                        var p22 = rt.Transform(new W.Point(x, y));
                        DrawLineInternal(dc, half, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, half, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(WM.DrawingContext dc, double half, WM.Brush brush, WM.Pen pen, bool isStroked, bool isFilled, ref W.Rect rect)
        {
            if (!isStroked && !isFilled)
                return;

            var gs = new WM.GuidelineSet(
                new double[]
                    {
                        rect.TopLeft.X + half,
                        rect.BottomRight.X + half
                    },
                new double[]
                    {
                        rect.TopLeft.Y + half,
                        rect.BottomRight.Y + half
                    });
            dc.PushGuidelineSet(gs);
            dc.DrawRectangle(isFilled ? brush : null, isStroked ? pen : null, rect);
            dc.Pop();
        }

        private static void DrawEllipseInternal(WM.DrawingContext dc, double half, WM.Brush brush, WM.Pen pen, bool isStroked, bool isFilled, ref W.Point center, double rx, double ry)
        {
            if (!isStroked && !isFilled)
                return;

            var gs = new WM.GuidelineSet(
                new double[]
                    {
                        center.X - rx + half,
                        center.X + rx + half
                    },
                new double[]
                    {
                        center.Y - ry + half,
                        center.Y + ry + half
                    });
            dc.PushGuidelineSet(gs);
            dc.DrawEllipse(isFilled ? brush : null, isStroked ? pen : null, center, rx, ry);
            dc.Pop();
        }

        private static void DrawPathGeometryInternal(WM.DrawingContext dc, double half, WM.Brush brush, WM.Pen pen, bool isStroked, bool isFilled, WM.PathGeometry pg)
        {
            if (!isStroked && !isFilled)
                return;

            var gs = new WM.GuidelineSet(
                new double[]
                    {
                        pg.Bounds.TopLeft.X + half,
                        pg.Bounds.BottomRight.X + half
                    },
                new double[]
                    {
                        pg.Bounds.TopLeft.Y + half,
                        pg.Bounds.BottomRight.Y + half
                    });
            dc.PushGuidelineSet(gs);
            dc.DrawGeometry(isFilled ? brush : null, isStroked ? pen : null, pg);
            dc.Pop();
        }

        private static void DrawGridInternal(WM.DrawingContext dc, double half, WM.Pen stroke, ref W.Rect rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new W.Point(x, oy);
                var p1 = new W.Point(x, ey);
                DrawLineInternal(dc, half, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new W.Point(ox, y);
                var p1 = new W.Point(ex, y);
                DrawLineInternal(dc, half, stroke, isStroked, ref p0, ref p1);
            }
        }

        private WM.PathGeometry ToPathGeometry(IArcShape arc, double dx, double dy)
        {
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            var pg = _arcCache.Get(arc);

            if (pg != null)
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new W.Point(a.Start.X + dx, a.Start.Y + dy);
                pf.IsFilled = arc.IsFilled;
                var segment = pf.Segments[0] as WM.ArcSegment;
                segment.Point = new W.Point(a.End.X + dx, a.End.Y + dy);
                segment.Size = new W.Size(a.Radius.Width, a.Radius.Height);
                segment.IsLargeArc = a.IsLargeArc;
                segment.IsStroked = arc.IsStroked;
            }
            else
            {
                var pf = new WM.PathFigure()
                {
                    StartPoint = new W.Point(a.Start.X, a.Start.Y),
                    IsFilled = arc.IsFilled
                };

                var segment = new WM.ArcSegment(
                    new W.Point(a.End.X, a.End.Y),
                    new W.Size(a.Radius.Width, a.Radius.Height),
                    0.0,
                    a.IsLargeArc, WM.SweepDirection.Clockwise,
                    arc.IsStroked);

                //segment.Freeze();
                pf.Segments.Add(segment);
                //pf.Freeze();
                pg = new WM.PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                _arcCache.Set(arc, pg);
            }

            return pg;
        }

        private WM.PathGeometry ToPathGeometry(ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var pg = _cubicBezierCache.Get(cubicBezier);

            if (pg != null)
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new W.Point(cubicBezier.Point1.X + dx, cubicBezier.Point1.Y + dy);
                pf.IsFilled = cubicBezier.IsFilled;
                var bs = pf.Segments[0] as WM.BezierSegment;
                bs.Point1 = new W.Point(cubicBezier.Point2.X + dx, cubicBezier.Point2.Y + dy);
                bs.Point2 = new W.Point(cubicBezier.Point3.X + dx, cubicBezier.Point3.Y + dy);
                bs.Point3 = new W.Point(cubicBezier.Point4.X + dx, cubicBezier.Point4.Y + dy);
                bs.IsStroked = cubicBezier.IsStroked;
            }
            else
            {
                var pf = new WM.PathFigure()
                {
                    StartPoint = new W.Point(cubicBezier.Point1.X + dx, cubicBezier.Point1.Y + dy),
                    IsFilled = cubicBezier.IsFilled
                };
                var bs = new WM.BezierSegment(
                        new W.Point(cubicBezier.Point2.X + dx, cubicBezier.Point2.Y + dy),
                        new W.Point(cubicBezier.Point3.X + dx, cubicBezier.Point3.Y + dy),
                        new W.Point(cubicBezier.Point4.X + dx, cubicBezier.Point4.Y + dy),
                        cubicBezier.IsStroked);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new WM.PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                _cubicBezierCache.Set(cubicBezier, pg);
            }

            return pg;
        }

        private WM.PathGeometry ToPathGeometry(IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var pg = _quadraticBezierCache.Get(quadraticBezier);

            if (pg != null)
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new W.Point(quadraticBezier.Point1.X + dx, quadraticBezier.Point1.Y + dy);
                pf.IsFilled = quadraticBezier.IsFilled;
                var qbs = pf.Segments[0] as WM.QuadraticBezierSegment;
                qbs.Point1 = new W.Point(quadraticBezier.Point2.X + dx, quadraticBezier.Point2.Y + dy);
                qbs.Point2 = new W.Point(quadraticBezier.Point3.X + dx, quadraticBezier.Point3.Y + dy);
                qbs.IsStroked = quadraticBezier.IsStroked;
            }
            else
            {
                var pf = new WM.PathFigure()
                {
                    StartPoint = new W.Point(quadraticBezier.Point1.X + dx, quadraticBezier.Point1.Y + dy),
                    IsFilled = quadraticBezier.IsFilled
                };

                var qbs = new WM.QuadraticBezierSegment(
                        new W.Point(quadraticBezier.Point2.X + dx, quadraticBezier.Point2.Y + dy),
                        new W.Point(quadraticBezier.Point3.X + dx, quadraticBezier.Point3.Y + dy),
                        quadraticBezier.IsStroked);
                //bs.Freeze();
                pf.Segments.Add(qbs);
                //pf.Freeze();
                pg = new WM.PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                _quadraticBezierCache.Set(quadraticBezier, pg);
            }

            return pg;
        }

        private WM.MatrixTransform ToMatrixTransform(IMatrixObject m)
        {
            return new WM.MatrixTransform(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        private void GetCached(IArrowStyle style, double thickness, out WM.Brush fill, out WM.Pen stroke)
        {
            (fill, stroke) = _arrowStyleCache.Get(style);
            if (fill == null || stroke == null)
            {
                fill = ToBrush(style.Fill);
                stroke = ToPen(style, thickness);
                _arrowStyleCache.Set(style, (fill, stroke));
            }
        }

        private void GetCached(IShapeStyle style, double thickness, out WM.Brush fill, out WM.Pen stroke)
        {
            (fill, stroke) = _styleCache.Get(style);
            if (fill == null || stroke == null)
            {
                fill = ToBrush(style.Fill);
                stroke = ToPen(style, thickness);
                _styleCache.Set(style, (fill, stroke));
            }
        }

        /// <inheritdoc/>
        public void InvalidateCache(IShapeStyle style)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IMatrixObject matrix)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IBaseShape shape, IShapeStyle style, double dx, double dy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ClearCache(bool isZooming)
        {
            _styleCache.Reset();
            _arrowStyleCache.Reset();

            if (!isZooming)
            {
                _curvedLineCache.Reset();
                _arcCache.Reset();
                _cubicBezierCache.Reset();
                _quadraticBezierCache.Reset();
                _textCache.Reset();
                _biCache.Reset();
                _pathCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var _dc = dc as WM.DrawingContext;
            var brush = ToBrush(color);
            var rect = new W.Rect(x, y, width, height);
            DrawRectangleInternal(_dc, 0.5, brush, null, false, true, ref rect);
        }

        /// <inheritdoc/>
        public object PushMatrix(object dc, IMatrixObject matrix)
        {
            var _dc = dc as WM.DrawingContext;
            _dc.PushTransform(ToMatrixTransform(matrix));
            return null;
        }

        /// <inheritdoc/>
        public void PopMatrix(object dc, object state)
        {
            var _dc = dc as WM.DrawingContext;
            _dc.Pop();
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPageContainer container, double dx, double dy)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILayerContainer layer, double dx, double dy)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.Draw(dc, this, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILineShape line, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = line.Style;
            if (style == null)
                return;

            double zoom = State.ZoomX;
            double thickness = style.Thickness / zoom;
            double half = thickness / 2.0;
            double thicknessStartArrow = style.StartArrowStyle.Thickness / zoom;
            double halfStartArrow = thicknessStartArrow / 2.0;
            double thicknessEndArrow = style.EndArrowStyle.Thickness / zoom;
            double halfEndArrow = thicknessEndArrow / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            DrawLineArrowsInternal(_dc, line, style, halfStartArrow, halfEndArrow, thicknessStartArrow, thicknessEndArrow, dx, dy, out var pt1, out var pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(_dc, half, stroke, line, ref pt1, ref pt2, dx, dy);
            }
            else
            {
                DrawLineInternal(_dc, half, stroke, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = rectangle.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);

            DrawRectangleInternal(_dc, half, fill, stroke, rectangle.IsStroked, rectangle.IsFilled, ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(_dc, half, stroke, ref rect, rectangle.OffsetX, rectangle.OffsetY, rectangle.CellWidth, rectangle.CellHeight, true);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = ellipse.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new W.Point(rect.X + rx, rect.Y + ry);

            DrawEllipseInternal(_dc, half, fill, stroke, ellipse.IsStroked, ellipse.IsFilled, ref center, rx, ry);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IArcShape arc, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = arc.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var pg = ToPathGeometry(arc, dx, dy);

            DrawPathGeometryInternal(_dc, half, fill, stroke, arc.IsStroked, arc.IsFilled, pg);
        }

        /// <inheritdoc/>
        public void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = cubicBezier.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var pg = ToPathGeometry(cubicBezier, dx, dy);

            DrawPathGeometryInternal(_dc, half, fill, stroke, cubicBezier.IsStroked, cubicBezier.IsFilled, pg);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = quadraticBezier.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var pg = ToPathGeometry(quadraticBezier, dx, dy);

            DrawPathGeometryInternal(_dc, half, fill, stroke, quadraticBezier.IsStroked, quadraticBezier.IsFilled, pg);
        }

        /// <inheritdoc/>
        public void Draw(object dc, ITextShape text, double dx, double dy)
        {
            var _dc = dc as WM.DrawingContext;

            var style = text.Style;
            if (style == null)
                return;

            if (!(text.GetProperty(nameof(ITextShape.Text)) is string tbind))
            {
                tbind = text.Text;
            }

            if (tbind == null)
            {
                return;
            }

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            var rect = CreateRect(text.TopLeft, text.BottomRight, dx, dy);

            (string ct, var ft, var cs) = _textCache.Get(text);
            if (string.Compare(ct, tbind) == 0 && cs == style)
            {
                _dc.DrawText(ft, GetTextOrigin(style, ref rect, ft));
            }
            else
            {
                var ci = CultureInfo.InvariantCulture;

                var fontStyle = W.FontStyles.Normal;
                var fontWeight = W.FontWeights.Regular;

                if (style.TextStyle.FontStyle != null)
                {
                    if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                    {
                        fontStyle = W.FontStyles.Italic;
                    }

                    if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                    {
                        fontWeight = W.FontWeights.Bold;
                    }
                }

                var tf = new WM.Typeface(new WM.FontFamily(style.TextStyle.FontName), fontStyle, fontWeight, W.FontStretches.Normal);

                ft = new WM.FormattedText(
                    tbind,
                    ci,
                    ci.TextInfo.IsRightToLeft ? W.FlowDirection.RightToLeft : W.FlowDirection.LeftToRight,
                    tf,
                    style.TextStyle.FontSize > 0.0 ? style.TextStyle.FontSize : double.Epsilon,
                    stroke.Brush, null, WM.TextFormattingMode.Ideal);

                if (style.TextStyle.FontStyle != null)
                {
                    if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline)
                    || style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                    {
                        var decorations = new W.TextDecorationCollection();

                        if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
                        {
                            decorations = new W.TextDecorationCollection(
                                decorations.Union(W.TextDecorations.Underline));
                        }

                        if (style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                        {
                            decorations = new W.TextDecorationCollection(
                                decorations.Union(W.TextDecorations.Strikethrough));
                        }

                        ft.SetTextDecorations(decorations);
                    }
                }

                _textCache.Set(text, (tbind, ft, style));

                _dc.DrawText(ft, GetTextOrigin(style, ref rect, ft));
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IImageShape image, double dx, double dy)
        {
            if (image.Key == null)
                return;

            var _dc = dc as WM.DrawingContext;
            var style = image.Style;
            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy);

            if (style != null)
            {
                double thickness = style.Thickness / State.ZoomX;
                double half = thickness / 2.0;

                GetCached(style, thickness, out var fill, out var stroke);

                DrawRectangleInternal(_dc, half, fill, stroke, image.IsStroked, image.IsFilled, ref rect);
            }

            var imageCached = _biCache.Get(image.Key);
            if (imageCached != null)
            {
                try
                {
                    _dc.DrawImage(imageCached, rect);
                }
                catch (Exception ex)
                {
                    _serviceProvider.GetService<ILog>()?.LogException(ex);
                }
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                    return;

                try
                {
                    var bytes = State.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        var ms = new System.IO.MemoryStream(bytes);
                        var bi = new WMI.BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = ms;
                        bi.EndInit();
                        bi.Freeze();

                        _biCache.Set(image.Key, bi);

                        _dc.DrawImage(bi, rect);
                    }
                }
                catch (Exception ex)
                {
                    _serviceProvider.GetService<ILog>()?.LogException(ex);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPathShape path, double dx, double dy)
        {
            if (path.Geometry == null)
                return;

            var _dc = dc as WM.DrawingContext;

            var style = path.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / State.ZoomX;
            double half = thickness / 2.0;

            GetCached(style, thickness, out var fill, out var stroke);

            (var pg, var sg, var cs) = _pathCache.Get(path);

            if (pg == path.Geometry && cs == style)
            {
                _dc.DrawGeometry(path.IsFilled ? fill : null, path.IsStroked ? stroke : null, sg);
            }
            else
            {
                sg = _converter.ToStreamGeometry(path.Geometry, dx, dy);

                // TODO: Enable PathShape caching, cache is disabled to enable PathHelper to work.
                //_pathCache.Set(path, (path.Geometry, sg, style));

                _dc.DrawGeometry(path.IsFilled ? fill : null, path.IsStroked ? stroke : null, sg);
            }
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
