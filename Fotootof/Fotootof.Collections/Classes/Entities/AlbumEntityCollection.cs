﻿using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Logs;
using Fotootof.Navigator;
using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Enums.EntityHelper;
using Fotootof.SQLite.EntityManager.Interfaces;
using Fotootof.SQLite.EntityManager.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XtrmAddons.Fotootof.Lib.Api.Models.Json;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Collections.Entities
{
    /// <summary>
    /// Class XtrmAddons Fotootof Server Libraries Common Albums Collection.
    /// </summary>
    public class AlbumEntityCollection : CollectionBaseEntity<AlbumEntity, AlbumOptionsList>
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
        /// Property to set or check if auto load is enabled.
        /// </summary>
        public override bool IsAutoloadEnabled => true;

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Common Albums Collection Constructor.
        /// </summary>
        /// <param name="autoLoad">Auto load data from database ?</param>
        /// <param name="options">Options for query filters.</param>
        public AlbumEntityCollection(bool autoLoad = false, AlbumOptionsList options = null) : base(autoLoad, options) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Common Albums Collection Constructor.
        /// </summary>
        /// <param name="list">A <see cref="List{AlbumEntity}"/> to paste in.</param>
        public AlbumEntityCollection(List<AlbumEntity> list) : base(list) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Common Albums Collection Constructor.
        /// </summary>
        /// <param name="collection">A <see cref="IEnumerable{AlbumEntity}"/> to paste in.</param>
        public AlbumEntityCollection(IEnumerable<AlbumEntity> collection) : base(collection) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Common Albums Collection Constructor.
        /// </summary>
        /// <param name="list">A list of Album to paste in.</param>
        public AlbumEntityCollection(List<AlbumJson> list) : base()
        {
            if (list == null)
            {
                ArgumentNullException ex = Exceptions.GetArgumentNull(nameof(list), list);
                log.Warn(ex.Output(), ex);
                return;
            }

            if (list.Count == 0)
            {
                return;
            }

            foreach (AlbumJson item in list)
            {
                Add(item.ToEntity());
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method to insert a list of Album entities into the database.
        /// </summary>
        /// <param name="newItems">Thee list of items to add.</param>
        public static IEnumerable<AlbumEntity> DbInsert(List<AlbumEntity> newItems)
        {
            if (newItems == null)
            {
                ArgumentNullException e = Exceptions.GetArgumentNull(nameof(newItems), newItems);
                log.Error(e.Output());
                return null;
            }

            // Check if the list to add is not empty.
            if (newItems.Count() == 0)
            {
                log.Debug($"{typeof(AlbumEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : the list of {typeof(AlbumEntity).Name} to insert is empty.");
                return null;
            }

            IList<AlbumEntity> itemsAdded = new List<AlbumEntity>();
            log.Info($"Adding {newItems.Count()} album{(newItems.Count() > 1 ? "s" : "")} : Please wait...");

            try
            {
                foreach (AlbumEntity entity in newItems)
                {
                    // Get all Album to check some properties before inserting new item.
                    // Format Alias before update.
                    FormatAlias(entity);

                    // Add new item into the database.
                    itemsAdded.Add(Db.Albums.Add(entity));
                    log.Info($"Album [{entity.PrimaryKey}:{entity.Name}] added.");
                }

                // Clear application navigator to refresh it for new data.
                AppNavigatorBase.Clear();
                log.Info($"Adding {itemsAdded.Count()} album{(itemsAdded.Count() > 1 ? "s" : "")} : Done.");
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex); ;
                MessageBoxs.Fatal(ex, $"Adding {newItems.Count() - itemsAdded.Count()}/{newItems.Count()} album{(newItems.Count() > 1 ? "s" : "")} : Failed.");
            }

            return itemsAdded;
        }

        /// <summary>
        /// Method to delete a list of Album entities from the database.
        /// </summary>
        /// <param name="oldItems">The list of items to remove.</param>
        public static async Task<IList<AlbumEntity>> DbDeleteAsync(IEnumerable<AlbumEntity> oldItems)
        {
            // Log error if the list to update if not null.
            if (oldItems == null)
            {
                ArgumentNullException e = Exceptions.GetArgumentNull(nameof(oldItems), oldItems);
                log.Error(e.Output(), e);
                return null;
            }

            // Check if the list to update is not empty.
            if (oldItems.Count() == 0)
            {
                log.Debug($"{typeof(AlbumEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : the list of {typeof(AlbumEntity).Name} to delete is empty.");
                return null;
            }

            // Create a new list for update return.
            IList<AlbumEntity> itemsDeleted = new List<AlbumEntity>();
            log.Info($"Deleting {oldItems.Count()} album{(oldItems.Count() > 1 ? "s" : "")} : Please wait...");

            // Check for Removing items.
            try
            {
                foreach (AlbumEntity entity in oldItems)
                {
                    itemsDeleted.Add(await Db.Albums.DeleteAsync(entity));
                    log.Info($"Album [{entity.PrimaryKey}:{entity.Name}] deleted.");
                }

                // Clear application navigator to refresh it for new data.
                AppNavigatorBase.Clear();
                log.Info($"Deleting {itemsDeleted.Count()} album{(itemsDeleted.Count() > 1 ? "s" : "")} : Done !");
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, $"Deleting {oldItems.Count() - itemsDeleted.Count()}/{oldItems.Count()} album{(oldItems.Count() > 1 ? "s" : "")} : Failed.");
            }

            return itemsDeleted;
        }

        /// <summary>
        /// Method to update an Album entity into the database.
        /// </summary>
        /// <param name="newItem">The item to update.</param>
        /// <param name="oldItem"></param>
        public static async Task<IList<AlbumEntity>> DbUpdateAsync(AlbumEntity newItem, AlbumEntity oldItem)
        {
            return await DbUpdateAsync(new AlbumEntity[] { newItem }, new AlbumEntity[] { oldItem });
        }

        /// <summary>
        /// Method to update a list of Album entities into the database.
        /// </summary>
        /// <param name="newItems">The list of items to update.</param>
        /// <param name="oldItems"></param>
        public static async Task<IList<AlbumEntity>> DbUpdateAsync(IEnumerable<AlbumEntity> newItems, IEnumerable<AlbumEntity> oldItems)
        {
            // Log error if the list to update if not null.
            if (newItems == null)
            {
                ArgumentNullException e = Exceptions.GetArgumentNull(nameof(newItems), newItems);
                log.Error(e.Output(), e);
                return null;
            }

            // Check if the list to update is not empty.
            if (newItems.Count() == 0)
            {
                log.Debug($"{typeof(AlbumEntityCollection).Name}.{MethodBase.GetCurrentMethod().Name} : the list of {typeof(AlbumEntity).Name} to update is empty.");
                return null;
            }

            // Create a new list for update return.
            IList<AlbumEntity> itemsUpdated = new List<AlbumEntity>();
            log.Info($"Updating {newItems.Count()} album{(newItems.Count() > 1 ? "s" : "")} : Please wait...");

            try
            {
                foreach (AlbumEntity entity in newItems)
                {
                    // Get all Album to check some properties before inserting new item.
                    // Format Alias before update.
                    FormatAlias(entity);

                    // Update item into the database.
                    itemsUpdated.Add(await Db.Albums.UpdateAsync(entity));
                    log.Info($"Album [{entity.PrimaryKey}:{entity.Name}] updated.");
                }

                // Clear application navigator to refresh it for new data.
                AppNavigatorBase.Clear();
                log.Info($"Updating {itemsUpdated.Count()} album{(itemsUpdated.Count() > 1 ? "s" : "")} : Done !");
            }
            catch (Exception ex)
            {
                log.Error(ex.Output(), ex);
                MessageBoxs.Fatal(ex, $"Updating {newItems.Count() - itemsUpdated.Count()}/{newItems.Count()} album{(newItems.Count() > 1 ? "s" : "")} : Failed.");
            }

            return itemsUpdated;
        }

        /// <summary>
        /// Method to format the Alias property of an entity.
        /// </summary>
        /// <param name="entity">An entity with an Alias property derived from IAlias.</param>
        /// <returns></returns>
        protected static AlbumEntity FormatAlias(AlbumEntity entity)
        {
            var obj = (IEntityNameAlias)entity;

            // Check if the alias is empty. Set name if required.
            if (obj.Alias.IsNullOrWhiteSpace())
            {
                obj.Alias = obj.Name;
            }

            // Check if another entity with the same alias is in database.
            var alb = Db.Albums.SingleOrNull(
                new AlbumOptionsSelect
                {
                    Alias = obj.Alias,
                    Dependencies = { EnumEntitiesDependencies.None }
                });

            if ((alb != null && alb.PrimaryKey != obj.PrimaryKey) || obj.Alias.IsNullOrWhiteSpace())
            {
                DateTime d = DateTime.Now;
                obj.Alias += "-" + d.ToString("yyyy-MM-dd") + "-" + d.ToString("HH-mm-ss-fff");
            }

            ((IEntityNameAlias)entity).Alias = obj.Alias;

            return entity;
        }

        #endregion
    }
}
