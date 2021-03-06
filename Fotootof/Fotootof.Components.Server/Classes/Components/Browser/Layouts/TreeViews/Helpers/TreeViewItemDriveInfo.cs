﻿using Fotootof.Layouts.Dialogs;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XtrmAddons.Net.Common.Extensions;
using XtrmAddons.Net.Picture.Classes;
using XtrmAddons.Net.Picture.Extensions;

namespace Fotootof.Components.Server.Browser.Layouts.Helpers
{
    /// <summary>
    /// Class Fotootof Components Server Browser Layouts Helper Tree View Item DriveInfo.
    /// </summary>
    internal class TreeViewItemDriveInfo : TreeViewItem
    {
        #region Constants

        /// <summary>
        /// Constant header height.
        /// </summary>
        private const int headerHeight = 20;

        #endregion



        #region Variables

        /// <summary>
        /// Variable logger.
        /// </summary>
        private static readonly log4net.ILog log =
        	log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion



        #region Constructor

        /// <summary>
        /// Class Fotootof Components Server Browser Layouts Helper Tree View Item DriveInfo Constructor.
        /// </summary>
        /// <param name="di">A <see cref="DriveInfo"/> create as <see cref="TreeViewItem"/>.</param>
        public TreeViewItemDriveInfo(DriveInfo di)
        {
            // Create the main Grid container.
            Grid header = GetHeader();
            StackPanel title = GetTitle(di);

            // The volume is not found or ready.
            if (title == null)
            {
                return;
            }

            // Create Drive special informations.
            string inf = "NaN";
            try
            {
                int filesCount = 0;
                int dirCount = 0;
                long freeSpace = 0;
                long totalSize = 0;
                int goUnit = 1024 * 1024 * 1024;

                if (di.IsReady)
                {
                    filesCount = di.RootDirectory.GetFiles().Length;
                    dirCount = di.RootDirectory.GetDirectories().Length;
                    freeSpace = di.AvailableFreeSpace;
                    totalSize = di.TotalSize;
                }
                
                inf = $"{filesCount}/{dirCount} - {freeSpace / goUnit}/{totalSize / goUnit}Go";
            }
            catch (Exception e)
            {
                log.Debug(e.Output());
                log.Debug($"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {di?.Name}");
            }

            // Create TextBlock for the special informations.
            TextBlock count = new TextBlock
            {
                Text = inf,
                Margin = new Thickness(0, 0, 10, 0),
                FontStyle = FontStyles.Italic,
                FontSize = 10
            };

            Grid.SetColumn(title, 0);
            Grid.SetColumn(count, 1);
            header.Children.Add(title);
            header.Children.Add(count);

            Header = header;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Tag = di;
            Style = Application.Current.Resources["TreeViewItemStyle"] as Style;

            Items.Add("Loading...");
        }

        /// <summary>
        /// Method to create the <see cref="Grid"/> header container.
        /// </summary>
        /// <returns>A <see cref="Grid"/> as header container.</returns>
        private static Grid GetHeader()
        {
            ColumnDefinition gr1 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
            Grid header = new Grid
            {
                Height = headerHeight
            };
            header.ColumnDefinitions.Add(gr1);
            header.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            return header;
        }

        /// <summary>
        /// Method to create the <see cref="StackPanel"/> title container.
        /// </summary>
        /// <returns>A <see cref="StackPanel"/> as title container.</returns>
        private static StackPanel GetTitle(DriveInfo di)
        {
            // Get the icon image of the Drive.
            BitmapImage icon = Win32Icon.IconFromHandle(di.Name).ToBitmap().ToBitmapImage();
            StackPanel title = title = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 20,
                Children =
                {
                    new Border()
                    {
                        Child = new Image
                        {
                            Width = icon.Width,
                            Height = icon.Height,
                            Source = icon
                        }
                    }
                }
            };

            try
            {
                TextBlock volName = null;

                if (di.IsReady)
                {
                    volName = new TextBlock
                    {
                        Text = $"{di.VolumeLabel} ({di.Name.ToString()})",
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                }
                else
                {
                    volName = new TextBlock
                    {
                        Text = $"Volume not ready ! ({di.Name.ToString()})",
                        Margin = new Thickness(5, 0, 0, 0),
                        Foreground = Brushes.Red
                    };
                }

                title.Children.Add(volName);
            }

            catch (IOException io)
            {
                log.Debug(io.Output());
                MessageBoxs.Error(io);

                title.Children.Add(
                    new TextBlock
                        {
                            Text = $"Volume not ready ! ({di.Name.ToString()})",
                            Margin = new Thickness(5,0,0,0),
                            Foreground = Brushes.Red
                        }
                    );

                title.ToolTip = io.Message;
            }

            catch (Exception e)
            {
                log.Debug(e.Output());
                MessageBoxs.Fatal(e);
            }

            return title;
        }

        #endregion
    }
}
