using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace Wrathion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // Test 
        // private void initData()
        // {
        //     Requests.SetHeader("X-Token", "");
        //     Requests.SetValue("userProjectId", "");
        //     Requests.SetValue("tenantCode", "");
        //     Requests.SetValue("userId", "");
        // }

        public MainWindow()
        {
            InitializeComponent();
            // Show Login Form to Load Data
            var l = new Login();
            l.Show();
            // Load Internet Img
            Image.Source = BitmapFrame.Create(new Uri("https://jcdn.lawliet.ren/qrcode.jpg", false),
                BitmapCreateOptions.None, BitmapCacheOption.Default);
        }
    }
}