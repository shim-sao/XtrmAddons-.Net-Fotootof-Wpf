﻿using Fotootof.Collections.Entities;
using Fotootof.Components.Server.Section.Layouts;
using Fotootof.Layouts.Controls.DataGrids;
using Fotootof.Layouts.Controls.ListViews;
using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Components;
using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Enums.EntityHelper;
using Fotootof.SQLite.EntityManager.Event;
using Fotootof.SQLite.EntityManager.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Components.Server.Section
{
    /// <summary>
    /// Class XtrmAddons Fotootof Component Server Page Section Model.
    /// </summary>
    public partial class PageSectionLayout : ComponentView
    {
        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, int> filters = new Dictionary<string, int>();

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the page model.
        /// </summary>
        internal PageSectionModel Model { get; private set; }

        /// <summary>
        /// Accessors to page user model.
        /// </summary>
        [System.Obsolete("use Model instead.", true)]
        public AlbumOptionsList AlbumOptionsListFilters
        {
            get
            {
                bool showAllAlbums = Settings.Sections.Default.ServerLayout_ShowAllAlbums;
                IEnumerable<SectionEntity> lse = FindName<DataGridSectionsLayout>("DataGridSectionsLayoutName").SelectedItems;

                AlbumOptionsList op = new AlbumOptionsList
                {
                    Dependencies = { EnumEntitiesDependencies.AlbumsInSections, EnumEntitiesDependencies.InfosInAlbums }
                };

                if (lse != null && !showAllAlbums)
                {
                    op.IncludeSectionKeys = new List<int>();
                    foreach (var se in lse)
                        op.IncludeSectionKeys.Add(se.PrimaryKey);
                }

                if (filters.Count > 0)
                {
                    op.IncludeInfoKeys = filters.Values.ToList();
                }

                return op;
            }
        }

        #endregion



        #region constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Server Component Sections Page Constructor.
        /// </summary>
        public PageSectionLayout()
        {
            MessageBoxs.IsBusy = true;
            log.Warn(string.Format(CultureInfo.CurrentCulture, Local.Properties.Logs.InitializingPageWaiting, "Section Server"));

            // Constuct page component.
            InitializeComponent();
            AfterInitializedComponent();

            log.Info(string.Format(CultureInfo.CurrentCulture, Local.Properties.Logs.InitializingPageDone, "Section Server"));
            MessageBoxs.IsBusy = false;
        }

        #endregion



        #region Methods
        
        /// <summary>
        /// Method to initialize and display data context.
        /// </summary>
        public override void Control_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
        }

        /// <summary>
        /// Method to initialize and display data context.
        /// </summary>
        public override void InitializeModel()
        {
            var UcDataGridSections = (DataGridSectionsLayout)FindName("DataGridSectionsLayoutName");
            var UcListViewAlbums = (ListViewAlbumsLayout)FindName("ListViewAlbumsLayoutName");

            // Paste page to the model & child elements.
            Model = new PageSectionModel(this)
            {
                Sections = new DataGridSectionsModel<DataGridSectionsControl>(UcDataGridSections),
                Albums = new ListViewAlbumsModel(UcListViewAlbums)
            };
            
            Model.LoadSections();

            if (Settings.Sections.Default.ServerLayout_ShowAllAlbums)
            {
                Model.LoadAlbums();
            }
            
            Model.FiltersQuality = InfoEntityCollection.TypesQuality();
            Model.FiltersColor = InfoEntityCollection.TypesColor();

            UcDataGridSections.Added += SectionsDataGrid_Added;
            UcDataGridSections.Changed += SectionsDataGrid_Changed;
            UcDataGridSections.Canceled += SectionsDataGrid_Canceled;
            UcDataGridSections.Deleted += SectionsDataGrid_Deleted;
            UcDataGridSections.DefaultChanged += SectionsDataGrid_DefaultChanged;
            UcDataGridSections.SelectionChanged += (s, es) => { RefreshAlbums(); };

            UcListViewAlbums.Added += AlbumsListView_OnAdd;
            UcListViewAlbums.Changed += AlbumsListView_OnChange;
            UcListViewAlbums.Canceled += AlbumsListView_OnCancel;
           // UcListViewAlbums.OnDelete += AlbumsListView_OnDeleteAsync;
        }

        #endregion



        #region Methods : Section

        /// <summary>
        /// Method called on <see cref="SectionEntity"/> edit view cancel event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void SectionsDataGrid_Canceled(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Canceling Section informations edit. Please wait...");

                //Model.LoadSections();

                log.Warn("Canceling Section informations edit. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Canceling Section informations edit. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="SectionEntity"/> add view save event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void SectionsDataGrid_Added(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Saving new Section informations. Please wait...");

                Model.AddSection((SectionEntity)e.NewEntity);

                log.Warn("Saving new Section informations. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Saving new Section informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="SectionEntity"/> update view save event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void SectionsDataGrid_Changed(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Saving Section informations. Please wait...");
                
                Model.UpdateSection((SectionEntity)e.NewEntity);
                RefreshAlbums();

                log.Warn("Saving Section informations. Done.");
            }

            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Saving Section informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="SectionEntity"/> delete event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void SectionsDataGrid_Deleted(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Deleting Section informations. Please wait...");

                Model.DeleteSection((SectionEntity)e.OldEntity);

                log.Warn("Deleting Section informations. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Deleting Section informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="SectionEntity"/> default change event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void SectionsDataGrid_DefaultChanged(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Setting default Section. Please wait...");

                SectionEntity newEntity = (SectionEntity)e.NewEntity;
                SectionEntityCollection.SetDefault(newEntity);
                Model.LoadSections();

                log.Warn("Setting default Section. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Setting default Section. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        #endregion



        #region Methods : Album

        /// <summary>
        /// Method to load the list of <see cref="AlbumEntity"/> from database.
        /// </summary>
        [System.Obsolete("use Model instead.", true)]
        private void LoadAlbums()
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Loading Albums list. Please wait...");

                Model.Albums.Items = new AlbumEntityCollection(true, AlbumOptionsListFilters);

                log.Warn($"Loading {Model?.Albums?.Items?.Count() ?? 0} Albums. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Loading Albums list. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method to refresh the list of <see cref="AlbumEntity"/> from database.
        /// </summary>
        private void RefreshAlbums()
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Refreshing Albums list. Please wait...");

                FindName<ListViewAlbumsLayout>("ListViewAlbumsLayoutName").Visibility = Visibility.Hidden;
                Model.LoadAlbums();
                FindName<ListViewAlbumsLayout>("ListViewAlbumsLayoutName").Visibility = Visibility.Visible;

                log.Warn($"Refreshing {Model?.Albums?.Items?.Count() ?? 0} Albums. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Refreshing Albums list. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="ListView"/> <see cref="AlbumEntity"/> candeled event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void AlbumsListView_OnCancel(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Canceling Album informations edit. Please wait...");

                // LoadAlbums();

                log.Warn("Canceling Album informations edit. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Canceling Album informations edit. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="ListView"/> <see cref="AlbumEntity"/> added event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void AlbumsListView_OnAdd(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Saving Album informations. Please wait...");

                Model.AddAlbum((AlbumEntity)e.NewEntity);

                log.Warn("Saving Album informations. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Saving Album informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="ListView"/> <see cref="AlbumEntity"/> changed event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void AlbumsListView_OnChange(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Saving Album informations. Please wait...");

                Model.UpdateAlbum((AlbumEntity)e.NewEntity);

                log.Warn("Saving Album informations. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Saving Album informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        /// <summary>
        /// Method called on <see cref="ListView"/> <see cref="AlbumEntity"/> deleted event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The entity changes event arguments <see cref="EntityChangesEventArgs"/>.</param>
        private void AlbumsListView_OnDelete(object sender, EntityChangesEventArgs e)
        {
            try
            {
                MessageBoxs.IsBusy = true;
                log.Warn("Deleting Album informations. Please wait...");

                Model.DeleteAlbum((AlbumEntity)e.NewEntity);

                log.Warn("Deleting Album informations. Done.");
            }

            catch (Exception ex)
            {
                log.Fatal(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Deleting Album informations. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }
        }

        #endregion



        #region Methods Filters

        /// <summary>
        /// Method called on quality filter selection changed event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The celection changed event arguments <see cref="SelectionChangedEventArgs"/></param>
        private void FiltersQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ((ComboBox)((ListViewAlbumsLayout)FindName("ListViewAlbumsLayoutName"))
                .FindName("FiltersQualitySelector")).IsEditable = false;

                if (((ComboBox)sender).SelectedItem is InfoEntity info)
                {
                    Model.ChangeFiltersQuality(info);
                }
            }

            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Filters Quality Selection Changed. Fail.");
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }

            RefreshAlbums();
        }

        /// <summary>
        /// Method called on color filter selection changed event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The celection changed event arguments <see cref="SelectionChangedEventArgs"/></param>
        private void FiltersColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ((ComboBox)((ListViewAlbumsLayout)FindName("ListViewAlbumsLayoutName"))
                    .FindName("FiltersColorSelector")).IsEditable = false;

                if (((ComboBox)sender).SelectedItem is InfoEntity info)
                {
                    Model.ChangeFiltersColor(info);
                }
            }

            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Error(ex);
            }

            finally
            {
                MessageBoxs.IsBusy = false;
            }

            RefreshAlbums();
        }

        #endregion



        #region Methods : Layout Size Changed

        /// <summary>
        /// Method called on <see cref="FrameworkElement"/> size changed event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The size changed event arguments <see cref="SizeChangedEventArgs"/>.</param>
        public override void Layout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                // Initialize some variables
                var blockContent = FindName<FrameworkElement>("BlockMiddleContentName");
                var topContent = FindName<FrameworkElement>("BlockTopControlsName");
                var tabContentW = ((Frame)MainBlockContentTabs.SelectedContent).ActualWidth;
                var tabContentH = ((Frame)MainBlockContentTabs.SelectedContent).ActualHeight;

                var UcDataGridSections = FindName<DataGridSectionsLayout>("DataGridSectionsLayoutName");
                var UcListViewAlbums = FindName<ListViewAlbumsLayout>("ListViewAlbumsLayoutName");

                // Arrange this height & width
                Width = Math.Max(tabContentW, 0);
                Height = Math.Max(tabContentH, 0);

                blockContent.Width = Width;
                blockContent.Height =
                    UcDataGridSections.Height =
                    UcListViewAlbums.Height =
                Math.Max(Height - topContent.RenderSize.Height, 0);
            }

            catch(Exception ex)
            {
                log.Debug(ex.Output(), ex);
                MessageBoxs.Error(ex);
            }
        }

        #endregion
    }
}