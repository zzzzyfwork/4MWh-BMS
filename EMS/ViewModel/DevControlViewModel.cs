using CommunityToolkit.Mvvm.Input;
using EMS.Common.Modbus.ModbusTCP;
using EMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EMS.Model
{
    public class DevControlViewModel : ViewModelBase
    {


        private int _address1;
        /// <summary>
        /// IP地址1：192
        /// </summary>
        public int Address1
        {
            get
            {
                return _address1;
            }
            set
            {
                SetProperty(ref _address1, value);
            }
        }

        private int _address2;
        /// <summary>
        /// IP地址2：168
        /// </summary>
        public int Address2
        {
            get
            {
                return _address2;
            }
            set
            {
                SetProperty(ref _address2, value);
            }
        }

        private int _address3;
        /// <summary>
        /// IP地址3：1
        /// </summary>
        public int Address3
        {
            get
            {
                return _address3;
            }
            set
            {
                SetProperty(ref _address3, value);
            }
        }

        private int _address4;
        /// <summary>
        /// IP地址4：100
        /// </summary>
        public int Address4
        {
            get
            {
                return _address4;
            }
            set
            {
                SetProperty(ref _address4, value);
            }
        }


        private int _mask1;
        /// <summary>
        /// 掩码1：255
        /// </summary>
        public int Mask1
        {
            get
            {
                return _mask1;
            }
            set
            {
                SetProperty(ref _mask1, value);
            }
        }



        private int _mask2;
        /// <summary>
        /// 掩码2：255
        /// </summary>
        public int Mask2
        {
            get
            {
                return _mask2;
            }
            set
            {
                SetProperty(ref _mask2, value);
            }
        }


        private int _mask3;
        /// <summary>
        /// 掩码3：255
        /// </summary>
        public int Mask3
        {
            get
            {
                return _mask3;
            }
            set
            {
                SetProperty(ref _mask3, value);
            }
        }

        private int _mask4;
        /// <summary>
        /// 掩码4：0
        /// </summary>
        public int Mask4
        {
            get
            {
                return _mask4;
            }
            set
            {
                SetProperty(ref _mask4, value);
            }
        }

        private int _gateway1;
        /// <summary>
        /// 网关1：192
        /// </summary>
        public int Gateway1
        {
            get
            {
                return _gateway1;
            }
            set
            {
                SetProperty(ref _gateway1, value);
            }
        }

        private int _gateway2;
        /// <summary>
        /// 网关2：168
        /// </summary>
        public int Gateway2
        {
            get
            {
                return _gateway2;
            }
            set
            {
                SetProperty(ref _gateway2, value);
            }
        }

        private int _gateway3;
        /// <summary>
        /// 网关3：0
        /// </summary>
        public int Gateway3
        {
            get
            {
                return _gateway3;
            }
            set
            {
                SetProperty(ref _gateway3, value);
            }
        }


        private int _gateway4;
        /// <summary>
        /// 网关4：1
        /// </summary>
        public int Gateway4
        {
            get
            {
                return _gateway4;
            }
            set
            {
                SetProperty(ref _gateway4, value);
            }
        }
        /// <summary>
        /// BMU集合
        /// </summary>
        private List<string> _bMUid;
        public List<string> BMUID
        {
            get { return _bMUid; }
            set { SetProperty(ref _bMUid, value); }
        }




        private List<string> _channels;
        /// <summary>
        /// 充电通道集合
        /// </summary>
        public List<string> Channels
        {
            get
            {
                return _channels;
            }
            set
            {
                SetProperty(ref _channels, value);
            }
        }



        private string _selectedChannel;
        /// <summary>
        /// BMU的通道
        /// </summary>
        public string SelectedChannel
        {
            get
            {
                return _selectedChannel;
            }
            set
            {
                SetProperty(ref _selectedChannel, value);
            }
        }



        /// <summary>
        /// 被选择的BMU
        /// </summary>
        private string _selectedBMU;
        public string SelectedBMU
        {
            get
            {
                return _selectedBMU;
            }
            set
            {
                SetProperty(ref _selectedBMU, value);
            }
        }




        /// <summary>
        /// 数据采集模式
        /// </summary>

        private string _selectedDataCollectionMode;
        public string SelectedDataCollectionMode
        {
            get => _selectedDataCollectionMode;
            set
            {
                SetProperty(ref _selectedDataCollectionMode, value);
            }
        }

        private List<string> _dataCollectionMode;
        public List<string> DataCollectionMode
        {
            get => _dataCollectionMode;
            set
            {
                SetProperty(ref _dataCollectionMode, value);
            }
        }
        /// <summary>
        /// 均衡模式选择
        /// </summary>
        private string _selectedBalanceMode;
        public string SelectedBalanceMode
        {
            get => _selectedBalanceMode;
            set
            {
                SetProperty(ref _selectedBalanceMode, value);
            }
        }

        private List<string> _balanceMode;
        public List<string> BalanceMode
        {
            get => _balanceMode;
            set
            {
                SetProperty(ref _balanceMode, value);
            }
        }

        private string _bCMUSName;
        public string BCMUSName
        {
            get => _bCMUSName;
            set
            {
                SetProperty(ref _bCMUSName, value);
            }
        }

        private string _bCMUName;
        public string BCMUName
        {
            get => _bCMUName;
            set
            {
                SetProperty(ref _bCMUName, value);
            }
        }

        public RelayCommand SelectDataCollectionModeCommand { get; set; }

        public RelayCommand ReadNetInfoCommand { get; set; }
        public RelayCommand SyncNetInfoCommand { get; set; }
        public RelayCommand OpenChargeChannelCommand { get; set; }
        public RelayCommand CloseChargeChannelCommand { get; set; }
        public RelayCommand SelectBalancedModeCommand { get; set; }
        public RelayCommand InNetCommand { get; set; }
        public RelayCommand FwUpdateCommand { get; set; }
        private ModbusClient ModbusClient;
        public RelayCommand ReadBCMUIDINFOCommand { get; set; }
        public RelayCommand SyncBCMUIDINFOCommand { get; set; }
        public DevControlViewModel(ModbusClient client)
        {

            ReadNetInfoCommand = new RelayCommand(ReadNetInfo);
            SyncNetInfoCommand = new RelayCommand(SyncNetInfo);
            OpenChargeChannelCommand = new RelayCommand(OpenChargeChannel);
            CloseChargeChannelCommand = new RelayCommand(CloseChargeChannel);
            SelectBalancedModeCommand = new RelayCommand(SelectBalancedMode);
            FwUpdateCommand = new RelayCommand(FwUpdate);
            InNetCommand = new RelayCommand(InNet);
            ReadBCMUIDINFOCommand = new RelayCommand(ReadBCMUIDINFO);
            SyncBCMUIDINFOCommand = new RelayCommand(SyncBCMUIDINFO);
            Channels = new List<string>();
            SelectDataCollectionModeCommand = new RelayCommand(SelectDataCollectionMode);   
            
            for (int i = 1; i <= 14; i++)
            {
                Channels.Add(i.ToString());
            }

            BMUID = new List<string>
            {
                "1",
                "2",
                "3"
            };
            BalanceMode = new List<string>
            {
                "远程模式",
                "自动模式"

            };

            DataCollectionMode = new List<string>
            {
                "正常模式",
                "仿真模式"
            };
            ModbusClient = client;
        }

        private void InNet()
        {
            ModbusClient.WriteFunc(40103, 0xBB11);
        }

        private void SelectBalancedMode()
        {

            if (SelectedBalanceMode == "自动模式")
            {
                ModbusClient.WriteFunc(40102, 0xBC11);

            }
            else if (SelectedBalanceMode == "远程模式")
            {
                ModbusClient.WriteFunc(40102, 0xBC22);

            }
            else
            {
                MessageBox.Show("请选择模式");
            }
        }

        private void CloseChargeChannel()
        {
            int.TryParse(SelectedChannel, out int closechannel);
            switch (SelectedBMU)
            {
                case "1":
                    {

                        UInt16 data = (UInt16)(0xBB11);
                        ModbusClient.WriteFunc(40101, (ushort)data);
                    }
                    break;
                case "2":
                    {
                        UInt16 data = (UInt16)(0xBC11);

                        ModbusClient.WriteFunc(40101, (ushort)data);

                    }
                    break;
                case "3":
                    {
                        UInt16 data = (UInt16)(0xBD11);

                        ModbusClient.WriteFunc(40101, (ushort)data);

                    }
                    break;
                default:
                    {
                        MessageBox.Show("请选择需要关闭的BMU");
                    } break;
            }
        }

        private void OpenChargeChannel()
        {
            int.TryParse(SelectedChannel, out int openchannel);
            switch (SelectedBMU)
            {
                case "1":
                    {

                        UInt16 data = (UInt16)(0xAB00 + openchannel);
                        ModbusClient.WriteFunc(40100, (ushort)data);
                    } break;
                case "2":
                    {
                        UInt16 data = (UInt16)(0xAC00 + openchannel);

                        ModbusClient.WriteFunc(40100, (ushort)data);

                    }
                    break;
                case "3":
                    {
                        UInt16 data = (UInt16)(0xAD00 + openchannel);

                        ModbusClient.WriteFunc(40100, (ushort)data);

                    }
                    break;
                default:
                    {
                        MessageBox.Show("请选择数据");
                    } break;

            }



        }

        private void ReadNetInfo()
        {
            byte[] data = ModbusClient.ReadFunc(40301, 6);
            int ipaddr1 = BitConverter.ToUInt16(data, 0);
            int ipaddr2 = BitConverter.ToUInt16(data, 2);
            int ma1 = BitConverter.ToUInt16(data, 4);
            int ma2 = BitConverter.ToUInt16(data, 6);
            int gw1 = BitConverter.ToUInt16(data, 8);
            int gw2 = BitConverter.ToUInt16(data, 10);
            Address1 = ipaddr1 & 0xFF;//192
            Address2 = (ipaddr1 & 0xFF00) >> 8; //168
            Address3 = ipaddr2 & 0xFF; //0
            Address4 = (ipaddr2 & 0xFF00) >> 8; //102
            Mask1 = ma1 & 0xFF; //255
            Mask2 = (ma1 & 0xFF00) >> 8;//255
            Mask3 = (ma2 & 0xFF);//255
            Mask4 = (ma2 & 0xFF00) >> 8;//0
            Gateway1 = gw1 & 0xFF;//192
            Gateway2 = (gw1 & 0xFF00) >> 8;//168
            Gateway3 = gw2 & 0xFF;//1
            Gateway4 = (gw2 & 0xFF00) >> 8;//1
        }

        private void SyncNetInfo()
        {
            int ipaddr1 = (Address2 << 8) | Address1;
            int ipaddr2 = (Address4 << 8) | Address3;
            int ma1 = (Mask2 << 8) | Mask1;
            int ma2 = (Mask4 << 8) | Mask3;
            int gw1 = (Gateway2 << 8) | Gateway1;
            int gw2 = (Gateway4 << 8) | Gateway3;
            ModbusClient.WriteFunc(40301, (ushort)ipaddr1);
            ModbusClient.WriteFunc(40302, (ushort)ipaddr2);
            ModbusClient.WriteFunc(40303, (ushort)ma1);
            ModbusClient.WriteFunc(40304, (ushort)ma2);
            ModbusClient.WriteFunc(40305, (ushort)gw1);
            ModbusClient.WriteFunc(40306, (ushort)gw2);

        }
        private void FwUpdate()
        {
            ModbusClient.WriteFunc(40104, 0xBBAA);
        }

        private void SelectDataCollectionMode()
        {
            if (SelectedDataCollectionMode == "正常模式")
            {
                //ModbusClient.WriteFunc(40105, 0xAAAA);

            }
            else if (SelectedDataCollectionMode == "仿真模式")
            {
                ModbusClient.WriteFunc(40105, 0xAA55);
            }
            else
            {
                MessageBox.Show("请选择正确模式");
            }

        }

        private void ReadBCMUIDINFO()
        {
            byte[] data = ModbusClient.ReadFunc(40307, 16);
            StringBuilder BCMUNameBuilder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                char BCMUNameChar = Convert.ToChar(data[i]);
                BCMUNameBuilder.Append(BCMUNameChar);
            }

            BCMUName = BCMUNameBuilder.ToString().TrimStart('0');
            StringBuilder BCMUSNameBuilder = new StringBuilder();           
            for (int i = 16; i < 32; i++)
            {
                char BCMUSNameChar = Convert.ToChar(data[i]);
                BCMUSNameBuilder.Append(BCMUSNameChar);
   
            }          
            BCMUSName = BCMUSNameBuilder.ToString().TrimStart('0');
        }

        private void SyncBCMUIDINFO()
        {
            int indexSN = 0; //BCMU序列号数据序号
            int indexN = 0;//BCMU别名序号
            
            
            string BCMUFullSName="";//补足16位的BCMU序列号
            string BCMUFullName = "";//补足16位的BCMU别名
            if (BCMUSName.Length< 16|| BCMUName.Length<16)
            {
                BCMUFullSName = BCMUSName.PadLeft(16, '0');
                BCMUFullName = BCMUName.PadLeft(16, '0');
            }
            else
            {
                BCMUFullSName = BCMUSName;
                BCMUFullName = BCMUName;
            }
            //写BCMU序列号
            for (int i = 0; i < BCMUFullSName.Length; i++)
            {
                int asciiCode = (int)BCMUFullSName[i];
                int asciiCode2;
                if (i % 2 == 0)
                {
                    asciiCode2 = (BCMUFullSName[i + 1]) << 8;                  
                    int nameof = asciiCode | asciiCode2;
                    ModbusClient.WriteFunc((ushort)(40315 + indexSN), (ushort)nameof);
                    indexSN++;
                }
            }
            //写BCMU别名
            for (int i = 0; i < BCMUFullName.Length; i++)
            {
                int asciiCode = (int)BCMUFullName[i];
                if (i % 2 == 0)
                {
                    int asciiCode2 = (BCMUFullName[i + 1]) << 8;
                    int nameof = asciiCode | asciiCode2;
                    ModbusClient.WriteFunc((ushort)(40307 + indexN), (ushort)nameof);
                    indexN++;
                }
            }
 
        }

    }
}
