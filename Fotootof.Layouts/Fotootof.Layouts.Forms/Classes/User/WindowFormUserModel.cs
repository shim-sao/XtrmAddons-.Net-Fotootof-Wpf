﻿using Fotootof.Collections.Entities;
using Fotootof.Libraries.Windows;
using Fotootof.SQLite.EntityConverters.ValueConverters;
using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Enums.EntityHelper;
using Fotootof.SQLite.EntityManager.Managers;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Layouts.Forms.User
{
    /// <summary>
    /// Class XtrmAddons Fotootof Layouts Window User Form Model.
    /// </summary>
    public class WindowFormUserModel : WindowLayoutFormModel<WindowFormUserLayout>
    {
        #region Variables

        /// <summary>
        /// Variable logger <see cref="log4net.ILog"/>.
        /// </summary>
        private static readonly log4net.ILog log =
        	log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Variable to store the <see cref="UserEntity"/>.
        /// </summary>
        protected UserEntity user;

        /// <summary>
        /// Variable to store the old <see cref="UserEntity"/>.
        /// </summary>
        protected UserEntity oldUser;

        /// <summary>
        /// Variable to store the collection of AclGroup entities <see cref="AclGroupEntityCollection"/>.
        /// </summary>
        protected AclGroupEntityCollection aclGroups;

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the User <see cref="UserEntity"/>.
        /// </summary>
        public UserEntity User
        {
            get => user;
            set
            {
                user = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to access to the old User <see cref="UserEntity"/>.
        /// </summary>
        public UserEntity OldUser
        {
            get => oldUser;
            set
            {
                oldUser = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to access to the AclGroups list <see cref="AclGroupEntityCollection"/>.
        /// </summary>
        public AclGroupEntityCollection AclGroups
        {
            get => aclGroups;
            set
            {
                aclGroups = value;
                NotifyPropertyChanged();
            }
        }

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Layouts Window User Form Model Constructor.
        /// </summary>
        /// <param name="controlView">The <see cref="object"/> owner associated to the model.</param>
        public WindowFormUserModel(WindowFormUserLayout controlView) : base(controlView) { }

        /// <summary>
        /// Class XtrmAddons Fotootof Layouts Window User Form Model Constructor.
        /// </summary>
        /// <param name="controlView">The <see cref="object"/> owner associated to the model.</param>
        /// <param name="UserId">An <see cref="UserEntity"/> unique id or primary key.</param>
        public WindowFormUserModel(WindowFormUserLayout controlView, int UserId) : this(controlView)
        {
            LoadUser(UserId); ;

            // Set model entity to dependencies converters.
            IsAclGroupInUser.Entity = User;

            // Assign list of AclGroup to the model.
            AclGroups = new AclGroupEntityCollection(true);
        }

        /// <summary>
        /// Method to load the informations of a user <see cref="UserEntity"/>
        /// </summary>
        /// <param name="UserId">An unique identifier or primary key.</param>
        public void LoadUser(int UserId)
        {
            User = null;
            if (UserId > 0)
            {
                var op = new UserOptionsSelect
                {
                    PrimaryKey = UserId,
                    Dependencies = { EnumEntitiesDependencies.All }
                };

                User = Db.Users.SingleOrNull(op);
            }

            User = User ?? new UserEntity();
            OldUser = User?.CloneJson();
        }

        /// <summary>
        /// Method to check if an email is unique.
        /// </summary>
        /// <param name="email">A email string to check.</param>
        public bool IsUniqueEmail(string email)
        {
            if(email.IsNullOrWhiteSpace())
            {
                return false;
            }

            if(Db.Users.SingleOrNull(new UserOptionsSelect { Email = email }) != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to check if a name is unique.
        /// </summary>
        /// <param name="name">A name string to check.</param>
        public bool IsUniqueName(string name)
        {
            if(name.IsNullOrWhiteSpace())
            {
                return false;
            }

            if(Db.Users.SingleOrNull(new UserOptionsSelect { Name = name }) != null)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}