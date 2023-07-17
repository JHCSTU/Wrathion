using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Wrathion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var loginView = new Login();
            loginView.Show();
        }
        
    }
}