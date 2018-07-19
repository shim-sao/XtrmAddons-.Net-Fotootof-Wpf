﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using XtrmAddons.Fotootof.Lib.Base.Interfaces;
using XtrmAddons.Fotootof.SQLiteService;
using XtrmAddons.Net.Application;

namespace XtrmAddons.Fotootof.Lib.Base.Classes.Pages
{
    /// <summary>
    /// Class XtrmAddons Fotootof Server Libraries Base Page.
    /// </summary>
    public abstract partial class PageBase : Page, IContentInitializer, ISizeChanged
    {
        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Properties

        /// <summary>
        /// Property alias to access to the main database connector.
        /// </summary>
        public static SQLiteSvc Db
            => (SQLiteSvc)ApplicationSession.Properties.Database;

        /// <summary>
        /// 
        /// </summary>
        public static object AppWindow 
            => ApplicationSession.Properties.MainWindow;

        /// <summary>
        /// Property to access to the MainWindow content Block.
        /// </summary>
        public static FrameworkElement MainBlockContent
            => ((Window)AppWindow).FindName("BlockContent") as FrameworkElement;

        /// <summary>
        /// Variable page width marging for content adjustement on size changed.
        /// </summary>
        public double MargingWidth { get; set; } = 0; // SystemParameters.VerticalScrollBarWidth

        /// <summary>
        /// Variable page height marging for content adjustement on size changed.
        /// </summary>
        public double MargingHeight { get; set; } = 0; // SystemParameters.HorizontalScrollBarHeight

        #endregion



        #region Methods Abstracts

        /// <summary>
        /// Method to initialize page data model.
        /// </summary>
        public abstract void InitializeModel();

        /// <summary>
        /// Method to initialize and display data context.
        /// </summary>
        public abstract void Control_Loaded(object sender, RoutedEventArgs e);

        /// <summary>
        /// Method called on page size changed event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        public abstract void Control_SizeChanged(object sender, SizeChangedEventArgs e);

        #endregion



        #region Methods

        /// <summary>
        /// Method called after required component initialized.
        /// </summary>
        protected void AfterInitializedComponent()
        {
            InitializeModel();

            // Initialize for the window size changed event.
            MainBlockContent.SizeChanged += PageBase_SizeChanged;

            // Merge main resources.
            Resources.MergedDictionaries.Add(((Window)AppWindow).Resources);
        }

        /// <summary>
        /// Method called on window sized changed.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        protected void PageBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Get main window page container dimensions.
            TraceSize(MainBlockContent);
            
            // Resize page to fit container.
            Width = Math.Max(MainBlockContent.ActualWidth - MargingWidth, 0);
            Height = Math.Max(MainBlockContent.ActualHeight - MargingHeight, 0);
            TraceSize(this);
        }

        #endregion



        #region Methods Debug

        /// <summary>
        /// Method called on window sized changed.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        [Conditional("DEBUG")]
        protected void TraceSize(FrameworkElement fe)
        {
            #if DEBUG_SIZE

            Trace.WriteLine(string.Format("----> Class({0}) : Object({1}) : Name({2})", GetType().Name, fe.GetType().Name, fe.Name));
            Trace.WriteLine("ActualSize = [" + fe.ActualWidth + "," + fe.ActualHeight + "]");
            Trace.WriteLine("Size = [" + fe.Width + "," + fe.Height + "]");
            Trace.WriteLine("RenderSize = [" + fe.RenderSize.Width + "," + fe.RenderSize.Height + "]");
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------");

            #endif
        }

        #endregion
    }
}
