﻿using Fotootof.Collections.Entities;
using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Windows;
using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Layouts.Controls.DataGrids
{
    /// <summary>
    /// Class XtrmAddons Fotootof Layouts Window Albums Data Grid.
    /// </summary>
    public partial class DataGridAlbumsWindow : WindowLayoutForm
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
        public new DataGridAlbumsWindowModel Model { get; private set; }

        /// <summary>
        /// Proper to get selected Albums.
        /// </summary>
        public ObservableCollection<AlbumEntity> SelectedAlbums 
            => (FindName("DataGridAlbumsControlName") as DataGridAlbumsLayout).SelectedAlbums;

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
        public DataGridAlbumsWindow() : base()
        {
            InitializeComponent();
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method called on Window load event.
        /// </summary>
        public void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Assign Window closing event.
            Closing += Window_Closing;

            // Assign list of AclGroup to the model.
            Model = new DataGridAlbumsWindowModel(this);
            LoadAlbums();

            // Add events handlers to the albums container.
            //DataGridAlbumsControlName.OnAdd += UCAlbumsContainer_OnAdd;
            //DataGridAlbumsControlName.OnCancel += UCAlbumsContainer_OnCancel;
            //DataGridAlbumsControlName.OnChange += UCAlbumsContainer_OnChangeAsync;
            //DataGridAlbumsControlName.OnDelete += UCAlbumsContainer_OnDeleteAsync;

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
                MessageBoxs.IsBusy = true;
                log.Warn("Starting loading Albums list. Please wait...");

                // Load Albums from database.
                Model.Albums = new AlbumEntityCollection(true);
            }
            catch (Exception e)
            {
                log.Error(e.Output(), e);
                MessageBoxs.Error(new InvalidOperationException("An error occurs while loading Albums list from database ! See logs for further informations."));
            }

            // Stop to busy application.
            finally
            {
                log.Warn("Ending loading Albums list.");
                MessageBoxs.IsBusy = false;
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
                MessageBoxs.IsBusy = true;
                log.Warn("Starting canceling operation. Please wait...");

                // No operation at this moment.

                // Stop to busy application.
                log.Warn("Endind canceling operation.");
                MessageBoxs.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Error(ex.Output());
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
                MessageBoxs.IsBusy = true;
                log.Info("Starting saving Album informations. Please wait...");

                AlbumEntity item = (AlbumEntity)e.NewEntity;
                Model.Albums.Add(item);
                AlbumEntityCollection.DbInsert(new List<AlbumEntity> { item });

                // Stop to busy application.
                log.Warn("Ending saving Album informations.");
                MessageBoxs.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Error(ex.Output());
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
                MessageBoxs.IsBusy = true;
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
                MessageBoxs.IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Error(ex.Output());
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
                MessageBoxs.IsBusy = true;
                log.Warn("Starting deleting Album(s). Please wait...");

                // Remove item from list.
                AlbumEntity item = (AlbumEntity)e.NewEntity;
                Model.Albums.Remove(item);

                // Delete item from database.
                await AlbumEntityCollection.DbDeleteAsync(new List<AlbumEntity> { item });

                // Stop to busy application.
                log.Warn("Ending deleting Album(s).");
                MessageBoxs.IsBusy = false;

            }
            catch(Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Error(ex.Output());
            }
        }

        #endregion
    }
}