﻿using CommunityToolkit.Mvvm.ComponentModel;
using EMS.Common.Modbus.ModbusTCP;
using EMS.Storage.DB.DBManage;
using EMS.Storage.DB.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EMS.Model
{
    /// <summary>
    /// 电池簇
    /// </summary>
    public class BatteryTotalBase : ObservableObject
    {
        private double _totalVoltage;
        /// <summary>
        /// 总簇电压    0.1V 
        /// </summary>
        public double TotalVoltage
        {

            get => _totalVoltage;
            set
            {
                SetProperty(ref _totalVoltage, value);
            }
        }

        private double _totalCurrent;
        /// <summary>
        /// 总簇电流    0.1A
        /// </summary>
        public double TotalCurrent
        {

            get => _totalCurrent;
            set
            {
                SetProperty(ref _totalCurrent, value);
            }
        }

        private double _totalSOC;
        /// <summary>
        /// 总簇SOC   0.1%
        /// </summary>
        public double TotalSOC
        {

            get => _totalSOC;
            set
            {
                SetProperty(ref _totalSOC, value);
            }
        }

        private double _totalSOH;
        /// <summary>
        /// 总簇SOH   0.1%
        /// </summary>
        public double TotalSOH
        {

            get => _totalSOH;
            set
            {
                SetProperty(ref _totalSOH, value);
            }
        }

        private double _averageTemperature;
        /// <summary>
        /// 平均温度    0.1℃
        /// </summary>
        public double AverageTemperature
        {

            get => _averageTemperature;
            set
            {
                SetProperty(ref _averageTemperature, value);
            }
        }

        private double _minVoltage;
        /// <summary>
        /// 单体最低电压
        /// </summary>
        public double MinVoltage
        {

            get => _minVoltage;
            set
            {
                SetProperty(ref _minVoltage, value);
            }
        }

        private int _minVoltageIndex;
        /// <summary>
        /// 单体最低电压编号
        /// </summary>
        public int MinVoltageIndex
        {

            get => _minVoltageIndex;
            set
            {
                SetProperty(ref _minVoltageIndex, value);
            }
        }

        private double _maxVoltage;
        /// <summary>
        /// 单体最高电压
        /// </summary>
        public double MaxVoltage
        {

            get => _maxVoltage;
            set
            {
                SetProperty(ref _maxVoltage, value);
            }
        }

        private int _maxVoltageIndex;
        /// <summary>
        /// 单体最高电压编号
        /// </summary>
        public int MaxVoltageIndex
        {

            get => _maxVoltageIndex;
            set
            {
                SetProperty(ref _maxVoltageIndex, value);
            }
        }

        private double _minTemperature;
        /// <summary>
        /// 单体最低温度
        /// </summary>
        public double MinTemperature
        {

            get => _minTemperature;
            set
            {
                SetProperty(ref _minTemperature, value);
            }
        }

        private int _minTemperatureIndex;
        /// <summary>
        /// 单体最低温度编号
        /// </summary>
        public int MinTemperatureIndex
        {

            get => _minTemperatureIndex;
            set
            {
                SetProperty(ref _minTemperatureIndex, value);
            }
        }

        private double _maxTemperature;
        /// <summary>
        /// 单体最高温度
        /// </summary>
        public double MaxTemperature
        {

            get => _maxTemperature;
            set
            {
                SetProperty(ref _maxTemperature, value);
            }
        }

        private int _maxTemperatureIndex;
        /// <summary>
        /// 单体最高温度编号
        /// </summary>
        public int MaxTemperatureIndex
        {

            get => _maxTemperatureIndex;
            set
            {
                SetProperty(ref _maxTemperatureIndex, value);
            }
        }


        //zyf
        /// <summary>
        /// 高压箱温度
        /// </summary>
        private double _volContainerTemperature1;
        public double VolContainerTemperature1
        {
            get => _volContainerTemperature1;
            set
            {
                SetProperty( ref _volContainerTemperature1, value);
            }
        }

        private double _volContainerTemperature2;
        public double VolContainerTemperature2
        {
            get => _volContainerTemperature2;
            set
            {
                SetProperty(ref _volContainerTemperature2 , value);
            }
        }

        private double _volContainerTemperature3;
        public double VolContainerTemperature3
        {
            get => _volContainerTemperature3;
            set
            {
                SetProperty (ref _volContainerTemperature3 , value);
            }
        }
        
        private double _volContainerTemperature4;
        public double VolContainerTemperature4
        {
            get => _volContainerTemperature4;
            set
            {
                SetProperty(ref _volContainerTemperature4 , value);
            }
        }


        
        /// <summary>
        /// BCMU软件版本号
        /// </summary>
        private int _versionSWBCMU;
        public int VersionSWBCMU
        {
            get => _versionSWBCMU;
            set
            {
                SetProperty(ref _versionSWBCMU, value);
            }
        }

        /// <summary>
        /// 绝缘电阻
        /// </summary>
        private int _iResistanceRP;
        public int IResistanceRP
        {
            get => _iResistanceRP;
            set
            {
                SetProperty(ref _iResistanceRP, value);
            }
        }

        private int _iResistanceRN;
        public int IResistanceRN
        {
            get => _iResistanceRN;
            set
            {
                SetProperty(ref _iResistanceRN, value);
            }
        }

        /// <summary>
        /// BCMU告警
        /// </summary>
        /// 
        private int _alarmStateBCMUFlag;
        public int AlarmStateBCMUFlag
        {
            get => _alarmStateBCMUFlag;
            set
            {

                SetProperty(ref _alarmStateBCMUFlag, value);
            }
        }

        private ObservableCollection<string> _alarmStateBCMU;
        public ObservableCollection<string> AlarmStateBCMU
        {
            get => _alarmStateBCMU;
            set
            {
                SetProperty(ref _alarmStateBCMU, value);
            }
        }




        //private string _alarmColorFlag;
        //public string AlarmColorFlag
        //{
        //    get => _alarmColorFlag;
        //    set
        //    {
        //        SetProperty(ref _alarmColorFlag, value);
        //    }
        //}

        private SolidColorBrush _alarmColorBCMU;
        public SolidColorBrush AlarmColorBCMU
        {
            get => _alarmColorBCMU;
            set
            {
                SetProperty(ref _alarmColorBCMU, value);
            }
        }





        /// <summary>
        /// BCMU保护
        /// </summary>
        /// 
        private int _protectStateBCMUFlag;
        public int ProtectStateBCMUFlag
        {
            get => _protectStateBCMUFlag;
            set
            {

                SetProperty(ref _protectStateBCMUFlag, value);
            }
        }

        private ObservableCollection<string> _protectStateBCMU;
        public ObservableCollection<string> ProtectStateBCMU
        {
            get => _protectStateBCMU;
            set
            {
                SetProperty(ref _protectStateBCMU, value);
            }
        }

        /// <summary>
        /// 保护颜色变化
        /// </summary>
        //private string _protectColorFlag;
        //public string ProtectColorFlag
        //{
        //    get => _protectColorFlag;
        //    set
        //    {
        //        SetProperty(ref _protectColorFlag, value);
        //    }
        //}

        private SolidColorBrush _protectColorBCMU;
        public SolidColorBrush ProtectColorBCMU
        {
            get => _protectColorBCMU;
            set
            {
                SetProperty(ref _protectColorBCMU, value);
            }
        }

        /// <summary>
        /// BCMU故障
        /// </summary>
        /// 
        private int _faultytStateBCMUFlag;
        public int FaultyStateBCMUFlag
        {
            get => _faultytStateBCMUFlag;
            set
            {

                SetProperty(ref _faultytStateBCMUFlag, value);
            }
        }

        private ObservableCollection<string> _faultyStateBCMU;
        public ObservableCollection<string> FaultyStateBCMU
        {
            get => _faultyStateBCMU;
            set
            {
                SetProperty(ref _faultyStateBCMU, value);
            }
        }
        
        /// <summary>
        /// 故障颜色变化
        /// </summary>
        //private string _faultyColorFlag;
        //public string FaultyColorFlag
        //{
        //    get => _faultyColorFlag;
        //    set
        //    {
        //        SetProperty(ref  _faultyColorFlag, value);
        //    }
        //}

        private SolidColorBrush _faultyColorBCMU;
        public SolidColorBrush FaultyColorBCMU
        {
            get => _faultyColorBCMU;
            set
            {
                SetProperty(ref _faultyColorBCMU, value);
            }
        }

        /// <summary>
        /// BCMU状态颜色 充电、静置、放电、离网
        /// </summary>
        /// 
        private int _stateBCMU;
        public int StateBCMU
        {
            get => _stateBCMU;
            set
            {
                SetProperty(ref _stateBCMU, value);
            }
        }
        private SolidColorBrush _chargeStateBCMU;
        public SolidColorBrush ChargeStateBCMU
        {
            get => _chargeStateBCMU;
            set
            {
                SetProperty(ref _chargeStateBCMU, value);
            }
        }

        private SolidColorBrush _disChargeStateBCMU;
        public SolidColorBrush DisChargeStateBCMU
        {
            get => _disChargeStateBCMU;
            set
            {
                SetProperty(ref _disChargeStateBCMU, value);
            }
        }

        private SolidColorBrush _standStateBCMU;
        public SolidColorBrush StandStateBCMU
        {
            get => _standStateBCMU;
            set
            {
                SetProperty(ref _standStateBCMU, value);
            }
        }

        private SolidColorBrush _offNetSateBCMU;
        public SolidColorBrush OffNetStateBCMU
        {
            get => _offNetSateBCMU;
            set
            {
                SetProperty(ref _offNetSateBCMU, value);
            }
        }




        /// <summary>
        /// 循环次数
        /// </summary>
        private int _batteryCycles;
        public int BatteryCycles
        {
            get => _batteryCycles;
            set
            {
                SetProperty( ref _batteryCycles, value);
            }
        }

        /// <summary>
        /// BCMU硬件版本
        /// </summary>
        private int _hwVersionBCMU;
        public int HWVersionBCMU
        {
            get => _hwVersionBCMU;
            set
            {
                SetProperty(ref _hwVersionBCMU, value);
            }
        }

        /// <summary>
        /// DC母线电压
        /// </summary>
        private int _dcVoltage;
        public int DCVoltage
        {
            get => _dcVoltage;
            set
            {
                SetProperty(ref  _dcVoltage, value);
            }
        }
        /// <summary>
        /// 最大充电功率
        /// </summary>
        private double _batMaxChgPower;
        public double BatMaxChgPower
        {
            get => _batMaxChgPower;
            set
            {
                SetProperty(ref _batMaxChgPower , value);
            }
        }
        /// <summary>
        /// 最大放电攻略
        /// </summary>
        private double _batMaxDischgPower;
        public double BatMaxDischgPower
        {
            get => _batMaxDischgPower;
            set
            {
                SetProperty(ref _batMaxDischgPower, value);
            }
        }

        /// <summary>
        /// 单次充电量
        /// </summary>
        private double _oneChgCoulomb;
        public double OneChgCoulomb
        {
            get => _oneChgCoulomb;
            set
            {
                SetProperty(ref _oneChgCoulomb, value);
            }
        }
        /// <summary>
        /// 单次放电量
        /// </summary>
        private double _oneDischgCoulomb;
        public double OneDischgCoulomb
        {
            get => _oneDischgCoulomb;
            set
            {
                SetProperty(ref _oneDischgCoulomb, value);
            }
        }
        /// <summary>
        /// 累计充电量
        /// </summary>
        private double _totalChgCoulomb;
        public double TotalChgCoulomb
        {
            get => _totalChgCoulomb;
            set
            {
                SetProperty(ref _totalChgCoulomb , value);
            }
        }
        /// <summary>
        /// 累计放电量
        /// </summary>
        private double _totalDischgCoulomb;
        public double TotalDischgCoulomb
        {
            get => _totalDischgCoulomb;
            set
            {
                SetProperty(ref _totalDischgCoulomb, value);
            }
        }
        /// <summary>
        /// 剩余电量
        /// </summary>
        private double _restCoulomb;
        public double RestCoulomb
        {
            get =>_restCoulomb;
            set
            {
                SetProperty (ref _restCoulomb, value);
            }
        }

        /// <summary>
        /// 最大压差
        /// </summary>
        private double _maxVolDiff;
        public double MaxVolDiff
        {
            get => _maxVolDiff;
            set
            {
                SetProperty(ref _maxVolDiff , value);
            }
        }

        private double _avgVol;
        public double AvgVol
        {
            get => _avgVol;
            set
            {
                SetProperty(ref _avgVol , value);
            }
        }





        private BitmapSource _devImage;
        /// <summary>
        /// 标签图标
        /// </summary>
        public BitmapSource DevImage
        {

            get => _devImage;
            set
            {
                SetProperty(ref _devImage, value);
            }
        }

        private BitmapSource _connectImage;
        /// <summary>
        /// 连接图标
        /// </summary>
        public BitmapSource ConnectImage
        {

            get => _connectImage;
            set
            {
                SetProperty(ref _connectImage, value);
            }
        }

        private BitmapSource _internetImage;
        /// <summary>
        /// 连接图标
        /// </summary>
        public BitmapSource InternetImage
        {

            get => _internetImage;
            set
            {
                SetProperty(ref _internetImage, value);
            }
        }

        private BitmapSource _daqDataImage;
        /// <summary>
        /// 采集图标
        /// </summary>
        public BitmapSource DaqDataImage
        {

            get => _daqDataImage;
            set
            {
                SetProperty(ref _daqDataImage, value);
            }
        }

        private BitmapSource _recordDataImage;
        /// <summary>
        /// 记录图标
        /// </summary>
        public BitmapSource RecordDataImage
        {

            get => _recordDataImage;
            set
            {
                SetProperty(ref _recordDataImage, value);
            }
        }

        private string _BCMUID;
        /// <summary>
        /// BCMU的序列号
        /// </summary>
        public string BCMUID
        {

            get => _BCMUID;
            set
            {
                if(SetProperty(ref _BCMUID, value))
                {
                    _totalID = "BCMU(" + value + ")";
                }
            }
        }

        private string _totalID;
        /// <summary>
        /// 电池簇名称
        /// </summary>
        public string TotalID
        {

            get => _totalID;
            set
            {
                SetProperty(ref _totalID, value);
            }
        }

        public string IP { set; get; }
        public string Port { set; get; }

        /// <summary>
        /// 电池数量
        /// </summary>
        private ushort _batteryCount;
        public ushort BatteryCount
        {
            get => _batteryCount;
            set
            {
                SetProperty(ref _batteryCount, value);
            }
        }

        /// <summary>
        /// 电池串数量
        /// </summary>
        public ushort SeriesCount { get; set; }

        /// <summary>
        /// 电池串中电池数量
        /// </summary>
        public ushort BatteriesCountInSeries { get; set; }

        /// <summary>
        /// 电池串集合
        /// </summary>
        public ObservableCollection<BatterySeriesBase> Series { get; set; }

        private bool _isConnected;
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectImageChange(value);
                }
            }
        }

        private bool _isInternet;
        /// <summary>
        /// 是否已入网
        /// </summary>
        public bool IsInternet
        {
            get { return _isInternet; }
            set
            {
                if (_isInternet != value)
                {
                    _isInternet = value;
                    InternetImageChange(value);
                }
            }
        }

        private bool _isDaqData;
        /// <summary>
        /// 是否已采集数据
        /// </summary>
        public bool IsDaqData
        {
            get { return _isDaqData; }
            set
            {
                if (_isDaqData != value)
                {
                    _isDaqData = value;
                    DaqImageChange(value);
                }
            }
        }

        private bool _isRecordData;
        /// <summary>
        /// 是否已记录数据
        /// </summary>
        public bool IsRecordData
        {
            get { return _isRecordData; } 
            set
            {
                if (_isRecordData != value)
                {
                    _isRecordData = value;
                    RecordImageChange(value);
                }
            }
        }

        /// <summary>
        /// 生成电池总簇实例
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        public BatteryTotalBase(string ip, string port)
        {
            Series = new ObservableCollection<BatterySeriesBase>();
            IP = ip;
            Port = port;
            ImageTitle();
            ConnectImageChange(false);
            InternetImageChange(false);
            DaqImageChange(false);
            RecordImageChange(false);
        }

        /// <summary>
        /// 标签图标
        /// </summary>
        public void ImageTitle()
        {
            DevImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/BMS.png"));
        }

        public void ConnectImageChange(bool isconnected)
        {
            if (isconnected)
            {
                ConnectImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OnConnect.png"));
            }
            else
            {
                ConnectImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OffConnect.png"));
            }
        }
        
        /// <summary>
        /// 入网图标改变
        /// </summary>
        /// <param name="isinternet">是否已入网</param>
        public void InternetImageChange(bool isinternet)
        {
            if (isinternet)
            {
                InternetImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/InNet.png"));
            }
            else
            {
                InternetImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OutNet.png"));
            }
        }

        /// <summary>
        /// 采集图标改变
        /// </summary>
        /// <param name="isdaq">是否已采集</param>
        public void DaqImageChange(bool isdaq)
        {
            if (isdaq)
            {
                DaqDataImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OnDaq.png"));
            }
            else
            {
                DaqDataImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OffDaq.png"));
            }
        }

        /// <summary>
        /// 记录图标改变
        /// </summary>
        /// <param name="isrecord">是否已记录</param>
        public void RecordImageChange(bool isrecord)
        {
            if (isrecord)
            {
                RecordDataImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OnRecord.png"));
            }
            else
            {
                RecordDataImage = new BitmapImage(new Uri("pack://application:,,,/Resource/Image/OffRecord.png"));
            }
        }

        



        /// <summary>
        /// 请求入网
        /// </summary>
        /// <returns>是否入网成功</returns>
        public bool RequestInterNet()
        {
            Console.WriteLine("设备请求入网");
            return true;
        }
    }
}
