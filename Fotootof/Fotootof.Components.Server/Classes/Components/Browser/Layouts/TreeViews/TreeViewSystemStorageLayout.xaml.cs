﻿using Fotootof.Layouts.Dialogs;
using Fotootof.Libraries.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using XtrmAddons.Net.Common.Extensions;

namespace Fotootof.Components.Server.Browser.Layouts
{
    /// <summary>
    /// Class XtrmAddons Fotootof Components Server Browser Storage System Browser Tree View Directory.
    /// </summary>
    public partial class TreeViewSystemStorageLayout : ControlLayout
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
        /// Property to access to the <see cref="TreeViewSystemStorageModel"/> of the layout.
        /// </summary>
        internal TreeViewSystemStorageModel Model { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Visibility IsHeaderVisible
        {
            get => FindName<FrameworkElement>("StackPanelBlockHeaderName").Visibility;
            set => FindName<FrameworkElement>("StackPanelBlockHeaderName").Visibility = value;
        }

        #endregion



        #region Constructors

        /// <summary>
        /// Class XtrmAddons Fotootof Components Server Browser Storage System Browser Tree View Directory Constructor.
        /// </summary>
        public TreeViewSystemStorageLayout() : base()
        {
            InitializeComponent();
            InitializeContent();
        }

        #endregion



        #region Methods

        /// <summary>
        /// Method called on user control loaded event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">Routed event arguments <see cref="RoutedEventArgs"/></param>
        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            ArrangeTreeView();
            DataContext = Model;
        }


        /// <summary>
        /// Method to initialize control content.
        /// </summary>
        public void InitializeContent()
        {
            Model = new TreeViewSystemStorageModel(this);
        }

        /// <summary>
        /// Method to expand the tree of sub-directories <see cref="TreeViewItem"/> of a directory.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The routed event arguments <see cref="RoutedEventArgs"/>.</param>
        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.ExpandTreeViewItem(e.Source as TreeViewItem);
            }
            catch (Exception ex)
            {
                log.Debug(ex.Output(), ex);
                MessageBoxs.Error(ex);
            }
        }

        /// <summary>
        /// Method called on clear collection click event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The routed event arguments <see cref="RoutedEventArgs"/>.</param>
        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            Model.Reinitialize();
        }

        /// <summary>
        /// Method to arrange the new size of the <see cref="TreeView"/>
        /// </summary>
        private void ArrangeTreeView()
        {
            // Get framework elements.
            var root = FindName<FrameworkElement>("GridBlockRootName");
            var header = FindName<FrameworkElement>("StackPanelBlockHeaderName");
            var tv = FindName<TreeView>("TreeViewDirectoryInfoName");

            // Process resize of the tree view.
            tv.Height = root.ActualHeight;
            if (header.IsVisible)
            {
                tv.Height -= header.ActualHeight;
            }
        }

        /// <summary>
        /// Method called on <see cref="FrameworkElement"/> size changed event.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> sender of the event.</param>
        /// <param name="e">The size changed event arguments <see cref="SizeChangedEventArgs"/>.</param>
        public override void Layout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangeTreeView();
        }

        #endregion
    }
}