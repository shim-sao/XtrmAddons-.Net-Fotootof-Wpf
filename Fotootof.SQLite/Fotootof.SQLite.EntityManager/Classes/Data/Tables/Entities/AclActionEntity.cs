﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Fotootof.SQLite.EntityManager.Data.Base;
using Fotootof.SQLite.EntityManager.Data.Tables.Dependencies;
using Fotootof.SQLite.EntityManager.Data.Tables.Dependencies.Observables;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.SQLite.EntityManager.Data.Tables.Entities
{
    /// <summary>
    /// Class XtrmAddons Fotootof Libraries SQLite Acl Action Entity.
    /// </summary>
    [Serializable]
    [Table("AclActions")]
    [JsonObject(MemberSerialization.OptIn, Title = "AclAction")]
    [XmlType(TypeName = "AclAction")]
    public partial class AclActionEntity : EntityBase
    {
        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        [NotMapped, XmlIgnore]
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Variable action of the item.
        /// </summary>
        [NotMapped, XmlIgnore]
        private string action = "";

        /// <summary>
        /// Variable parameters of the item.
        /// </summary>
        [NotMapped, XmlIgnore]
        private string parameters = "";

        #endregion



        #region Variables Dependencies

        /// <summary>
        /// Variable AclGroup id (required for entity dependency).
        /// </summary>
        [NotMapped, XmlIgnore]
        private int aclGroupId = 0;

        #endregion



        #region Properties

        /// <summary>
        /// Property to access to the primary key auto incremented.
        /// </summary>
        [Key]
        [Column(Order = 0)]
        [XmlIgnore]
        public int AclActionId
        {
            get => PrimaryKey;
            set
            {
                if (value != primaryKey)
                {
                    PrimaryKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property to access to the action of the item.
        /// </summary>
        [Column(Order = 1)]
        [JsonProperty(PropertyName = "Action")]
        [XmlAttribute(DataType = "string", AttributeName = "Action")]
        public string Action
        {
            get { return action; }
            set
            {
                if (value != action)
                {
                    action = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property to access to the parameters of the item.
        /// </summary>
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "Parameters")]
        [XmlAttribute(DataType = "string", AttributeName = "Parameters")]
        public string Parameters
        {
            get { return parameters; }
            set
            {
                if (value != parameters)
                {
                    parameters = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion



        #region Properties : Dependencies

        /// <summary>
        /// <para>Property to access to an AclGroup Id or Primary Key.</para>
        /// <para>Don't use it. It is only required for EntityFramework foreign key dependency.</para>
        /// </summary>
        [NotMapped, XmlIgnore]
        public int AclGroupId
        {
            get => aclGroupId;
            set
            {
                if (value != aclGroupId)
                {
                    aclGroupId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property to access to the list of AclGroup dependencies primary key.
        /// </summary>
        [NotMapped]
        public ObservableCollection<int> AclGroupsPKeys
        {    
            get
            {
                AclGroupsInAclActions.Populate();
                return AclGroupsInAclActions.DepPKeys;
            }
        }

        /// <summary>
        /// Property to access to the list of AclGroup associated to the AclAction.
        /// </summary>
        [NotMapped]
        [JsonProperty(PropertyName = "AclGroups", ItemConverterType = typeof(Array))]
        [XmlElement(ElementName = "AclGroups")]
        public ObservableCollection<AclGroupEntity> AclGroups
        {
            get
            {
                AclGroupsInAclActions.Populate();
                return AclGroupsInAclActions.DepReferences;
            }
            set
            {
                if (value != AclGroupsInAclActions.DepReferences)
                {
                    AclGroupsInAclActions.DepReferences.ClearAndAdd(value);
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property to access to the collection of relationship AclGroup in AclAction.
        /// </summary>
        [JsonProperty(PropertyName = "AclGroups_AclActions")]
        [XmlElement(ElementName = "AclGroups_AclActions")]
        public ObservableAclGroupsInAclActions<AclActionEntity, AclGroupEntity> AclGroupsInAclActions { get; set; }
            = new ObservableAclGroupsInAclActions<AclActionEntity, AclGroupEntity>();

        #endregion



        #region Constructor

        /// <summary>
        /// Class XtrmAddons Fotootof Libraries SQLite AclAction Entity Constructor.
        /// </summary>
        public AclActionEntity() { }

        #endregion



        #region Methods

        /// <summary>
        /// Method to associate an AclGroup to the AclAction.
        /// </summary>
        /// <param name="aclGroupPk">An AclGroup id or primary key to link.</param>
        /// <returns>True if link process is successful otherwise false.</returns>
        [System.Obsolete("Use => AclGroupsPKeys.Add(aclGroupPk);")]
        public bool LinkAclGroup(int aclGroupPk)
        {
            Debug.WriteLine("System.Obsolete : Use => AclGroupsPKeys.Add(aclGroupPk);");

            try
            {
                int index = FindIndexDependencyAclGroup(aclGroupPk);
                if (index < 0)
                {
                    AclGroupsInAclActions.Add(new AclGroupsInAclActions { AclGroupId = aclGroupPk });
                }
                return true;
            }
            catch (Exception e)
            {
                log.Debug(e.Output());
                return false;
            }
        }

        /// <summary>
        /// Method to dissociate an AclGroup of the AclAction.
        /// </summary>
        /// <param name="aclGroupPk">An AclGroup id or primary key to unlink.</param>
        /// <returns>True if unlink process is successful otherwise false.</returns>
        [System.Obsolete("Use => AclGroupsPKeys.Remove(aclGroupPk);")]
        public bool UnLinkAclGroup(int aclGroupPk)
        {
            Debug.WriteLine("System.Obsolete : Use => AclGroupsPKeys.Remove(aclGroupPk);");

            try
            {
                int index = FindIndexDependencyAclGroup(aclGroupPk);
                if (index > 0)
                {
                    AclGroupsInAclActions.RemoveAt(index);
                }
                return true;
            }
            catch (Exception e)
            {
                log.Debug(e.Output());
                return false;
            }
        }

        /// <summary>
        /// Method to an AclGroup dependency.
        /// </summary>
        /// <param name="aclGroupPk">An AclGroup id or primary key to find.</param>
        /// <returns>The index of the dependency or -1, on error or if it is not found.</returns>
        [System.Obsolete("")]
        public int FindIndexDependencyAclGroup(int aclGroupPk)
        {
            try
            {
                return AclGroupsInAclActions.ToList().FindIndex(x => x.AclGroupId == aclGroupPk);
            }
            catch (Exception e)
            {
                log.Debug(e.Output());
                return -1;
            }
        }

        #endregion
    }
}