﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Autofac;
using Avalonia;
#if !_CORERT
using Avalonia.X11;
#endif
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Core2D.UI.Avalonia.Modules;
using Core2D.Interfaces;

namespace Core2D.UI.Avalonia
{
    /// <summary>
    /// Encapsulates an Core2D avalonia program.
    /// </summary>
    internal class Program
    {
        private static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        [STAThread]
        private static void Main(string[] args)
        {
#if !_CORERT
            bool useGpu = true;
            bool useEGL = false;
            bool deferredRendering = true;
            bool useDirect2D1 = false;
            bool useSkia = false;
            bool useWin32 = false;
            bool useX11 = false;
            bool useNative = false;

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "--software":
                        useGpu = false;
                        break;
                    case "--egl":
                        useEGL = true;
                        break;
                    case "--immediate":
                        deferredRendering = false;
                        break;
                    case "--deferred":
                        deferredRendering = true;
                        break;
                    case "--d2d":
                        useDirect2D1 = true;
                        break;
                    case "--skia":
                        useSkia = true;
                        break;
                    case "--win32":
                        useWin32 = true;
                        break;
                    case "--x11":
                        useX11 = true;
                        break;
                    case "--native":
                        useNative = true;
                        break;
                }
            }
#endif
            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DependenciesModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<ViewModule>();
                using (var container = builder.Build())
                {
                    using (var log = container.Resolve<ILog>())
                    {
                        var appBuilder = BuildAvaloniaApp();
#if !_CORERT
                        if (useDirect2D1 == true)
                        {
                            appBuilder.UseDirect2D1();
                        }
                        if (useSkia == true)
                        {
                            appBuilder.UseSkia();
                        }
                        if (useWin32 == true)
                        {
                            appBuilder.UseWin32()
                                      .With(new Win32PlatformOptions { AllowEglInitialization = useEGL, UseDeferredRendering = deferredRendering });
                        }
                        if (useX11 == true)
                        {
                            appBuilder.UseX11()
                                      .With(new X11PlatformOptions { UseGpu = useGpu, UseEGL = useEGL });
                        }
                        if (useNative == true)
                        {
                            appBuilder.UseAvaloniaNative()
                                      .With(new AvaloniaNativePlatformOptions { UseGpu = useGpu, UseDeferredRendering = deferredRendering });
                        }
#endif
                        appBuilder.SetupWithoutStarting();
                        var app = appBuilder.Instance as App;
                        var aboutInfo = app.CreateAboutInfo(
                            appBuilder.RuntimePlatform.GetRuntimeInfo(),
                            appBuilder.WindowingSubsystemName,
                            appBuilder.RenderingSubsystemName);
                        app.Start(container.Resolve<IServiceProvider>(), aboutInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        /// <summary>
        /// Builds Avalonia app.
        /// </summary>
        /// <returns>The Avalonia app builder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
#if !_CORERT
                         .UsePlatformDetect()
#elif _CORERT_WIN_X64
                         .UseWin32().UseSkia()
#elif _CORERT_LINUX_X64
                         .UseX11().UseSkia()
#elif _CORERT_OSX_X64
                         .UseAvaloniaNative().UseSkia()
#endif
                         .UseReactiveUI()
                         .UseDataGrid()
                         .LogToDebug();
    }
}
