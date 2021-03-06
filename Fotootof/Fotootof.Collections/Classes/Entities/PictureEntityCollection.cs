﻿using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Logs;
using Fotootof.Libraries.Models.Systems;
using Fotootof.Navigator;
using Fotootof.SQLite.EntityManager.Data.Tables.Dependencies;
using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Collections.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class PictureEntityCollection : CollectionBaseEntity<PictureEntity, PictureOptionsList>
    {
        #region Variables

        /// <summary>
        /// Variable logger <see cref="log4net.ILog"/>.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public override bool IsAutoloadEnabled => true;

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection Constructor.
        /// </summary>
        /// <param name="autoLoad">Auto load data from database ?</param>
        /// <param name="options">Options for query filters.</param>
        public PictureEntityCollection(PictureOptionsList options = null, bool autoLoad = false)
            : base(autoLoad, options) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection Constructor.
        /// </summary>
        /// <param name="list">A <see cref="List{PictureEntity}"/> to paste in.</param>
        public PictureEntityCollection(List<PictureEntity> list) : base(list) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection Constructor.
        /// </summary>
        /// <param name="collection">A <see cref="IEnumerable{PictureEntity}"/> to paste in.</param>
        public PictureEntityCollection(IEnumerable<PictureEntity> collection) : base(collection) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection Constructor.
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="album"></param>
        public PictureEntityCollection(string[] fileNames, ref AlbumEntity album) : base()
        {
            PictureEntity[] items = FromFileNames(fileNames, ref album);
            foreach (PictureEntity entity in items)
            {
                Add(entity);
            }
        }

        /// <summary>
        /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection Constructor.
        /// </summary>
        /// <param name="fileNames"></param>
        public PictureEntityCollection(string[] fileNames) : base()
        {
            AlbumEntity album = default(AlbumEntity);
            PictureEntity[] items = FromFileNames(fileNames, ref album);
            foreach (PictureEntity entity in items)
            {
                Add(entity);
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Class method to load a list of Album from database.
        /// </summary>
        public override void Load()
        {
            LoadOptions(null);
        }

        /// <summary>
        /// Class method to load a list of Album from database.
        /// </summary>
        /// <param name="options">Options for query filters.</param>
        public new void LoadOptions(PictureOptionsList options = null)
        {
            options = options ?? Options;
            options = options ?? OptionsDefault;

            var items = Db.Pictures.List(options);
            foreach (PictureEntity entity in items)
            {
                Add(entity);
            }
        }

        /// <summary>
        /// Method to create a list of pictures from a list of file names.
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public static PictureEntity[] FromFileNames(string[] fileNames, ref AlbumEntity album)
        {
            // Initialize the list of pictures to add.
            IList<PictureEntity> newItems = new List<PictureEntity>();
            PictureEntity[] pictAdded = null;

            // Create the array of new Picture entities to add in the database.
            foreach (string s in fileNames)
            {
                // Check if storage information is not null
                // Add a new picture to the album.
                PictureEntity item = (new StorageInfoModel(new FileInfo(s))).ToPicture();
                if (item != null && !newItems.Contains(item))
                {
                    // Add Picture to the list for Pictures.
                    newItems.Add(item);
                }
            }

            // Insert Pictures into the database.
            log.Warn($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : {newItems?.Count() ?? 0} Pictures ready to insert into database.");
            pictAdded = DbInsert(newItems, ref album).ToArray();

            return pictAdded;
        }

        /// <summary>
        /// Method to insert a list of Picture entities into the database.
        /// </summary>
        /// <param name="newItems">The list of items to add.</param>
        /// <returns>The list of new items inserted in the database.</returns>
        public static IList<PictureEntity> DbInsert(IEnumerable<PictureEntity> newItems)
        {
            // Create a default albums enumerable list.
            IEnumerable<AlbumEntity> albums = default(IEnumerable<AlbumEntity>);

            // Return the main Db Insert result.
            log.Debug($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} 1 : {newItems?.Count()} newItems to insert into database.");
            return DbInsert(newItems, ref albums);
        }

        /// <summary>
        /// Method to insert a list of Picture entities into the database.
        /// </summary>
        /// <param name="newItems">The list of items to add.</param>
        /// <param name="album">The Album, past on reference, to associate the items and update its informations.</param>
        /// <returns>The list of new items inserted in the database.</returns>
        public static IList<PictureEntity> DbInsert(IEnumerable<PictureEntity> newItems, ref AlbumEntity album)
        {
            // Create a default albums enumerable list.
            // Initialize array with the album past on reference.
            IEnumerable<AlbumEntity> albums = new AlbumEntity[] { album };

            // Return the main Db Insert result.
            log.Debug($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} 2 : {newItems?.Count()} newItems to insert into database.");
            return DbInsert(newItems, ref albums);
        }

        /// <summary>
        /// Method to insert a list of Picture entities into the database.
        /// </summary>
        /// <param name="newItems">The list of items to add.</param>
        /// <param name="albums">The list of Albums, past on reference, to associate the items and update their informations.</param>
        /// <returns>The list of new items inserted in the database.</returns>
        public static IList<PictureEntity> DbInsert(IEnumerable<PictureEntity> newItems, ref IEnumerable<AlbumEntity> albums)
        {
            log.Debug($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} 3 : {newItems?.Count()} newItems to insert into database.");

            if (newItems == null)
            {
                log.Error(Exceptions.GetArgumentNull(nameof(newItems), typeof(IEnumerable<PictureEntity>)).Output());
                return null;
            }

            if (newItems.Count() == 0)
            {
                log.Debug($"{typeof(AlbumEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : the list of {typeof(IEnumerable<PictureEntity>).Name} to insert is empty.");
                return null;
            }

            if (albums == null)
            {
                log.Error(Exceptions.GetArgumentNull(nameof(albums), typeof(IEnumerable<AlbumEntity>)).Output());
                return null;
            }

            // Create new Picture list to return.
            IList<PictureEntity> itemsAdded = new List<PictureEntity>();

            try
            {
                log.Debug("----------------------------------------------------------------------------------------------------------");
                log.Info($"Adding {newItems.Count()} picture{(newItems.Count() > 1 ? "s" : "")} : Please wait...");

                foreach (PictureEntity entity in newItems)
                {
                    if (entity == null)
                    {
                        log.Error($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : Picture null can't be inserted in database.");
                        continue;
                    }

                    // Process association to each AlbumEntity.
                    if (albums != null && albums.Count() > 0)
                    {
                        foreach (AlbumEntity a in albums)
                        {
                            if (a == null)
                            {
                                log.Error(new NullReferenceException($"Album null can't be associated to a Picture.").Output());
                                continue;
                            }

                            // Try to find Picture and Album dependency
                            PicturesInAlbums dependency = (new List<PicturesInAlbums>(entity.PicturesInAlbums)).Find(x => x.AlbumId == a.AlbumId);

                            // Associate Picture to the Album if not already set.
                            if (dependency == null)
                            {
                                entity.PicturesInAlbums.Add(
                                    new PicturesInAlbums
                                    {
                                        AlbumId = a.AlbumId,
                                        Ordering = entity.PicturesInAlbums.Count + 1
                                    }
                                );

                                log.Info($"Picture [{entity.PrimaryKey}:{entity.Name}] associated to Album [{a.PrimaryKey}:{a.Name}].");
                            }
                        }
                    }

                    // Add picture strored in database to the return list.
                    var pictAdded = Db.Pictures.Add(entity);
                    itemsAdded.Add(pictAdded);

                    foreach (AlbumEntity a in albums)
                    {
                        if (a == null)
                        {
                            log.Error(new NullReferenceException($"Picture null can't be associated to a Album.").Output());
                            continue;
                        }

                        a.LinkPicture(pictAdded.PrimaryKey, false);

                        log.Info($"Album [{a.PrimaryKey}:{a.Name}] associated to Picture [{entity.PrimaryKey}:{entity.Name}].");
                    }

                    log.Info($"Picture [{entity.PrimaryKey}:{entity.Name}] added to database.");
                }

                AppNavigatorBase.Clear();
                log.Debug("Application navigator cleared.");
                log.Info($"Adding {itemsAdded?.Count() ?? 0} picture{(itemsAdded?.Count() > 1 ? "s" : "")} : Done.");
            }

            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, $"Adding {newItems.Count()}/{newItems.Count()} picture{(newItems.Count() > 1 ? "s" : "")} : Failed.");
            }

            log.Debug("----------------------------------------------------------------------------------------------------------");
            return itemsAdded;
        }

        /// <summary>
        /// Method to delete a list of Picture entities from the database.
        /// </summary>
        /// <param name="oldItems">The list of items to remove.</param>
        public static async Task<IList<PictureEntity>> DbDeleteAsync(IEnumerable<PictureEntity> oldItems)
        {
            if (oldItems == null)
            {
                ArgumentNullException e = Exceptions.GetArgumentNull(nameof(oldItems), typeof(IEnumerable<PictureEntity>));
                log.Error(e.Output());
                return null;
            }

            // Check if the list to update is not empty.
            if (oldItems.Count() == 0)
            {
                log.Debug($"{typeof(PictureEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : the list of {typeof(PictureEntity).Name} to delete is empty.");
                return null;
            }

            // Create a new list for update return.
            IList<PictureEntity> itemsDeleted = new List<PictureEntity>();
            log.Info($"Deleting {oldItems.Count()} picture{(oldItems.Count() > 1 ? "s" : "")} : Please wait...");

            try
            {
                foreach (PictureEntity entity in oldItems)
                {
                    itemsDeleted.Add(await Db.Pictures.DeleteAsync(entity));
                    log.Info($"Picture [{entity.PrimaryKey}:{entity.Name}] deleted.");
                }

                // Clear application navigator to refresh it for new data.
                AppNavigatorBase.Clear();
                log.Info($"Deleting {itemsDeleted.Count()} picture{(itemsDeleted.Count() > 1 ? "s" : "")} : Done !");
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, $"Deleting {oldItems.Count() - itemsDeleted.Count()}/{oldItems.Count()} picture{(oldItems.Count() > 1 ? "s" : "")} : Failed.");
            }

            return itemsDeleted;
        }

        /// <summary>
        /// Method to update a list of Picture entities into the database.
        /// </summary>
        /// <param name="newItems">The list of items to update.</param>
        /// <param name="oldItems"></param>
        public static async void DbUpdateAsync(List<PictureEntity> newItems, List<PictureEntity> oldItems)
        {
            // Check for Replace | Edit items.
            try
            {
                log.Info("Replacing Picture. Please wait...");

                if (newItems != null && newItems.Count > 0)
                {
                    foreach (PictureEntity entity in newItems)
                    {
                        await Db.Pictures.UpdateAsync(entity);

                        log.Info(string.Format("Picture [{0}:{1}] updated.", entity.PrimaryKey, entity.Name));
                    }
                }

                AppNavigatorBase.Clear();
                log.Info("Replacing Picture(s). Done !");
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, "Replacing Picture(s) failed !");
            }
        }

        #endregion
    }
}