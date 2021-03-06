﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Shapes;
using System.Collections.Immutable;

namespace Core2D.UI.Wpf.Views.Custom.Lists
{
    /// <summary>
    /// The <see cref="ListBox"/> control for <see cref="BaseShape"/> items with drag and drop support.
    /// </summary>
    public class BaseShapeDragAndDropListBox : DragAndDropListBox<IBaseShape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseShapeDragAndDropListBox"/> class.
        /// </summary>
        public BaseShapeDragAndDropListBox()
            : base()
        {
            Initialized += (s, e) => base.Initialize();
        }

        /// <summary>
        /// Updates DataContext binding to ImmutableArray collection property.
        /// </summary>
        /// <param name="array">The updated immutable array.</param>
        protected override void UpdateDataContext(ImmutableArray<IBaseShape> array)
        {
            var editor = (IProjectEditor)Tag;

            var layer = editor.Project.CurrentContainer.CurrentLayer;

            var previous = layer.Shapes;
            var next = array;
            editor.Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;
        }
    }
}
