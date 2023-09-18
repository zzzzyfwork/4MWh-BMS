﻿using EMS.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMS.View
{
    /// <summary>
    /// DataAnalysis_OptimizeView.xaml 的交互逻辑
    /// </summary>
    public partial class DataAnalysis_OptimizeView : Page
    {
        private DataAnalysisViewModel viewmodel;
        public DataAnalysis_OptimizeView()
        {
            InitializeComponent();
           
            viewmodel = new DataAnalysisViewModel();
            this.DataContext = viewmodel;

           


        }

       
          
        
        
        
           
        
            
            


        private void BatteryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    viewmodel.SelectedBatteryList.Add((item as ListBoxItem).Content.ToString());
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    viewmodel.SelectedBatteryList.Remove((item as ListBoxItem).Content.ToString());

                }
            }

            viewmodel.SwitchBatteryData();
        }

        private void DataTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewmodel.SwitchBatteryData();
        }
    }
}
