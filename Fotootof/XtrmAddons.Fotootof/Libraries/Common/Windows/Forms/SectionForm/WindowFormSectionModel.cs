﻿using XtrmAddons.Fotootof.Lib.Base.Classes.Windows;
using XtrmAddons.Fotootof.Lib.SQLite.Database.Data.Tables.Entities;
using XtrmAddons.Fotootof.Libraries.Common.Collections;

namespace XtrmAddons.Fotootof.Libraries.Common.Windows.Forms.SectionForm
{
    /// <summary>
    /// Class XtrmAddons Fotootof Libraries Common Window Section Form Model.
    /// </summary>
    public class WindowFormSectionModel<WindowSectionForm> : WindowBaseFormModel<WindowSectionForm>
    {
        #region Variables

        /// <summary>
        /// Variable Section entity.
        /// </summary>
        private SectionEntity section;

        /// <summary>
        /// Variable AclGroups collection.
        /// </summary>
        private AclGroupEntityCollection aclGroups;

        /// <summary>
        /// Variable Albums collection.
        /// </summary>
        public AlbumEntityCollection albums;

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the Section.
        /// </summary>
        public SectionEntity Section
        {
            get { return section; }
            set
            {
                section = value;
                RaisePropertyChanged("Section");
            }
        }

        /// <summary>
        /// Property to access to the AclGroups collection.
        /// </summary>
        public AclGroupEntityCollection AclGroups
        {
            get { return aclGroups; }
            set
            {
                aclGroups = value;
                RaisePropertyChanged("AclGroups");
            }
        }

        /// <summary>
        /// Property to access to the Albums collection.
        /// </summary>
        public AlbumEntityCollection Albums
        {
            get { return albums; }
            set
            {
                albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Libraries Common Window Section Form Model Constructor.
        /// </summary>
        /// <param name="windowBase">The associated window form base.</param>
        public WindowFormSectionModel(WindowSectionForm windowBase) : base(windowBase) { }

        #endregion
    }
}