using EMS.Model;
using EMS.MyControl;
using EMS.Storage.DB.DBManage;
using EMS.Storage.DB.Models;
using EMS.View;
using EMS.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace EMS
{
    /// <summary>
    /// MainWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private PCSSettingViewModel pcsviewmodel;
        private MainViewModel viewmodel;
        DevTest_CollectView devTest_Daq;
        DataAnalysis_OptimizeView dataAnalysis_Optimize;
        DevControlView devControlView;
        ParameterSettingView parameterSettingView;
        PCSSettingView pCSSettingView;
        public MainWindow()
        {
            InitializeComponent();


            viewmodel = new MainViewModel();
            this.DataContext = viewmodel;
            DevListView.DataContext = viewmodel.DisplayContent;
            DaqDataRaBtn.IsChecked = true;
            SelectedPage("DaqDataRaBtn");
            pcsviewmodel = new PCSSettingViewModel();
            PCSIP.DataContext = pcsviewmodel;
            PCSTCPModify.DataContext = pcsviewmodel;
            PCSConncet.DataContext = pcsviewmodel;
            ConnectStateColor.DataContext = pcsviewmodel;
            ConncetState.DataContext = pcsviewmodel;
        }

        private void ReConnect_Click(object sender, RoutedEventArgs e)
        {
            var item = DevList.SelectedItem as BatteryTotalBase;
            try
            {
                if (item.IsConnected)
                {
                    DisConnect_Click(null, null);
                    ReConnect_Click(null, null);
                }
                else
                {
                    // 连接成功后将设备信息添加到左边的导航栏中
                    if (viewmodel.DisplayContent.AddConnectedDev(item))
                    {
                        // 更新数据库中设备信息BCMUID
                        DevConnectInfoManage manage = new DevConnectInfoManage();
                        manage.Update(new DevConnectInfoModel() { BCMUID = item.BCMUID, IP = item.IP, Port = item.Port });
                    }
                }
            }
            catch
            {
                MessageBox.Show("重新连接设备失败，请检查通讯参数和连接介质！");
            }
        }

        private void DisConnect_Click(object sender, RoutedEventArgs e)
        {
            // 断开连接设备
            var item = DevList.SelectedItem as BatteryTotalBase;
            int index = viewmodel.DisplayContent.RemoveDisConnectedDev(item);
        }

        private void InterNet_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("入网操作");
            var item = DevList.SelectedItem as BatteryTotalBase;
            viewmodel.DisplayContent.RequestInterNet(item);
        }

        private void DelDev_Click(object sender, RoutedEventArgs e)
        {
            var item = DevList.SelectedItem as BatteryTotalBase;
            if (!item.IsConnected)
            {
                viewmodel.DisplayContent.BatteryTotalList.Remove(item);
                DevConnectInfoManage manage = new DevConnectInfoManage();
                manage.Delete(new DevConnectInfoModel() { IP = item.IP, Port = item.Port, BCMUID = item.BCMUID });
            }
            else
            {
                MessageBox.Show("请先断开设备连接");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RadioButton_Checked(object sender,RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            SelectedPage(radioButton.Name);
        }

        

        private void SelectedPage(string PageName)
        {
            switch (PageName)
            {
                case "DaqDataRaBtn":
                    if (devTest_Daq == null)
                    {
                        devTest_Daq = new DevTest_CollectView();
                        devTest_Daq.DevSource = viewmodel.DisplayContent.OnlineBatteryTotalList;
                        devTest_Daq.DevSource.CollectionChanged += devTest_Daq.Test_CollectionChanged;
                    }
                    Mainbody.Content = new Frame() { Content = devTest_Daq };
                    break;
                case "AnalysisDataRaBtn":
                    if (dataAnalysis_Optimize == null)
                    {
                        dataAnalysis_Optimize = new DataAnalysis_OptimizeView();
                    }
                    Mainbody.Content = new Frame() { Content = dataAnalysis_Optimize };
                    break;
                case "ControlRaBtn":
                    if (devControlView == null)
                    {
                        devControlView = new DevControlView();
                        
                    } 
                    devControlView.SyncContent(viewmodel.DisplayContent.OnlineBatteryTotalList.ToList(), viewmodel.DisplayContent.ClientList);
                    Mainbody.Content = new Frame() { Content = devControlView };
                    break;

                case "ValueSettingRaBtn":
                    if (parameterSettingView == null)
                    {
                        parameterSettingView = new ParameterSettingView();
                    }
                    parameterSettingView.SyncContent(viewmodel.DisplayContent.OnlineBatteryTotalList.ToList(), viewmodel.DisplayContent.ClientList);
                    Mainbody.Content = new Frame() { Content = parameterSettingView };
                    break;

                case "PCSSettingRaBtn":
                    if(pCSSettingView == null)
                    {
                        pCSSettingView = new PCSSettingView();
                        
                    }
                    //simulationSettingView.SyncContent(viewmodel.DisplayContent.OnlineBatteryTotalList.ToList(), viewmodel.DisplayContent.ClientList);
                    Mainbody.Content = new Frame() { Content = pCSSettingView };
                    break;
                default:
                    break;
            }
        }



        private void OperationManual_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "About");
            string filePath = System.IO.Path.Combine(folderPath, "OperationManual.pdf");

            System.Diagnostics.Process.Start(filePath);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutView view = new AboutView();

            view.ShowDialog();

        }

        
    }
}
