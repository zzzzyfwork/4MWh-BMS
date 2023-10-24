using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMS.Common.Modbus.ModbusTCP;
using EMS.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace EMS.ViewModel
{
    public class PCSSettingViewModel:ObservableObject
    {
        
        private string _testinfo;
        public string TestInfo
        {
            get => _testinfo;
            set
            {
                SetProperty(ref  _testinfo, value);
            }
        }

        /// <summary>
        /// IP、port及状态
        /// </summary>
        private string _iP;
        public string IP
        {
            get => _iP;
            set
            {
                SetProperty(ref _iP, value);
            }
        }

        private string _port;
        public string Port
        {
            get => _port;
            set
            {
                SetProperty(ref _port, value);
            }
        }

        private SolidColorBrush _connectStateColor;
        public SolidColorBrush ConnectStateColor
        {
            get => _connectStateColor;
            set
            {
                SetProperty(ref _connectStateColor, value);
            }
        }

        private string _conncetState;
        public string ConncetState
        {
            get => _conncetState;
            set
            {
                SetProperty(ref _conncetState, value);
            }
        }

        private string _dateTimeText;
        public string DateTimeText
        {
            get => _dateTimeText;
            set
            {
                SetProperty(ref _dateTimeText, value);
            }
        }

        private string _timeTime;
        public string TimeTime
        {
            get => _timeTime;
            set
            {
                SetProperty(ref _timeTime, value);
            }
        }

        private string _weekTime;
        public string WeekTime
        {
            get => _weekTime;
            set
            {
                SetProperty(ref _weekTime, value);
            }
        }

        public RelayCommand ModifyPCSTCPCommand {  get; set; }
        public RelayCommand ConnectPCSTCPCommand { get; set; }
        public ModbusClient modbusClient;
        public PCSSettingViewModel()
        {
            ConncetState = "未连接";
            ConnectStateColor = new SolidColorBrush(Colors.Red);
            ModifyPCSTCPCommand = new RelayCommand(ModifyPCSTCP);
            ConnectPCSTCPCommand = new RelayCommand(ConnectPCSTCP);
            TestInfo = "停止";
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval=TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            
        }

        private void ShouStratgy(DateTime now)
        {
            
            if (now.Hour == 16 && now.Minute == 41)
            {
                TestInfo = "开始";
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
           
            DateTimeText = now.ToString("yyyy年MM月dd日");
            TimeTime = now.ToString("HH:mm:ss");
            WeekTime = now.ToString("dddd");
            ShouStratgy(now);
        }

        private void ModifyPCSTCP()
        {
            AddDevView view = new AddDevView();
            view.PCSGrid.Visibility = Visibility.Visible;
            view.TCPGrid.Visibility = Visibility.Collapsed;
            view.PCSRaB.IsChecked = true;
            view.IsPCS =true;
            view.BCMURaB.IsEnabled = false;
            if (view.ShowDialog()== true)
            {
                if(view.IsPCS==true)
                {
                    IP = view.IPText2.AddressText;
                    Port = view.TCPPort2.Text;

                }
            }
        }
        private void ConnectPCSTCP()
        {
            if(IP !=null&&Port!=null)
            {
                int.TryParse(Port, out int Portint);
                modbusClient = new ModbusClient(IP, Portint);
                try
                {
                    modbusClient.Connect();
                    ConncetState = "已连接";
                    ConnectStateColor = new SolidColorBrush(Colors.Green);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                
                ShowInfo(modbusClient);
            }
        }

        public void ShowInfo(ModbusClient client)
        {
            byte[] info = new byte[2];
            //client.WriteFunc(1,51000,123);
            Array.Copy(client.AddReadRequest(51000, 1), 0, info, 0, 2);
            TestInfo = BitConverter.ToUInt16(info, 0).ToString();
        }




    }
}
