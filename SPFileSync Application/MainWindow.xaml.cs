﻿using Microsoft.SharePoint.Client;
using System.Windows;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Web actual = Connection.SharePointResult();
        }
    }
}
