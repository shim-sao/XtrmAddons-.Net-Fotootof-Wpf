﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace XtrmAddons.Fotootof.PluginTest
{
    /// <summary>
    /// Logique d'interaction pour UcPluginTest.xaml
    /// </summary>
    [Export(typeof(UserControl))]
    public partial class UcPluginTest : UserControl
    {
        public UcPluginTest()
        {
            InitializeComponent();
        }
    }
}