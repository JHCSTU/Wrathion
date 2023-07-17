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
        private void initData()
        {
            Requests.SetValue("userProjectId","");
            Requests.SetValue("tenantCode","");
            Requests.SetValue("userId","");
        }
        public MainWindow()
        {
            InitializeComponent();
            initData();
            
            var loginView = new Login();
            loginView.Show();
            // Requests.GetProgress();
        }
        
    }
}