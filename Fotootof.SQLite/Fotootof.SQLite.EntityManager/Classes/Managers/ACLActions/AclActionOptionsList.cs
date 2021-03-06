﻿using Fotootof.SQLite.EntityManager.Base;

using System.Collections.Generic;

namespace Fotootof.SQLite.EntityManager.Managers
{
    /// <summary>
    /// Class XtrmAddons Fotootof Libraries SQLite ACL Actions Entities List Options.
    /// </summary>
    public class AclActionOptionsList : EntitiesOptionsList
    {
        #region Properties

        /// <summary>
        /// Property list of Action field.
        /// </summary>
        public List<string> Actions { get; set; } = new List<string>();

        #endregion
    }
}