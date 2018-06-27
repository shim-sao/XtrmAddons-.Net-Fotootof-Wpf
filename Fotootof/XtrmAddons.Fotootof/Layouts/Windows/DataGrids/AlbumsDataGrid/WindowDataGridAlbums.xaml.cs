﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using XtrmAddons.Fotootof.Common.Collections;
using XtrmAddons.Fotootof.Lib.Base.Classes.AppSystems;
using XtrmAddons.Fotootof.Lib.Base.Classes.Windows;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Data.Tables.Entities;
using XtrmAddons.Fotootof.Lib.SQLite.Event;
using XtrmAddons.Net.Common.Extensions;

namespace XtrmAddons.Fotootof.Layouts.Windows.DataGrids.AlbumsDataGrid
{
    /// <summary>
    /// Class XtrmAddons Fotootof Layouts Window Albums Data Grid.
    /// </summary>
    public partial class WindowDataGridAlbums : WindowBaseForm
    {
        #region Variable

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the Window model.
        /// </summary>
        public new WindowDataGridAlbumsModel<WindowDataGridAlbums> Model { get; private set; }

        /// <summary>
        /// Proper to get selected Albums.
        /// </summary>
        public ObservableCollection<AlbumEntity> SelectedAlbums => UCAlbumsContainer.SelectedAlbums;

        /// <summary>
        /// Variable old Album informations backup.
        /// </summary>
        public AlbumEntityCollection OldForm { get; set; }

        /// <summary>
        /// Variable old Album informations backup.
        /// </summary>
        public AlbumEntityCollection NewForm
        {
            get => Model.Albums;
            set => Model.Albums = value;
        }

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Layouts Window Albums Data Grid Constructor.
        /// </summary>
        public WindowDataGridAlbums() : base()
        {
            InitializeComponent();
            Loaded += (s, e) => Window_Loaded();
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method called on Window load event.
        /// </summary>
        public void Window_Loaded()
        {
            // Assign Window closing event.
            Closing += Window_Closing;

            // Assign list of AclGroup to the model.
            Model = new WindowDataGridAlbumsModel<WindowDataGridAlbums>(this);
            LoadAlbums();

            // Add events handlers to the albums container.
            UCAlbumsContainer.OnAdd += UCAlbumsContainer_OnAdd;
            UCAlbumsContainer.OnCancel += UCAlbumsContainer_OnCancel;
            UCAlbumsContainer.OnChange += UCAlbumsContainer_OnChangeAsync;
            UCAlbumsContainer.OnDelete += UCAlbumsContainer_OnDeleteAsync;

            // Add model to the Window context.
            DataContext = Model;

            // Add selection changed handler
            SelectedAlbums.CollectionChanged += SelectedAlbums_CollectionChanged;
        }

        /// <summary>
        /// Method called on albums collection selection changed.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Notify collection changed event arguments.</param>
        private void SelectedAlbums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedAlbums.Count > 0)
            {
                ((Button)FindName("Button_Save")).IsEnabled = true;
            }
            else
            {
                ((Button)FindName("Button_Save")).IsEnabled = false;
            }
        }

        /// <summary>
        /// Method to load the list of Album from database.
        /// </summary>
        private void LoadAlbums()
        { 
            try
            {
                // Start to busy application.
                MessageBase.IsBusy = true;
                log.Warn("Starting loading Albums list. Please wait...");

                // Load Albums from database.
                Model.Albums = new AlbumEntityCollection(true);
            }
            catch (Exception e)
            {
                log.Error(e.Output(), e);
                MessageBase.Error(new InvalidOperationException("An error occurs while loading Albums list from database ! See logs for further informations."));
            }

            // Stop to busy application.
            finally
            {
                log.Warn("Ending loading Albums list.");
                MessageBase.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on Album view cancel event.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Entity changes event arguments.</param>
        private void UCAlbumsContainer_OnCancel(object sender, EntityChangesEventArgs e)
        {
            try
            {
                // Start to busy application.
                MessageBase.IsBusy = true;
                log.Warn("Starting canceling operation. Please wait...");

                // No operation at this moment.

                // Stop to busy application.
                log.Warn("Endind canceling operation.");
                MessageBase.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBase.Error(ex.Output());
            }
        }

        /// <summary>
        /// Method called on Album add event.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Entity changes event arguments.</param>
        private void UCAlbumsContainer_OnAdd(object sender, EntityChangesEventArgs e)
        {
            try
            {
                // Start to busy application.
                MessageBase.IsBusy = true;
                log.Info("Starting saving Album informations. Please wait...");

                AlbumEntity item = (AlbumEntity)e.NewEntity;
                Model.Albums.Add(item);
                AlbumEntityCollection.DbInsert(new List<AlbumEntity> { item });

                // Stop to busy application.
                log.Warn("Ending saving Album informations.");
                MessageBase.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBase.Error(ex.Output());
            }
        }

        /// <summary>
        /// Method called on Album change event asynchounously.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Entity changes event arguments.</param>
        private async void UCAlbumsContainer_OnChangeAsync(object sender, EntityChangesEventArgs e)
        {
            try
            {
                // Start to busy application.
                MessageBase.IsBusy = true;
                log.Info("Starting updating Album informations. Please wait...");

                // Get old & new entity informations.
                AlbumEntity newEntity = (AlbumEntity)e.NewEntity;
                AlbumEntity old = Model.Albums.Single(x => x.PrimaryKey == newEntity.PrimaryKey);

                // Update the database.
                newEntity = (await AlbumEntityCollection.DbUpdateAsync(new AlbumEntity[] { newEntity }, new AlbumEntity[] { old }))[0];

                // Replace the old entity in the model by the new one. 
                int index = Model.Albums.IndexOf(old);
                Model.Albums[index] = newEntity;

                // Stop to busy application.
                log.Warn("Ending updating Album informations.");
                MessageBase.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBase.Error(ex.Output());
            }
        }

        /// <summary>
        /// Method called on Album delete event asynchounously.
        /// </summary>
        /// <param name="sender">The object sender of the event.</param>
        /// <param name="e">Entity changes event arguments.</param>
        private async void UCAlbumsContainer_OnDeleteAsync(object sender, EntityChangesEventArgs e)
        {
            try
            {
                // Start to busy application.
                MessageBase.IsBusy = true;
                log.Warn("Starting deleting Album(s). Please wait...");

                // Remove item from list.
                AlbumEntity item = (AlbumEntity)e.NewEntity;
                Model.Albums.Remove(item);

                // Delete item from database.
                await AlbumEntityCollection.DbDeleteAsync(new List<AlbumEntity> { item });

                // Stop to busy application.
                log.Warn("Ending deleting Album(s).");
                MessageBase.IsBusy = false;

            }
            catch(Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBase.Error(ex.Output());
            }
        }

        #endregion
    }
}
