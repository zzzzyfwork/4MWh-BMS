﻿using CommunityToolkit.Mvvm.Input;
using EMS.Common.Modbus.ModbusTCP;
using EMS.Model;
using EMS.Storage.DB.DBManage;
using EMS.Storage.DB.Models;
using EMS.View;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace EMS.ViewModel
{
    public class DisplayContentViewModel : ViewModelBase
    {
        private ObservableCollection<BatteryTotalBase> _onlineBatteryTotalList;
        /// <summary>
        /// 在线设备集合
        /// </summary>
        public ObservableCollection<BatteryTotalBase> OnlineBatteryTotalList
        {
            get => _onlineBatteryTotalList;
            set
            {
                SetProperty(ref _onlineBatteryTotalList, value);
            }
        }

        private ObservableCollection<BatteryTotalBase> _batteryTotalList;
        /// <summary>
        /// 设备集合
        /// </summary>
        public ObservableCollection<BatteryTotalBase> BatteryTotalList
        {
            get => _batteryTotalList;
            set
            {
                SetProperty(ref _batteryTotalList, value);
            }
        }

        public RelayCommand AddDevCommand { get; set; }
        public RelayCommand AddDevArrayCommand { get; set; }
        public RelayCommand DelAllDevCommand { get; set; }

        //public ConcurrentQueue<TotalBatteryInfoModel> TotalBatteryInfoQueue;
        public List<ModbusClient> ClientList;
        public int DaqTimeSpan = 1;
        public bool IsStartSaveData = false;
        public DisplayContentViewModel()
        {
            AddDevCommand = new RelayCommand(AddDev);
           
            DelAllDevCommand = new RelayCommand(DelAllDev);

            // 初始化设备列表
            BatteryTotalList = new ObservableCollection<BatteryTotalBase>();
            DevConnectInfoManage manage = new DevConnectInfoManage();
            var entites = manage.Get();
            if (entites != null)
            {
                foreach (var entity in entites)
                {
                    BatteryTotalList.Add(new BatteryTotalBase(entity.IP, entity.Port) { BCMUID = entity.BCMUID });
                }
            }
            OnlineBatteryTotalList = new ObservableCollection<BatteryTotalBase>();
            ClientList = new List<ModbusClient>();
        }

        private void DelAllDev()
        {
            BatteryTotalList.Clear();
            DevConnectInfoManage manage = new DevConnectInfoManage();
            manage.DeleteAll();
        }

       

        private void AddDev()
        {
            AddDevView view = new AddDevView();
            view.PCSRaB.IsEnabled = false;
            if (view.ShowDialog() == true)
            {
                //! 判断该IP是否存在
                var objs = BatteryTotalList.Where(dev => dev.IP == view.IPText.AddressText).ToList();
                if (objs.Count == 0)
                {
                    // add Modbus TCP Dev
                    BatteryTotalBase dev = new BatteryTotalBase(view.IPText.AddressText, view.TCPPort.Text);
                    dev.BCMUID = (BatteryTotalList.Count + 1).ToString();
                    BatteryTotalList.Add(dev);

                    //! 配置文件中新增IP
                    DevConnectInfoModel entity = new DevConnectInfoModel() { BCMUID = dev.BCMUID, IP = dev.IP, Port = dev.Port };
                    DevConnectInfoManage manage = new DevConnectInfoManage();
                    manage.Insert(entity);
                }
            }
        }

        public bool IsStartDaqData = false;
        /// <summary>
        /// 添加已连接设备
        /// </summary>
        /// <param name="battery">BCMU实例</param>
        /// <returns>是否添加成功</returns>
        public bool AddConnectedDev(BatteryTotalBase battery)
        {
            try
            {
                if (!OnlineBatteryTotalList.Contains(battery))
                {
                    if (int.TryParse(battery.Port, out int port))
                    {
                        ModbusClient client = new ModbusClient(battery.IP, port);
                        Connect(battery, client);
                        OnlineBatteryTotalList.Add(battery);
                        ClientList.Add(client);
                        if (IsStartDaqData)
                        {
                            OnlineBatteryTotalList[OnlineBatteryTotalList.Count-1].IsDaqData = true;
                            Thread thread = new Thread(ReadBatteryTotalData);
                            thread.IsBackground = true;
                            thread.Start(OnlineBatteryTotalList.Count - 1);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// 移除断开的设备
        /// </summary>
        /// <param name="item">断开的设备</param>
        /// <returns>移除设备的序号</returns>
        public int RemoveDisConnectedDev(BatteryTotalBase item)
        {
            try
            {
                if (OnlineBatteryTotalList.Count > 0)
                {
                    int index = OnlineBatteryTotalList.IndexOf(item);
                    Disconnect(index);
                    OnlineBatteryTotalList[index].IsDaqData = false;
                    OnlineBatteryTotalList.RemoveAt(index);
                    ClientList.RemoveAt(index);
                    return index;
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 指定设备请求入网
        /// </summary>
        /// <param name="item">指定设备</param>
        /// <returns>请求设备的序号</returns>
        public void RequestInterNet(BatteryTotalBase item)
        {
            try
            {
                if (item.RequestInterNet())
                {
                    item.IsInternet = true;
                }
                else
                {
                    item.IsInternet = false;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void Connect(BatteryTotalBase obj, ModbusClient client)
        {
            try
            {
                if (!obj.IsConnected)
                {
                    client.Connect();
                    obj.IsConnected = true;
                    InitBatteryTotalNew(obj, client);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect(int index)
        {
            if (OnlineBatteryTotalList[index].IsConnected)
            {
                ClientList[index].Disconnect();
                OnlineBatteryTotalList[index].IsConnected = false;
            }
        }

        /// <summary>
        /// 初始化电池总簇信息
        /// </summary>
        

        /// <summary>
        /// 新版初始化电池总簇信息
        /// </summary>
        public void InitBatteryTotalNew(BatteryTotalBase total, ModbusClient client)
        {
            if (total.IsConnected)
            {
                //** 注：应该尽可能的少次多量读取数据，多次读取数据会因为读取次数过于频繁导致丢包


                //byte[] BCMUData = new byte[90];
                //Array.Copy(client.AddReadRequest(11000, 45), 0, BCMUData, 0, 90);
                byte[] BCMUData = new byte[70];
                Array.Copy(client.AddReadRequest(11000, 35), 0, BCMUData, 0, 70);

                byte[] BMUIDData = new byte[48];
                Array.Copy(client.AddReadRequest(11045, 24), 0, BMUIDData, 0, 48);
                byte[] BMUData = new byte[744];
                Array.Copy(client.AddReadRequest(10000, 120), 0, BMUData, 0, 240);
                Array.Copy(client.AddReadRequest(10120, 120), 0, BMUData, 240, 240);
                Array.Copy(client.AddReadRequest(10240, 120), 0, BMUData, 480, 240);
                Array.Copy(client.AddReadRequest(10360, 12), 0, BMUData, 720, 24);
                

                // 信息补全
                total.TotalVoltage = BitConverter.ToUInt16(BCMUData, 0) * 0.1;
                total.TotalCurrent = BitConverter.ToInt16(BCMUData, 2) * 0.1;
                total.TotalSOC = BitConverter.ToUInt16(BCMUData, 4) * 0.1;
                total.TotalSOH = BitConverter.ToUInt16(BCMUData, 6) * 0.1;
                total.AverageTemperature = (BitConverter.ToInt16(BCMUData, 8)-2731) * 0.1;
                total.MinVoltage = BitConverter.ToInt16(BCMUData, 10) * 0.001;
                total.MaxVoltage = BitConverter.ToInt16(BCMUData, 12) * 0.001;
                total.MinVoltageIndex = BitConverter.ToUInt16(BCMUData, 14);
                total.MaxVoltageIndex = BitConverter.ToUInt16(BCMUData, 16);
                total.MinTemperature =( BitConverter.ToInt16(BCMUData, 18) -2731)* 0.1;
                total.MaxTemperature = (BitConverter.ToInt16(BCMUData, 20) - 2731) * 0.1;
                total.MinTemperatureIndex = BitConverter.ToUInt16(BCMUData, 22);
                total.MaxTemperatureIndex = BitConverter.ToUInt16(BCMUData, 24);
                //8.21通讯修改
                total.BatteryCycles = BitConverter.ToInt16(BCMUData, 26);
                total.HWVersionBCMU = BitConverter.ToInt16(BCMUData, 28);
                total.VersionSWBCMU = BitConverter.ToInt16(BCMUData, 34);
                total.BatteryCount = BitConverter.ToUInt16(BCMUData, 38);
                //total.SeriesCount = BitConverter.ToUInt16(BCMUData, 40);
                
                total.SeriesCount = 3;
                //total.BatteriesCountInSeries = BitConverter.ToUInt16(BCMUData, 42);
                total.BatteriesCountInSeries = 14;
                ///zyf
                ///
                total.StateBCMU = BitConverter.ToInt16(BCMUData, 48);
                total.IResistanceRP = BitConverter.ToInt16(BCMUData, 50);
                total.IResistanceRN = BitConverter.ToInt16(BCMUData, 52);
                total.DCVoltage = BitConverter.ToInt16(BCMUData, 54);
                total.VolContainerTemperature1 = (BitConverter.ToUInt16(BCMUData, 56)-2731) * 0.1;
                total.VolContainerTemperature2 = (BitConverter.ToUInt16(BCMUData, 58) - 2731) * 0.1;
                total.VolContainerTemperature3 = (BitConverter.ToUInt16(BCMUData, 60) - 2731) * 0.1;
                total.VolContainerTemperature4 = (BitConverter.ToUInt16(BCMUData, 62) - 2731) * 0.1;
                total.AlarmStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 64);
                total.ProtectStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 66);
                total.FaultyStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 68);
                //total.BatMaxChgPower = BitConverter.ToUInt16(BCMUData, 72) * 0.01;
                //total.BatMaxDischgPower = BitConverter.ToUInt16(BCMUData, 74) * 0.01;
                //total.OneChgCoulomb = BitConverter.ToUInt16(BCMUData, 76) * 0.01;
                //total.OneDischgCoulomb = BitConverter.ToUInt16(BCMUData, 78) * 0.01;
                //total.TotalChgCoulomb = BitConverter.ToUInt16(BCMUData, 80) * 0.01;
                //total.TotalDischgCoulomb = BitConverter.ToUInt16(BCMUData, 82) * 0.01;
                //total.RestCoulomb = BitConverter.ToUInt16(BCMUData, 84) * 0.01;
                //total.MaxVolDiff = BitConverter.ToUInt16(BCMUData, 86) * 0.01;
                //total.AvgVol = BitConverter.ToUInt16(BCMUData, 88) * 0.01;
                total.Series.Clear();


                //转化BCMU状态及Color标志
                
                bool AlarmColorFlagBCMU = GetActiveAlarmBCMU(total);
                bool FaultyColorFlagBCMU = GetActiveFaultyBCMU(total);
                bool ProtectColorFlagBCMU = GetActiveProtectBCMU(total);
              


                /// <summary>
                /// BMU信息
                /// </summary>

                for (int i = 0; i < total.SeriesCount; i++)
                {
                    BatterySeriesBase series = new BatterySeriesBase();
                    series.SeriesId = i.ToString();
                    series.AlarmStateFlagBMU = BitConverter.ToUInt16(BMUData, (336 + i)*2);
                    series.FaultyStateFlagBMU = BitConverter.ToUInt16(BMUData, (339 + i) * 2);
                    series.ChargeChannelState = BitConverter.ToUInt16(BMUData, (342 + i) * 2);
                    series.ChargeCapacitySum = BitConverter.ToUInt16(BMUData, (345 + i) * 2)*0.01;
                    series.MinVoltage = BitConverter.ToInt16(BMUData, (348 + i * 8) * 2) * 0.001;
                    series.MaxVoltage = BitConverter.ToInt16(BMUData, (349 + i * 8) * 2) * 0.001;
                    series.MinVoltageIndex = BitConverter.ToUInt16(BMUData, (350 + i * 8) * 2);
                    series.MaxVoltageIndex = BitConverter.ToUInt16(BMUData, (351 + i * 8) * 2);
                    series.MinTemperature = (BitConverter.ToInt16(BMUData, (352 + i * 8) * 2) - 2731) * 0.1;
                    series.MaxTemperature = (BitConverter.ToInt16(BMUData, (353 + i * 8) * 2)-2731) * 0.1;
                    series.MinTemperatureIndex = BitConverter.ToUInt16(BMUData, (354 + i * 8) * 2);
                    series.MaxTemperatureIndex = BitConverter.ToUInt16(BMUData, (355 + i * 8) * 2);
                    series.ChargeChannelStateNumber = GetSetBitPositions(series.ChargeChannelState).ToString();
                    bool FaultColorBMU = GetActiveFaultyBMU(series);
                    bool AlarmColorBMU = GetActiveAlarmBMU(series);
                                     
                    
                    //if (FaultColorBMU) { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                    //else { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                    //if (AlarmColorBMU) { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                    //else { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                    series.Batteries.Clear();

                    byte[] BMUIDArray = new byte[16];
                    Array.Copy(BMUIDData, 16 * i, BMUIDArray, 0, 16);
                    int ID1 = BitConverter.ToInt16(BMUIDArray, 0);
                    StringBuilder BMUNameBuilder = new StringBuilder();

                    for (int k = 0; k < 16; k++)
                    {
                        char BMUIDChar = Convert.ToChar(BMUIDArray[k]);
                        BMUNameBuilder.Append(BMUIDChar);
                    }
                    series.BMUID = BMUNameBuilder.ToString();

                    for (int j = 0; j < total.BatteriesCountInSeries; j++)
                    {
                        BatteryBase battery = new BatteryBase();
                        battery.Voltage = BitConverter.ToInt16(BMUData, (j + i * 16)*2) * 0.001;
                        battery.Temperature1 = (BitConverter.ToInt16(BMUData, (48 + j * 2 + i * 32) * 2) - 2731) * 0.1;
                        battery.Temperature2 = (BitConverter.ToInt16(BMUData, (48 + j * 2 + 1 + i * 32) * 2) - 2731) * 0.1;
                        battery.SOC = BitConverter.ToUInt16(BMUData, (144 + j + i * 16)*2) * 0.1;
                        battery.SOH = BitConverter.ToUInt16(BMUData,(192 + j + i * 16) * 2);
                        battery.Resistance = BitConverter.ToUInt16(BMUData, (240 + j + i * 16) * 2);
                        battery.Capacity = BitConverter.ToUInt16(BMUData, (288 + j + i * 16) * 2) * 0.1;
                        battery.VoltageColor = new SolidColorBrush(Colors.White);
                        battery.TemperatureColor = new SolidColorBrush(Colors.White);
                        battery.BatteryNumber = j;
                        series.Batteries.Add(battery);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            if (j + 1 == series.MinVoltageIndex)
                            {
                                battery.VoltageColor = new SolidColorBrush(Colors.LightBlue);
                            }

                            if (j + 1 == series.MaxVoltageIndex)
                            {
                                battery.VoltageColor = new SolidColorBrush(Colors.Red);
                            }

                            if (j + 1 == series.MinTemperatureIndex)
                            {
                                battery.TemperatureColor = new SolidColorBrush(Colors.LightBlue);
                            }

                            if (j + 1 == series.MaxTemperatureIndex)
                            {
                                battery.TemperatureColor = new SolidColorBrush(Colors.Red);
                            }
                            if (FaultColorBMU) { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                            else { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                            if (AlarmColorBMU) { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                            else { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }


                            if (FaultyColorFlagBCMU == true)
                            {
                                total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            }
                            else
                            {
                                total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }

                            if (ProtectColorFlagBCMU == true)
                            {
                                total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            }
                            else
                            {
                                total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }

                            if (AlarmColorFlagBCMU == true)
                            {
                                total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            }
                            else
                            {
                                total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }

                            if(total.StateBCMU == 1)
                            {
                                total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }
                            else if (total.StateBCMU == 2)
                            {
                                total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }
                            else if(total.StateBCMU == 3)
                            {
                                total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            }
                            else if(total.StateBCMU == 4)
                            {
                                total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                            }
                            
                            ///BCMU状态变化
                            

                            
                            //转化BCMU状态
                            //if (FaultyColorFlagBCMU == true)

                            //{
                            //    total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            //}
                            //else
                            //{
                            //    total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            //}

                            //if (ProtectColorFlagBCMU == true)
                            //{
                            //    total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            //}
                            //else
                            //{
                            //    total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            //}

                            //if (AlarmColorFlagBCMU == true)
                            //{
                            //    total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                            //}
                            //else
                            //{
                            //    total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                            //}

                            //if (FaultColorBMU) { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                            //else { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                            //if (AlarmColorBMU) { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                            //else { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                        });
                        //if (j + 1 == series.MinVoltageIndex)
                        //{
                        //    battery.VoltageColor = new SolidColorBrush(Colors.LightBlue);
                        //}

                        //if (j + 1 == series.MaxVoltageIndex)
                        //{
                        //    battery.VoltageColor = new SolidColorBrush(Colors.Red);
                        //}

                        //if (j + 1 == series.MinTemperatureIndex)
                        //{
                        //    battery.TemperatureColor = new SolidColorBrush(Colors.LightBlue);
                        //}

                        //if (j + 1 == series.MaxTemperatureIndex)
                        //{
                        //    battery.TemperatureColor = new SolidColorBrush(Colors.Red);
                        //}
                    }
                    total.Series.Add(series);
                }
            }
        }





        /// <summary>
        /// 转化BCMU警告状态及颜色flag
        /// </summary>
        /// <param name="total"></param>
        public bool GetActiveAlarmBCMU(BatteryTotalBase total)
        {
            int Value;
           ObservableCollection<string> INFO = new ObservableCollection<string>();
            Value = total.AlarmStateBCMUFlag;
            bool colorflag = false;

            if ((Value & 0x0001) != 0) { INFO.Add("高压箱高温"); colorflag = true; }       //bit0
            if ((Value & 0x0002) != 0) { INFO.Add("充电过流"); colorflag = true; }  //bit1
            if ((Value & 0x0004) != 0) { INFO.Add("放电过流"); colorflag = true; }  //bit2
            if ((Value & 0x0008) != 0) { INFO.Add("绝缘Rp异常"); colorflag = true; }  //bit3
            if ((Value & 0x0010) != 0) { INFO.Add("绝缘Rn异常"); colorflag = true; }  //bit4
            total.AlarmStateBCMU = INFO;

            return colorflag;
        }
        
        /// <summary>
        /// 转化BCMU保护状态及颜色flag
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        public bool GetActiveProtectBCMU(BatteryTotalBase total)
        {
            int Value;
            bool colorflag = false;
            ObservableCollection<string> INFO = new ObservableCollection<string>();
            Value = total.ProtectStateBCMUFlag;
            if ((Value & 0x0001) != 0) { INFO.Add("电池单体低压保护"); colorflag = true; }       //bit0
            if ((Value & 0x0002) != 0) { INFO.Add("电池单体高压保护"); colorflag = true; }  //bit1`
            if ((Value & 0x0004) != 0) { INFO.Add("电池组充电高压保护"); colorflag = true; }  //bit2
            if ((Value & 0x0008) != 0) { INFO.Add("充电低温保护"); colorflag = true; }  //bit3
            if ((Value & 0x0010) != 0) { INFO.Add("充电高温保护"); colorflag = true; }  //bit4
            if ((Value & 0x0020) != 0) { INFO.Add("放电低温保护"); colorflag = true; } //bit5
            if ((Value & 0x0040) != 0) { INFO.Add("放电高温保护"); colorflag = true; } //bit6
            if ((Value & 0x0080) != 0) { INFO.Add("电池组充电过流保护"); colorflag = true; } //bit7
            if ((Value & 0x0100) != 0) { INFO.Add("电池组放电过流保护"); colorflag = true; } //bit8
            if ((Value & 0x0200) != 0) { INFO.Add("电池模块欠压保护"); colorflag = true; } //bit9
            if ((Value & 0x0400) != 0) { INFO.Add("电池模块过压保护"); colorflag = true; } //bit10
            total.ProtectStateBCMU = INFO;
            return colorflag;
        }

        /// <summary>
        /// 转化BCMU故障状态及颜色flag
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        public bool GetActiveFaultyBCMU(BatteryTotalBase total)
        {
            int Value;
            ObservableCollection<string> INFO = new ObservableCollection<string>();
            Value = total.FaultyStateBCMUFlag;
            //total.FaultyStateBCMUColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")
            bool colorflag =false;
            if ((Value & 0x0001) != 0) {INFO.Add("主接触开关异常"); colorflag = true; } //bit0
            if ((Value & 0x0002) != 0) { INFO.Add("预放继电器异常");  colorflag = true; }  //bit1
            if ((Value & 0x0004) != 0) { INFO.Add("断路器继电器开关异常"); colorflag = true; }  //bit2
            if ((Value & 0x0008) != 0) { INFO.Add("CAN通讯异常"); colorflag = true; }  //bit3
            if ((Value & 0x0010) != 0) { INFO.Add("485硬件异常"); colorflag = true; }  //bit4
            if ((Value & 0x0020) != 0) { INFO.Add("以太网phy异常"); colorflag = true; } //bit5
            if ((Value & 0x0040) != 0) { INFO.Add("以太网通讯测试异常"); colorflag = true; } //bit6
            if ((Value & 0x0080) != 0) { INFO.Add("霍尔ADC I2C通讯异常"); colorflag = true; } //bit7
            if ((Value & 0x0100) != 0) { INFO.Add("霍尔电流检测异常"); colorflag = true; } //bit8
            if ((Value & 0x0200) != 0) { INFO.Add("分流器电流检测异常"); colorflag = true; } //bit9
            if ((Value & 0x0400) != 0) { INFO.Add("主接触开关异常"); colorflag = true; } //bit10
            if ((Value & 0x0800) != 0) { INFO.Add("环流预充开关异常"); colorflag = true; }//bit11
            if ((Value & 0x1000) != 0) { INFO.Add("断路器开关异常"); colorflag = true; } //bit12
            if ((Value & 0x2000) != 0) { INFO.Add("绝缘检测ADC I2C通讯异常"); colorflag = true; } //bit13
            if ((Value & 0x4000) != 0) { INFO.Add("高压DC电压检测ADC I2C通讯异常"); colorflag = true; } //bit 14

            total.FaultyStateBCMU = INFO;
            return colorflag;

        }

        /// <summary>
        /// 解析BMU故障
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public bool GetActiveFaultyBMU(BatterySeriesBase series)
        {
            int Value;
            ObservableCollection<string> INFO = new ObservableCollection<string>();
            Value = series.FaultyStateFlagBMU;
            
            bool colorflag = false;
            if ((Value & 0x0001) != 0) { INFO.Add("电压传感器故障"); colorflag = true; } //bit0
            if ((Value & 0x0002) != 0) { INFO.Add("温度传感器故障"); colorflag = true; }  //bit1
            if ((Value & 0x0004) != 0) { INFO.Add("内部通讯故障"); colorflag = true; }  //bit2
            if ((Value & 0x0008) != 0) { INFO.Add("输入过压故障"); colorflag = true; }  //bit3
            if ((Value & 0x0010) != 0) { INFO.Add("输入反接故障"); colorflag = true; }  //bit4
            if ((Value & 0x0020) != 0) { INFO.Add("继电器故障"); colorflag = true; } //bit5
            if ((Value & 0x0040) != 0) { INFO.Add("电池损坏故障"); colorflag = true; } //bit6
            if ((Value & 0x0080) != 0) { INFO.Add("关机电路异常"); colorflag = true; } //bit7
            if ((Value & 0x0100) != 0) { INFO.Add("BMIC异常"); colorflag = true; } //bit8
            if ((Value & 0x0200) != 0) { INFO.Add("内部总线异常"); colorflag = true; } //bit9
            if ((Value & 0x0400) != 0) { INFO.Add("开机自检异常"); colorflag = true; } //bit10
            //if ((Value & 0x0800) != 0) { INFO.Add("模块高压告警"); colorflag = true; }//bit11
            //if ((Value & 0x1000) != 0) { INFO.Add("断路器开关异常"); colorflag = true; } //bit12
            //if ((Value & 0x2000) != 0) { INFO.Add("绝缘检测ADC I2C通讯异常"); colorflag = true; } //bit13
            //if ((Value & 0x4000) != 0) { INFO.Add("高压DC电压检测ADC I2C通讯异常"); colorflag = true; } //bit 14
            series.FaultyStateBMU = INFO;
            return colorflag;
        }

        /// <summary>
        /// 解析BMU告警
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public bool GetActiveAlarmBMU(BatterySeriesBase series)
        {
            int Value;
            ObservableCollection<string> INFO = new ObservableCollection<string>();
            Value = series.AlarmStateFlagBMU;

            bool colorflag = false;
            if ((Value & 0x0001) != 0) { INFO.Add("单体低压告警"); colorflag = true; } //bit0
            if ((Value & 0x0002) != 0) { INFO.Add("单体高压告警"); colorflag = true; }  //bit1
            if ((Value & 0x0004) != 0) { INFO.Add("放电低压告警"); colorflag = true; }  //bit2
            if ((Value & 0x0008) != 0) { INFO.Add("充电高压告警"); colorflag = true; }  //bit3
            if ((Value & 0x0010) != 0) { INFO.Add("充电低温告警"); colorflag = true; }  //bit4
            if ((Value & 0x0020) != 0) { INFO.Add("充电高温告警"); colorflag = true; } //bit5
            if ((Value & 0x0040) != 0) { INFO.Add("放电低温告警"); colorflag = true; } //bit6
            if ((Value & 0x0080) != 0) { INFO.Add("放电高温告警"); colorflag = true; } //bit7
            if ((Value & 0x0100) != 0) { INFO.Add("充电过流告警"); colorflag = true; } //bit8
            if ((Value & 0x0200) != 0) { INFO.Add("放电过流告警"); colorflag = true; } //bit9
            if ((Value & 0x0400) != 0) { INFO.Add("模块低压告警"); colorflag = true; } //bit10
            if ((Value & 0x0800) != 0) { INFO.Add("模块高压告警"); colorflag = true; }//bit11
            //if ((Value & 0x1000) != 0) { INFO.Add("断路器开关异常"); colorflag = true; } //bit12
            //if ((Value & 0x2000) != 0) { INFO.Add("绝缘检测ADC I2C通讯异常"); colorflag = true; } //bit13
            //if ((Value & 0x4000) != 0) { INFO.Add("高压DC电压检测ADC I2C通讯异常"); colorflag = true; } //bit 14
            series.AlarmStateBMU = INFO;
            return colorflag;
        }



        /// <summary>
        /// 解析BCMU的均衡状态
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        int GetSetBitPositions(UInt16 num)
        {
            int[] setBits = new int[16];
            int bitPosition = 0;

            while (num > 0)
            {
                if ((num & 1) == 1)
                {
                    setBits[bitPosition] = 1;

                }

                num >>= 1;
                bitPosition++;
            }

            return bitPosition;
        }




        /// <summary>
        /// 展示实时数据
        /// </summary>
        public void DisplayRealTimeData()
        {
            IsStartDaqData = true;
            for (int i = 0; i < OnlineBatteryTotalList.Count; i++)
            {
                OnlineBatteryTotalList[i].IsDaqData = true;
                Thread thread = new Thread(ReadBatteryTotalData);
                thread.IsBackground = true;
                thread.Start(i);
            }
        }

        /// <summary>
        /// 停止展示实时数据
        /// </summary>
        public void StopDisplayRealTimeData()
        {
            IsStartDaqData = false;
            for (int i = 0; i < OnlineBatteryTotalList.Count; i++)
            {
                OnlineBatteryTotalList[i].IsDaqData = false;
            }
        }


        /// <summary>
        /// 读取BCMU数据
        /// </summary>
        /// <param name="index"></param>
        private void ReadBatteryTotalData(object index)
        {
            var total = OnlineBatteryTotalList[(int)index];
            var client = ClientList[(int)index];
            while (true)
            {
                try
                {
                    if (!total.IsDaqData)
                    {
                        break;
                    }

                    Thread.Sleep(DaqTimeSpan * 1000);
                    if (total.IsConnected)
                    {
                        //** 注：应该尽可能的少次多量读取数据，多次读取数据会因为读取次数过于频繁导致丢包
                        //byte[] BCMUData = new byte[118];
                        //Array.Copy(client.AddReadRequest(11000, 59), 0, BCMUData, 0, 118);
                        //byte[] BCMUData = new byte[70];
                        //Array.Copy(client.AddReadRequest(11000, 35), 0, BCMUData, 0, 70);
                        byte[] BCMUData = new byte[90];
                        Array.Copy(client.AddReadRequest(11000, 45), 0, BCMUData, 0, 90);
                        byte[] BMUIDData = new byte[48];
                        Array.Copy(client.AddReadRequest(11045, 24), 0, BMUIDData, 0, 48);
                        byte[] BMUData = new byte[744];
                        //Array.Copy(client.ReadFunc(10000, 120), 0, BMUData, 0, 240);
                        //Array.Copy(client.ReadFunc(10120, 120), 0, BMUData, 240, 240);
                        //Array.Copy(client.ReadFunc(10240, 120), 0, BMUData, 480, 240);
                        Array.Copy(client.AddReadRequest(10000, 120), 0, BMUData, 0, 240);//请求读取120个数据点，一个地址数据点两个字节。
                        Array.Copy(client.AddReadRequest(10120, 120), 0, BMUData, 240, 240);
                        Array.Copy(client.AddReadRequest(10240, 120), 0, BMUData, 480, 240);
                        Array.Copy(client.AddReadRequest(10360, 12), 0, BMUData, 720, 24);

                        // 信息补全
                        total.TotalVoltage = BitConverter.ToInt16(BCMUData, 0) * 0.1;
                        total.TotalCurrent = BitConverter.ToInt16(BCMUData, 2) * 0.1;
                        total.TotalSOC = BitConverter.ToUInt16(BCMUData, 4) * 0.1;
                        total.TotalSOH = BitConverter.ToUInt16(BCMUData, 6) * 0.1;
                        total.AverageTemperature = (BitConverter.ToInt16(BCMUData, 8) - 2731) * 0.1;
                        total.MinVoltage = BitConverter.ToInt16(BCMUData, 10) * 0.001;
                        total.MaxVoltage = BitConverter.ToInt16(BCMUData, 12) * 0.001;
                        total.MinVoltageIndex = BitConverter.ToUInt16(BCMUData, 14);
                        total.MaxVoltageIndex = BitConverter.ToUInt16(BCMUData, 16);
                        total.MinTemperature = (BitConverter.ToInt16(BCMUData, 18) - 2731) * 0.1;
                        total.MaxTemperature = (BitConverter.ToInt16(BCMUData, 20) - 2731) * 0.1;
                        total.MinTemperatureIndex = BitConverter.ToUInt16(BCMUData, 22);
                        total.MaxTemperatureIndex = BitConverter.ToUInt16(BCMUData, 24);
                        //8.21通讯修改
                        total.BatteryCycles = BitConverter.ToInt16(BCMUData, 26);
                        total.HWVersionBCMU = BitConverter.ToInt16(BCMUData, 28);
                        total.VersionSWBCMU = BitConverter.ToInt16(BCMUData, 34);
                        total.BatteryCount = BitConverter.ToUInt16(BCMUData, 38);
                        total.SeriesCount = BitConverter.ToUInt16(BCMUData, 40);
                        total.BatteriesCountInSeries = BitConverter.ToUInt16(BCMUData, 42);

                        ///zyf
                        ///
                        total.StateBCMU = BitConverter.ToInt16(BCMUData, 48);
                        total.IResistanceRP = BitConverter.ToInt16(BCMUData, 50);
                        total.IResistanceRN = BitConverter.ToInt16(BCMUData, 52);
                        total.DCVoltage = BitConverter.ToInt16(BCMUData, 54);
                        total.VolContainerTemperature1 = (BitConverter.ToUInt16(BCMUData, 56) - 2731) * 0.1;
                        total.VolContainerTemperature2 = (BitConverter.ToUInt16(BCMUData, 58) - 2731) * 0.1;
                        total.VolContainerTemperature3 = (BitConverter.ToUInt16(BCMUData, 60) - 2731) * 0.1;
                        total.VolContainerTemperature4 = (BitConverter.ToUInt16(BCMUData, 62) - 2731) * 0.1;
                        total.AlarmStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 64);
                        total.ProtectStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 66);
                        total.FaultyStateBCMUFlag = BitConverter.ToUInt16(BCMUData, 68);
                        total.BatMaxChgPower = BitConverter.ToUInt16(BCMUData, 72) * 0.01;
                        total.BatMaxDischgPower = BitConverter.ToUInt16(BCMUData, 74) * 0.01;
                        total.OneChgCoulomb = BitConverter.ToUInt16(BCMUData, 76) * 0.01;
                        total.OneDischgCoulomb = BitConverter.ToUInt16(BCMUData,78)*0.01;
                        total.TotalChgCoulomb = BitConverter.ToUInt16(BCMUData,80)*0.01;
                        total.TotalDischgCoulomb = BitConverter.ToUInt16(BCMUData,82)*0.01;
                        total.RestCoulomb = BitConverter.ToUInt16(BCMUData,84)*0.01;
                        total.MaxVolDiff = BitConverter.ToUInt16(BCMUData,86)*0.01;
                        total.AvgVol = BitConverter.ToUInt16(BCMUData,88)*0.01;
                        bool AlarmColorFlagBCMU = GetActiveAlarmBCMU(total);
                        bool FaultyColorFlagBCMU = GetActiveFaultyBCMU(total);
                        bool ProtectColorFlagBCMU = GetActiveProtectBCMU(total);
                        




                        for (int i = 0; i < total.Series.Count; i++)
                        {
                            BatterySeriesBase series = total.Series[i];
                            series.SeriesId = i.ToString();
                            series.AlarmStateFlagBMU = BitConverter.ToUInt16(BMUData, (336 + i) * 2);
                            series.FaultyStateFlagBMU = BitConverter.ToUInt16(BMUData, (339 + i) * 2);
                            series.ChargeChannelState = BitConverter.ToUInt16(BMUData, (342 + i) * 2);
                            series.ChargeCapacitySum = BitConverter.ToUInt16(BMUData, (345 + i) * 2) * 0.01;
                            series.MinVoltage = BitConverter.ToInt16(BMUData, (348 + i * 8) * 2) * 0.001;
                            series.MaxVoltage = BitConverter.ToInt16(BMUData, (349 + i * 8) * 2) * 0.001;
                            series.MinVoltageIndex = BitConverter.ToUInt16(BMUData, (350 + i * 8) * 2);
                            series.MaxVoltageIndex = BitConverter.ToUInt16(BMUData, (351 + i * 8) * 2);
                            series.MinTemperature = (BitConverter.ToInt16(BMUData, (352 + i * 8) * 2) - 2731) * 0.1;
                            series.MaxTemperature = (BitConverter.ToInt16(BMUData, (353 + i * 8) * 2) - 2731) * 0.1;
                            series.MinTemperatureIndex = BitConverter.ToUInt16(BMUData, (354 + i * 8) * 2);
                            series.MaxTemperatureIndex = BitConverter.ToUInt16(BMUData, (355 + i * 8) * 2);
                            series.ChargeChannelStateNumber = GetSetBitPositions(series.ChargeChannelState).ToString();
                            bool FaultColorBMU = GetActiveFaultyBMU(series);
                            bool AlarmColorBMU = GetActiveAlarmBMU(series);
                            byte[] BMUIDArray = new byte[16];
                            Array.Copy(BMUIDData, 16 * i, BMUIDArray, 0, 16);
                            int ID1 = BitConverter.ToInt16(BMUIDArray, 0);
                            StringBuilder BMUNameBuilder = new StringBuilder();

                            for (int k = 0; k < 16; k++)
                            {
                                char BMUIDChar = Convert.ToChar(BMUIDArray[k]);
                                BMUNameBuilder.Append(BMUIDChar);
                            }
                            series.BMUID = BMUNameBuilder.ToString();

                            for (int j = 0; j < series.Batteries.Count; j++)
                            {
                                BatteryBase battery = series.Batteries[j];
                                battery.Voltage = BitConverter.ToInt16(BMUData, (j + i * 16) * 2) * 0.001;
                                battery.Temperature1 = (BitConverter.ToInt16(BMUData, (48 + j * 2 + i * 32) * 2) - 2731) * 0.1;
                                battery.Temperature2 = (BitConverter.ToInt16(BMUData, (48 + j * 2 + 1 + i * 32) * 2) - 2731) * 0.1;
                                battery.SOC = BitConverter.ToUInt16(BMUData, (144 + j + i * 16) * 2) * 0.1;
                                battery.SOH = BitConverter.ToUInt16(BMUData, (192 + j + i * 16) * 2);
                                battery.Resistance = BitConverter.ToUInt16(BMUData, (240 + j + i * 16) * 2);
                                battery.Capacity = BitConverter.ToUInt16(BMUData, (288 + j + i * 16) * 2) * 0.1;
                                //battery.VoltageColor = new SolidColorBrush(Colors.White);
                                //battery.TemperatureColor = new SolidColorBrush(Colors.White);
                                battery.BatteryNumber = j;
                                // series.Batteries.Add(battery);

                                //if (j + 1 == series.MinVoltageIndex)
                                //{
                                //    battery.VoltageColor = new SolidColorBrush(Colors.LightBlue);
                                //}

                                //if (j + 1 == series.MaxVoltageIndex)
                                //{
                                //    battery.VoltageColor = new SolidColorBrush(Colors.Red);
                                //}

                                //if (j + 1 == series.MinTemperatureIndex)
                                //{
                                //    battery.TemperatureColor = new SolidColorBrush(Colors.LightBlue);
                                //}

                                //if (j + 1 == series.MaxTemperatureIndex)
                                //{
                                //    battery.TemperatureColor = new SolidColorBrush(Colors.Red);
                                //}


                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    if (j + 1 == series.MinVoltageIndex)
                                    {
                                        battery.VoltageColor = new SolidColorBrush(Colors.LightBlue);
                                    }

                                    if (j + 1 == series.MaxVoltageIndex)
                                    {
                                        battery.VoltageColor = new SolidColorBrush(Colors.Red);
                                    }

                                    if (j + 1 == series.MinTemperatureIndex)
                                    {
                                        battery.TemperatureColor = new SolidColorBrush(Colors.LightBlue);
                                    }

                                    if (j + 1 == series.MaxTemperatureIndex)
                                    {
                                        battery.TemperatureColor = new SolidColorBrush(Colors.Red);
                                    }
                                    if (FaultColorBMU) { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                                    else { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                                    if (AlarmColorBMU) { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                                    else { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }


                                    if (FaultyColorFlagBCMU == true)
                                    {
                                        total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    }
                                    else
                                    {
                                        total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }

                                    if (ProtectColorFlagBCMU == true)
                                    {
                                        total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    }
                                    else
                                    {
                                        total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }

                                    if (AlarmColorFlagBCMU == true)
                                    {
                                        total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    }
                                    else
                                    {
                                        total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }

                                    ///BCMU状态变化
                                    if (total.StateBCMU == 1)
                                    {
                                        total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                        total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }
                                    else if (total.StateBCMU == 2)
                                    {
                                        total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                        total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }
                                    else if (total.StateBCMU == 3)
                                    {
                                        total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                        total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    }
                                    else if (total.StateBCMU == 4)
                                    {
                                        total.ChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.DisChargeStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.StandStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                        total.OffNetStateBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FF33"));
                                    }
                                    //转化BCMU状态
                                    //if (FaultyColorFlagBCMU == true)

                                    //{
                                    //    total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    //}
                                    //else
                                    //{
                                    //    total.FaultyColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    //}

                                    //if (ProtectColorFlagBCMU == true)
                                    //{
                                    //    total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    //}
                                    //else
                                    //{
                                    //    total.ProtectColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    //}

                                    //if (AlarmColorFlagBCMU == true)
                                    //{
                                    //    total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000"));
                                    //}
                                    //else
                                    //{
                                    //    total.AlarmColorBCMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1"));
                                    //}

                                    //if (FaultColorBMU) { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                                    //else { series.FaultyColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                                    //if (AlarmColorBMU) { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE0000")); }
                                    //else { series.AlarmColorBMU = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1D1D1")); }

                                });

                                ///取BMU故障码



                            }
                            //total.Series.Add(series);
                        }

                        if (total.IsRecordData)
                        {
                            DateTime date = DateTime.Now;
                            TotalBatteryInfoModel TotalModel = new TotalBatteryInfoModel();
                            TotalModel.BCMUID = total.BCMUID;
                            TotalModel.Voltage = total.TotalVoltage;
                            TotalModel.Current = total.TotalCurrent;
                            TotalModel.SOC = total.TotalSOC;
                            TotalModel.SOH = total.TotalSOH;
                            TotalModel.AverageTemperature = total.AverageTemperature;
                            TotalModel.MinVoltage = total.MinVoltage;
                            TotalModel.MinVoltageIndex = total.MinVoltageIndex;
                            TotalModel.MaxVoltage = total.MaxVoltage;
                            TotalModel.MaxVoltageIndex = total.MaxVoltageIndex;
                            TotalModel.MinTemperature = total.MinTemperature;
                            TotalModel.MinTemperatureIndex = total.MinTemperatureIndex;
                            TotalModel.MaxTemperature = total.MaxTemperature;
                            TotalModel.MaxTemperatureIndex = total.MaxTemperatureIndex;
                            TotalModel.HappenTime = date;
                            BCMUDataList.Enqueue(TotalModel);

                            for (int i = 0; i < total.Series.Count; i++)
                            {
                                SeriesBatteryInfoModel model = new SeriesBatteryInfoModel();
                                model.BCMUID = total.BCMUID;
                                model.BMUID = total.Series[i].SeriesId;
                                model.MinVoltage = total.Series[i].MinVoltage;
                                model.MinVoltageIndex = total.Series[i].MinVoltageIndex;
                                model.MaxVoltage = total.Series[i].MaxVoltage;
                                model.MaxVoltageIndex = total.Series[i].MaxVoltageIndex;
                                model.MinTemperature = total.Series[i].MinTemperature;
                                model.MinTemperatureIndex = total.Series[i].MinTemperatureIndex;
                                model.MaxTemperature = total.Series[i].MaxTemperature;
                                model.MaxTemperatureIndex = total.Series[i].MaxTemperatureIndex;
                                model.AlarmState = total.Series[i].AlarmStateFlagBMU.ToString();
                                model.FaultState = total.Series[i].FaultyStateFlagBMU.ToString();
                                model.ChargeChannelState = total.Series[i].ChargeChannelState.ToString();
                                model.ChargeCapacitySum = total.Series[i].ChargeCapacitySum;
                                model.HappenTime = date;
                                for (int j = 0; j < total.Series[i].Batteries.Count; j++)
                                {
                                    typeof(SeriesBatteryInfoModel).GetProperty("Voltage" + j).SetValue(model, total.Series[i].Batteries[j].Voltage);
                                    typeof(SeriesBatteryInfoModel).GetProperty("Capacity" + j).SetValue(model, total.Series[i].Batteries[j].Capacity);
                                    typeof(SeriesBatteryInfoModel).GetProperty("SOC" + j).SetValue(model, total.Series[i].Batteries[j].SOC);
                                    typeof(SeriesBatteryInfoModel).GetProperty("Resistance" + j).SetValue(model, total.Series[i].Batteries[j].Resistance);
                                    typeof(SeriesBatteryInfoModel).GetProperty("Temperature" + (j * 2)).SetValue(model, total.Series[i].Batteries[j].Temperature1);
                                    typeof(SeriesBatteryInfoModel).GetProperty("Temperature" + (j * 2 + 1)).SetValue(model, total.Series[i].Batteries[j].Temperature2);
                                }
                                BMUDataList.Enqueue(model);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (total != null)
                        {
                            total.IsConnected = false;
                            total.IsDaqData = false;
                            total.IsRecordData = false;
                            OnlineBatteryTotalList.Remove(total);
                        }

                        if (client != null)
                        {
                            ClientList.Remove(client);
                        }
                    });
                    break;
                }
            }
        }

        /// <summary>
        /// Byte[]转String
        /// </summary>
        /// <param name="values">代转数据</param>
        /// <param name="startindex">偏移量</param>
        /// <param name="num">长度</param>
        /// <param name="value">结果数据</param>
        /// <returns>是否转换成功</returns>
        public bool Bytes_Str(byte[] values, int startindex, int num, out string value)
        {
            value = "";
            if (startindex >= 0 && num > 0)
            {
                if (startindex + num <= values.Length)
                {
                    value = BitConverter.ToString(values, startindex, num);
                    return true;
                }
            }
            return false;
        }

        private ConcurrentQueue<TotalBatteryInfoModel> BCMUDataList;
        private ConcurrentQueue<SeriesBatteryInfoModel> BMUDataList;
        internal void StartSaveData()
        {
            BCMUDataList = new ConcurrentQueue<TotalBatteryInfoModel>();
            BMUDataList = new ConcurrentQueue<SeriesBatteryInfoModel>();
            for (int i = 0; i < OnlineBatteryTotalList.Count; i++)
            {
                OnlineBatteryTotalList[i].IsRecordData = true;
            }

            Thread thread = new Thread(SaveBatteryData);
            thread.IsBackground = true;
            thread.Start();

            IsStartSaveData = true;
        }

        private void SaveBatteryData()
        {
            while(IsStartSaveData)
            {
                if (BCMUDataList.TryDequeue(out TotalBatteryInfoModel BCMUData))
                {
                    TotalBatteryInfoManage TotalManage = new TotalBatteryInfoManage();
                    TotalManage.Insert(BCMUData);
                }

                if(BMUDataList.TryDequeue(out SeriesBatteryInfoModel BMUData))
                {
                    SeriesBatteryInfoManage TotalManage = new SeriesBatteryInfoManage();
                    TotalManage.Insert(BMUData);
                }
            }
        }

        internal void StopSaveData()
        {
            for (int i = 0; i < OnlineBatteryTotalList.Count; i++)
            {
                OnlineBatteryTotalList[i].IsRecordData = false;
            }
            IsStartSaveData = false;
        }
    }
}
