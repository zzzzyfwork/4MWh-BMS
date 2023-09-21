using EMS.Common.Modbus.ModbusTCP;
using EMS.Model;
using EMS.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EMS.View
{
    /// <summary>
    /// DevControlView.xaml 的交互逻辑
    /// </summary>
    public partial class DevControlView : Page
    {
        private List<BatteryTotalBase> batteryTotalBases;
        private List<ModbusClient> Clients;
        private List<DevControlViewModel> ViewModels;
        public DevControlView()
        {
            InitializeComponent();
            ViewModels = new List<DevControlViewModel>();
        }

        public void SyncContent(List<BatteryTotalBase> TotalList, List<ModbusClient> ClientList)
        {
            batteryTotalBases = TotalList;
            Clients = ClientList;
            InitDevList();
        }

        private void InitDevList()
        {
            BCMUInfo.Items.Clear();
            // 初始化BCMU列表
            for (int i = 0; i < batteryTotalBases.Count; i++)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/Online.png"));
                image.Height = 50;

                TextBlock textBlock = new TextBlock();
                textBlock.Margin = new Thickness(5, 0, 10, 0);
                textBlock.VerticalAlignment = VerticalAlignment.Bottom;
                textBlock.Text = batteryTotalBases[i].TotalID;

                ListBox listBox = new ListBox();
                listBox.Items.Add(image);
                listBox.Items.Add(textBlock);
                
                //StackPanel stackPanel = new StackPanel();
                //stackPanel.Orientation = Orientation.Horizontal;
                //stackPanel.Children.Add(image);
                //stackPanel.Children.Add(textBlock);

                RadioButton radioButton = new RadioButton();
                radioButton.Click += RadioButton_Click;
                radioButton.Content = listBox;

                BCMUInfo.Items.Add(radioButton);
                
                DevControlViewModel viewmodel = new DevControlViewModel(Clients[i]);
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
            int index = BCMUInfo.Items.IndexOf(item);
            this.DataContext = ViewModels[index];
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            string pattern = @"^[\x00-\x7F]*$";
            if (textBox.Text.Length ==16)
            {
                
                e.Handled = true;
                return;
            }
            if (!Regex.IsMatch(text, pattern))
            {
                MessageBox.Show("请输入正确字符");
                
            }


        }
    }
}
