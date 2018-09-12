﻿using Fotootof.Components.Server.Logs;
using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Logs.Log4net;
using Fotootof.SQLite.Services;
using Fotootof.Theme;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XtrmAddons.Net.Application;
using XtrmAddons.Net.NotifyIcons;

namespace Fotootof
{
    /// <summary>
    /// <para>Class Fotootof Server Main Window.</para>
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
        /// Variable memory log watcher for log4net memory appender management.
        /// </summary>
        private static readonly MemoryLogWatcher logWatcher = new MemoryLogWatcher();

        /// <summary>
        /// Variable logs page.
        /// </summary>
        private static readonly PageLogsLayout pageLogs = new PageLogsLayout();

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the logs page.
        /// </summary>
        public PageLogsLayout BlockLogs => pageLogs;

        /// <summary>
        /// Property alias to access to the text block container of logs stack.
        /// </summary>
        //public Border BlockContent => BlockContent;

        /// <summary>
        /// Property alias to access to the text block container of logs stack.
        /// </summary>
        public Xceed.Wpf.Toolkit.BusyIndicator BusyIndicator => XCTKBusyIndicator;

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
            // Merge dynamic custom 
            ThemeLoader.MergeThemeTo(Resources, false);

            // Initialize window component.
            InitializeComponent();

            log.Info(Local.Properties.Logs.InitializingApplicationWindowComponent);

            // Main Window to application session.
            ApplicationSession.Properties.MainWindow = this;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method to initialize application content.
        /// </summary>
        private async Task InitializeContentAsync()
        {
            await Task.Delay(10);

            // Assigned page frames.
            FrameBlockLogsName.Navigate(BlockLogs);
            //Frame_Content.Navigate(new PageBrowser());

            // Adjust frame logs content on resize. 
            SizeChanged += BlockLogs.Page_SizeChanged;

            // Show frame logs according to the preferences.
            if (Settings.Controls.Default.MainMenuMenuItemDisplayLogsIsChecked)
            {
                ToggleLogs();
            }
        }

        #endregion



        #region Methods Windows Events

        /// <summary>
        /// Method called on window load event.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBoxs.IsBusy = true;
            await Task.Delay(10);

            // Add application to system tray.
            NotifyIconManager.AddToTray();
            log.Info(Local.Properties.Logs.ApplicationToSystemTray);

            // Add application log watcher event handler.
            AppLogger.UpdateLogTextbox(logWatcher.LogContent);
            logWatcher.LogContent = "";
            logWatcher.Updated += LogWatcher_Updated;

            // Initialize window content.
            await InitializeContentAsync();

            MessageBoxs.IsBusy = false;
        }

        /// <summary>
        /// Method called on window closing event.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="e">The cancel event arguments.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NotifyIconManager.Dispose();
            log.Info(string.Format(Local.Properties.Logs.WindowClosing, GetType().Name));
        }

        /// <summary>
        /// Method called on file exit click.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The routed event arguments.</param>
        private void FileExit_Click(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Method called on window size changed event.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
#if DEBUG_SIZE
            FrameworkElement fe = FindName("BlockContent") as FrameworkElement;
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------");
            Trace.TraceInformation($"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}");
            Trace.TraceInformation("MainWindow.ActualSize = [" + ActualWidth + "," + ActualHeight + "]");
            Trace.TraceInformation("fe.ActualSize = [" + fe.ActualWidth + "," + fe.ActualHeight + "]");
            Trace.TraceInformation("Frame_Content.ActualSize = [" + Frame_Content.ActualWidth + "," + Frame_Content.ActualHeight + "]");
            Trace.TraceInformation("RowGridMain.Height = [" + RowGridMain.Height + "]");
            Trace.TraceInformation("-------------------------------------------------------------------------------------------------------");

#endif
        }

        #endregion


        #region Methods Logs Watcher

        /// <summary>
        /// Method to watch logs for console & application page logs.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        public void LogWatcher_Updated(object sender, EventArgs e)
        {
            AppLogger.UpdateLogTextbox(logWatcher.LogContent);
            logWatcher.LogContent = "";
        }

        /// <summary>
        /// Method to toggle logs frame.
        /// </summary>
        public void ToggleLogs()
        {
            // Set the row grid splitter Height.
            RowGridSplitter.Height =
                RowGridSplitter.Height == new GridLength(0)
                ? new GridLength(6) : new GridLength(0);

            // Set the grid row logs height.
            RowGridLogs.Height =
                RowGridLogs.Height == new GridLength(0)
                ? new GridLength(Settings.Controls.Default.MainWindowRowGridLogs) : new GridLength(0);
        }

        #endregion
    }
}