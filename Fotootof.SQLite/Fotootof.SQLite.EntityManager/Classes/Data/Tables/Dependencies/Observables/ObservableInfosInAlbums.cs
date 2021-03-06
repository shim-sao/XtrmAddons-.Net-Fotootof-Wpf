﻿using Newtonsoft.Json;
using System.Collections.Generic;
using Fotootof.SQLite.EntityManager.Data.Base;

namespace Fotootof.SQLite.EntityManager.Data.Tables.Dependencies.Observables
{
    /// <summary>
    /// Class Fotootof.SQLite.EntityManager Data Tables Dependencies Observable Infos In Albums.
    /// </summary>
    /// <typeparam name="O">The Type of the entity item to observe.</typeparam>
    /// <typeparam name="E">The Type of the entity items destination of the dependency.</typeparam>
    [JsonArray(Title = "Infos_Albums")]
    public class ObservableInfosInAlbums<O, E> : ObservableDependencyBase<InfosInAlbums, O, E> where O : class where E : class
    {
        /// <summary>
        /// Class Fotootof.SQLite.EntityManager Data Tables Dependencies Observable Infos In Albums Constructor.
        /// </summary>
        public ObservableInfosInAlbums() : base() { }

        /// <summary>
        /// Class Fotootof.SQLite.EntityManager Data Tables Dependencies Observable Infos In Albums Constructor.
        /// </summary>
        /// <param name="list">A list of items to add at the collection initialization. </param>
        public ObservableInfosInAlbums(List<InfosInAlbums> list) : base(list) { }

        /// <summary>
        /// Class Fotootof.SQLite.EntityManager Data Tables Dependencies Observable Infos In Albums Constructor.
        /// </summary>
        /// <param name="collection">A enumerable collection of items to add at the collection initialization.</param>
        public ObservableInfosInAlbums(IEnumerable<InfosInAlbums> collection) : base(collection) { }
    }
}
