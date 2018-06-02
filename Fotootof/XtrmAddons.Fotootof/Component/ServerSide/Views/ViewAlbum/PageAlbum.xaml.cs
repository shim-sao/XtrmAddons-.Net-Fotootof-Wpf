﻿using System;
using System.Windows;
using XtrmAddons.Fotootof.Lib.Base.Classes.Pages;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Data.Tables.Entities;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Manager;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Manager.Base;
using XtrmAddons.Fotootof.Common.Tools;
using System.Globalization;

namespace XtrmAddons.Fotootof.Component.ServerSide.Views.ViewAlbum
{
    /// <summary>
    /// <para>Class XtrmAddons Fotootof Component Views Server Page Album.</para>
    /// <para>This page provides the entire display for server administration of an image album.</para>
    /// </summary>
    public partial class PageAlbum : PageBase
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
        /// Property to access to the data model of the page.
        /// </summary>
        public PageAlbumModel Model { get; private set; }

        /// <summary>
        /// Property to access to the album's id or primary key.
        /// </summary>
        public int ItemId { get; }

        #endregion



        #region constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Component Views Server Page Album Constructor.
        /// </summary>
        public PageAlbum(int albumId = 0)
        {
            AppOverwork.IsBusy = true;
            log.Info(string.Format(CultureInfo.CurrentCulture, DLogs.InitializingPageWaiting, "Album"));

            // Store Album primary key.
            ItemId = albumId;

            // Constuct page component.
            InitializeComponent();
            AfterInitializedComponent();

            // Construct page data model.
            InitializeModel();

            log.Info(string.Format(CultureInfo.CurrentCulture, DLogs.InitializingPageDone, "Album"));
            AppOverwork.IsBusy = false;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method to initialize page content.
        /// </summary>
        public override void Control_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;

            PicturesCollection.Title_Text.Text = Model.Album.Name;
        }

        /// <summary>
        /// Method to initialize page content asynchronously.
        /// </summary>
        public override async void InitializeModel()
        {
            AlbumEntity album = new AlbumEntity();

            if (ItemId > 0)
            {
                AlbumOptionsSelect options = new AlbumOptionsSelect
                {
                    Dependencies = { EnumEntitiesDependencies.All },
                    PrimaryKey = ItemId
                };
                album = await MainWindow.Database.Albums.SingleOrNullAsync(options);
                album = album ?? new AlbumEntity();
            }

            Model = new PageAlbumModel(this)
            {
                Album = album
            };
        }

        #endregion



        #region Methods Size Changed

        /// <summary>
        /// Method called on user control size changed event.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        public override void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fe = ((MainWindow)AppWindow).Block_Content as FrameworkElement;

            this.Width = Math.Max(fe.ActualWidth, 0);
            this.Height = Math.Max(fe.ActualHeight, 0);

            Block_MiddleContents.Width = Math.Max(this.Width, 0);
            Block_MiddleContents.Height = Math.Max(this.Height, 0);

            PicturesCollection.Height = Math.Max(this.Height, 0);

            TraceSize(fe);
            TraceSize(this);
            TraceSize(Block_MiddleContents);
            TraceSize(PicturesCollection);
        }

        #endregion



        #region Obsoletes

        /// <summary>
        /// Method to initialize and display data context.
        /// </summary>
        [Obsolete("Will be remove. None sense...")]
        public override void Page_Loaded_Async(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}