﻿using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XtrmAddons.Fotootof.Component.Logs.Pages;
using XtrmAddons.Fotootof.Component.ServerSide.ViewBrowser;
using XtrmAddons.Fotootof.Culture;
using XtrmAddons.Fotootof.Lib.Base.Classes.Pages;
using XtrmAddons.Fotootof.Libraries.Common.Tools;
using XtrmAddons.Fotootof.SQLiteService;
using XtrmAddons.Net.Application;
using XtrmAddons.Net.NotifyIcons;

namespace XtrmAddons.Fotootof
{
    /// <summary>
    /// Class XtrmAddons Fotootof Server Main Window.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Variable logs page.
        /// </summary>
        private PageLogs pageLogs = new PageLogs();

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the logs page.
        /// </summary>
        public PageLogs PageLogs => pageLogs;

        /// <summary>
        /// Property alias to access to the text block container of logs stack.
        /// </summary>
        public TextBlock LogsStack => PageLogs.TextBlockLogsStack;

        /// <summary>
        /// Property alias to access to the text block container of logs stack.
        /// </summary>
        public Border BlockContent => this.Block_Content;

        /// <summary>
        /// Property to access to the SQLite Service.
        /// </summary>
        public static SQLiteSvc Database
        {
            get => ApplicationSession.Properties.Database;
            set => ApplicationSession.Properties.Database = value;
        }

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Server Main Window Constructor.
        /// </summary>
        public MainWindow()
        {
            // Initialize window component.
            InitializeComponent();
            log.Info(Translation.Logs["InitializingApplicationWindowComponentDone"]);
            
            // Main Window to application session.
            ApplicationSession.Properties.MainWindow = this;
        }

        #endregion


        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add application to system tray.
            NotifyIconManager.AddToTray();
            log.Info(Translation.Logs["AddingApplicationToSystemTrayDone"]);

            // Initialize window content.
            InitializeContentAsync();
        }

        /// <summary>
        /// Method to initialize application content.
        /// </summary>
        private async void InitializeContentAsync()
        {
            AppOverwork.IsBusy = true;
            await Task.Delay(10);

            // Initialize application settings.
            MainSettings.Initialize();

            // Assigned page frames.
            Frame_Logs.Navigate(pageLogs);
            Frame_Content.Navigate(new PageBrowser());

            // Initialize items of Server Menu.
            AppMainMenu.InitializeMenuItemsServer();

            // Adjust frame logs content on resize. 
            SizeChanged += pageLogs.Window_SizeChanged;

            AppOverwork.IsBusy = false;
        }

        /// <summary>
        /// Method called on windows closing event.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="e">The cancel event arguments.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NotifyIconManager.Dispose();
            log.Info(Translation.Logs["ApplicationClosed"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------");
            Trace.WriteLine("MainWindow.ActualSize = [" + ActualWidth + "," + ActualHeight + "]");
            Trace.WriteLine("Block_Content.ActualSize = [" + Block_Content.ActualWidth + "," + Block_Content.ActualHeight + "]");
            Trace.WriteLine("Frame_Content.ActualSize = [" + Frame_Content.ActualWidth + "," + Frame_Content.ActualHeight + "]");
            Trace.WriteLine("RowGridMain.Height = [" + RowGridMain.Height + "]");
            Trace.WriteLine("-------------------------------------------------------------------------------------------------------");
        }

        #endregion
    }
}