﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Data.Tables.Dependencies;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Data.Tables.Entities;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Manager.Base;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Scheme;

namespace XtrmAddons.Fotootof.Lib.SQLite.Database.Manager
{
    /// <summary>
    /// Class XtrmAddons Fotootof Libraries SQLite AclAction Entities Manager.
    /// </summary>
    public partial class AclActionManager : EntitiesManager
    {
        #region Constructors

        /// <summary>
        /// Class XtrmAddons Fotootof Libraries SQLite AclActions Entities Manager Constructor.
        /// </summary>
        /// <param name="context"></param>
        public AclActionManager(DatabaseContextCore context) : base(context) { }

        #endregion



        #region Methods

        /// <summary>
        /// Method to add an AclGroup to an AclAction.
        /// </summary>
        /// <returns>The added user entity.</returns>
        public AclActionEntity AddAclGroupDependency(int aclActionId, int aclGroupId, bool save = true)
        {
            AclActionOptionsSelect options = new AclActionOptionsSelect { PrimaryKey = aclActionId };
            options.Dependencies.Add(EnumEntitiesDependencies.AclGroupsInAclActions);

            AclActionEntity item = Select(options);
            item.AclGroupsInAclActions.Add(new AclGroupsInAclActions { AclGroupId = aclGroupId });

            return Update(item, save);
        }

        /// <summary>
        /// Method to get a list of AclAction entities.
        /// </summary>
        /// <param name="depencies">Load also dependencies.</param>
        /// <returns>A list of AclAction entities.</returns>
        public List<AclActionEntity> List(AclActionOptionsList op = null)
        {
            // Initialize default option list.
            op = op ?? new AclActionOptionsList { };

            // Initialize query.
            IQueryable<AclActionEntity> query = Context.AclActions;

            // Load ACLGroup dependency if required.
            if (op.IsDependOn(EnumEntitiesDependencies.AclGroupsInAclActions))
            {
                query = query.Include(x => x.AclGroupsInAclActions);
            }

            // Check for include primary keys to search in.
            query.QueryListInclude(op, "AclActionId");

            // Check for exclude primary keys in search.
            query.QueryListExclude(op, "AclActionId");

            // Set number elements to skip & the number elements to select.
            query.QueryStartLimit(op);

            // Return a list of entities.
            return query.ToList();
        }

        /// <summary>
        /// Method to delete association between an AclAction and an AclGroup.
        /// </summary>
        /// <param name="aclActionId">The id of the AclAction.</param>
        /// <param name="AclGroupId">The id of the AclGroup.</param>
        /// <returns>Modified AclAction entity as result.</returns>
        public AclActionEntity RemoveAclGroupDependency(int aclActionId, int aclGroupId, bool save = true)
        {
            AclActionOptionsSelect options = new AclActionOptionsSelect { PrimaryKey = aclActionId };
            options.Dependencies.Add(EnumEntitiesDependencies.AclGroupsInAclActions);

            AclActionEntity item = Select(options);
            item.AclGroupsInAclActions.Remove(item.AclGroupsInAclActions.SingleOrDefault(x => x.AclGroupId == aclGroupId));

            return Update(item, save);
        }

        /// <summary>
        /// Method to remove all associations between a AclAction and an AclGroup.
        /// </summary>
        /// <param name="aclActionId">The id of the AclAction.</param>
        /// <param name="aclGroupId">The id of the AclGroup.</param>
        /// <returns>Asynchronous task with modified AclAction entity as result.</returns>
        public async Task<AclActionEntity> RemoveAclGroupDependenciesAsync(int aclActionId, int aclGroupId, bool save = true)
        {
            int result = await Context.Database.ExecuteSqlCommandAsync(
                "DELETE FROM AclGroupsInAclActions"
                + string.Format(" WHERE AclActionId = {0} AND AclGroupId = {1}", aclActionId, aclGroupId)
             );

            if(save)
            {
                Save();
            }

            AclActionOptionsSelect options = new AclActionOptionsSelect { PrimaryKey = aclActionId };
            options.Dependencies.Add(EnumEntitiesDependencies.AclGroupsInAclActions);

            return Select(options);
        }

        /// <summary>
        /// Method to select an AclAction.
        /// </summary>
        /// <param name="op">ACL Actions entities select options to perform query.</param>
        /// <returns>A ACL Action entity or null if not found.</returns>
        public AclActionEntity Select(AclActionOptionsSelect op, bool nullable = false)
        {
            // Initialize query.
            IQueryable<AclActionEntity> query = Context.AclActions;

            // Load ACL Groups dependencies if required.
            if (op.IsDependOn(EnumEntitiesDependencies.AclGroupsInAclActions))
            {
                query = query.Include(x => x.AclGroupsInAclActions);
            }

            // Initialize
            if (op.PrimaryKey > 0)
            {
                if(nullable)
                {
                    return query.SingleOrNull(x => x.AclActionId == op.PrimaryKey);
                }

                return query.SingleOrDefault(x => x.AclActionId == op.PrimaryKey);
            }

            if (op.Action != "")
            {

                if (nullable)
                {
                    return query.SingleOrNull(x => x.Action == op.Action);
                }

                return query.SingleOrDefault(x => x.Action == op.Action);
            }

            throw new ArgumentNullException("AclActionOptionsSelect must contains no empty or null value Primary Key or Action for selection.");
        }
        
        #endregion Methods
    }
}