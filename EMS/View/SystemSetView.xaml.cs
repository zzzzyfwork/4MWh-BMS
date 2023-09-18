﻿using EMS.Model;
using EMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EMS.View
{
    /// <summary>
    /// SystemSetView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetView : Window
    {
        public SystemSetView()
        {
            InitializeComponent();
        }

        public SystemSetView(SystemConfigurationBase SystemConfiguration)
        {
            InitializeComponent();
            DaqContent.DataContext = SystemConfiguration.daqConfiguration;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
