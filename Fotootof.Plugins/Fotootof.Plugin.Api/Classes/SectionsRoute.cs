﻿using Fotootof.SQLite.EntityManager.Data.Tables.Entities;
using Fotootof.SQLite.EntityManager.Enums.EntityHelper;
using Fotootof.SQLite.EntityManager.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XtrmAddons.Net.Common.Extensions;
using XtrmAddons.Net.HttpWebServer.Requests;
using XtrmAddons.Net.HttpWebServer.Responses;

namespace Fotootof.Plugin.Api.Router
{
    /// <summary>
    /// Class XtrmAddons Fotootof Libraries Api Router Sections.
    /// </summary>
    public class SectionsRoute : Router
    {
        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        /// <summary>
        /// Class XtrmAddons Fotootof Libraries Api Router Sections.
        /// </summary>
        public SectionsRoute() : base() { }

        /// <summary>
        /// Class XtrmAddons Fotootof Libraries Api Router Sections.
        /// </summary>
        /// <param name="uri">The Uri to parse.</param>
        public SectionsRoute(WebServerRequestUrl uri) : base(uri) { }

        /// <summary>
        /// Method to get a list of associated Sections of an User.
        /// </summary>
        /// <returns>WPF Web Server response data of list of Sections.</returns>
        public override WebServerResponseData Index()
        {
            log.Info("Api : Serving sections list request. Please wait...");
            
            if (!IsAuth())
            {
                log.Info("Api : Return response user not auth.");
                return ResponseNotAuth();
            }
            
            try
            {
                // Get user and dependencies.
                UserEntity user = GetAuthUser();
                List<SectionEntity> l = new List<SectionEntity>();
                
                log.Debug($"Api : AclGroups Primary Keys Count = {user.AclGroupsPKeys?.Count}");

                foreach (int groupPK in user.AclGroupsPKeys)
                {
                    AclGroupEntity entity = Database.AclGroups.SingleOrDefault
                        (
                            new AclGroupOptionsSelect

                            {
                                PrimaryKey = groupPK,
                                Dependencies = new List<EnumEntitiesDependencies> { EnumEntitiesDependencies.All }
                            }
                        );

                    log.Debug($"Api : AclGroup Sections Primary Keys Count = {entity.SectionsPKeys?.Count}");

                    foreach (int sectionPK in entity.SectionsPKeys)
                    {
                        if(!l.Exists(x => x.PrimaryKey == sectionPK))
                        {
                            l.Add(
                                Database.Sections.SingleOrNull
                                (
                                    new SectionOptionsSelect
                                    {
                                        PrimaryKey = sectionPK
                                        //Dependencies = new List<EnumEntitiesDependencies> { EnumEntitiesDependencies.All }
                                    }
                                )
                            );
                        }
                    }
                }

                log.Debug($"Api : Sections Count = {l?.Count}");

                var a = l.ToJson();
                var b = a.ToString();

                //log.Debug($"Api : Sections Json = {a}");

                // Get list of folders associated to user.
                Content["Authentication"] = true;
                Content["Response"] = ConvertJsonAuthSections(l);
                return ResponseContentToJson();
            }
            catch (Exception e)
            {
                log.Fatal("Api : list sections failed", e);
                return ResponseError500();
            }
        }

        /// <summary>
        ///  Method to get an associated section.
        /// </summary>
        /// <returns>>WPF Web Server response data of the selected folder.</returns>
        public WebServerResponseData Get()
        {
            log.Debug($"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}");
            log.Info($"{MethodBase.GetCurrentMethod().Name} : Serving section request. Please wait...");

            if (!IsAuth())
            {
                log.Info($"{MethodBase.GetCurrentMethod().Name} : Return response user not auth.");
                return ResponseNotAuth();
            }
            
            try
            {
                // Check if request params are correct.
                if(Uri.Params.Length == 0)
                {
                    log.Info($"{MethodBase.GetCurrentMethod().Name} : Params in the request are empty.");
                    log.Info($"{MethodBase.GetCurrentMethod().Name} : Return response section not found or doesn't exists.");
                    return ResponseError404("Section not found or doesn't exists.");
                }

                // Get user and dependencies.
                UserEntity user = GetAuthUser();
                SectionEntity entity = null;

                // Try to found section in dependencies.
                foreach (AclGroupEntity group in user.AclGroups)
                {
                    AclGroupEntity ag = Database.AclGroups.SingleOrDefault
                        (
                            new AclGroupOptionsSelect
                            {
                                PrimaryKey = group.PrimaryKey,
                                Dependencies = new List<EnumEntitiesDependencies> { EnumEntitiesDependencies.All }
                            }
                        );

                    entity = ag.Sections.ToList().Find(x => x.PrimaryKey == int.Parse(Uri.Params[0]));
                }

                // Check if folder is associated or not.
                if (entity == null)
                {
                    log.Info($"{MethodBase.GetCurrentMethod().Name} : Section not found in user association.");
                    log.Info($"{MethodBase.GetCurrentMethod().Name} : Return response Section not found or doesn't exists.");
                    return ResponseError404("Section not found or doesn't exists.");
                }

                // Ensure to select dependencies.
                log.Info("Api : Creating section informations. Please wait...");
                entity = Database.Sections.SingleOrNull
                    (
                        new SectionOptionsSelect
                        {
                            PrimaryKey = entity.PrimaryKey,
                            Dependencies = new List<EnumEntitiesDependencies> { EnumEntitiesDependencies.All }
                        }
                    );

                Content["Authentication"] = true;
                Content["Response"] = ConvertJsonAuthSection(entity);
                return ResponseContentToJson();
            }
            catch (Exception e)
            {
                log.Fatal($"{MethodBase.GetCurrentMethod().Name} : Serving section request failed.", e);
                return ResponseError500();
            }
        }
    }
}