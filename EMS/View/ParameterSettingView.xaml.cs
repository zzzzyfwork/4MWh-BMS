using EMS.Common.Modbus.ModbusTCP;
using EMS.Model;
using EMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// ParameterSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterSettingView : Page
    {
        private List<BatteryTotalBase> batteryTotalBases;
        private List<ModbusClient> Clients;
        private List<ParameterSettingViewModel> ViewModels;
        public ParameterSettingView()
        {
            InitializeComponent();
            ViewModels = new List<ParameterSettingViewModel>();
        }
        public void SyncContent(List<BatteryTotalBase> TotalList, List<ModbusClient> ClientList)
        {
            batteryTotalBases = TotalList;
            Clients = ClientList;
            InitDevList();
        }
        private void InitDevList()
        {
            BCMUInfo2.Items.Clear();
            // 初始化BCMU列表
            for (int i = 0; i < batteryTotalBases.Count; i++)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/Online.png"));
                image.Height = 50;
                image.Margin=new Thickness(5,0,0,0);

                TextBlock textBlock = new TextBlock();
                textBlock.Margin = new Thickness(5, 0, 10, 0);
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.Text = batteryTotalBases[i].TotalID;

                ListBox listBox = new ListBox();
                listBox.Items.Add(image);
                listBox.Items.Add(textBlock);

                RadioButton radioButton = new RadioButton();
                radioButton.Click += RadioButton_Click;
                radioButton.Content = listBox;

                BCMUInfo2.Items.Add(radioButton);
                ParameterSettingViewModel viewmodel = new ParameterSettingViewModel(Clients[i]);
                ViewModels.Add(viewmodel);
                if (i == 0)
                {
                    radioButton.IsChecked = true;
                    this.DataContext = ViewModels[i];
                }
            }

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as RadioButton;
            int index = BCMUInfo2.Items.IndexOf(item);
            this.DataContext = ViewModels[index];
        }

      
    }
}

