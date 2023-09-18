using CommunityToolkit.Mvvm.Input;
using EMS.Common.Modbus.ModbusTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.ViewModel
{
    public  class ParameterSettingViewModel: ViewModelBase
    {
        /// <summary>
        /// 单体过压保护阈值
        /// </summary>
        private double _singleOverVolThresh;
        public double SingleOverVolThresh
        {
            get => _singleOverVolThresh;
            set
            {
                SetProperty(ref  _singleOverVolThresh, value);
            }
        }

        /// <summary>
        /// 单体过压保护恢复阈值
        /// </summary>
        private double _singleRecoveryOverVolThresh;
        public double SingleRecoveryOverVolThresh
        {
            get => _singleRecoveryOverVolThresh;
            set
            {
                SetProperty(ref _singleRecoveryOverVolThresh, value);
            }
        }

        /// <summary>
        /// 单体低压保护阈值
        /// </summary>
        private double _singleLowVolThresh;
        public double SingleLowVolThresh
        {
            get => _singleLowVolThresh;
            set
            {
                SetProperty(ref _singleLowVolThresh, value);
            }

        }

        /// <summary>
        /// 单体低压保护恢复阈值
        /// </summary>
        private double _singleRecoveryLowVolThresh;
        public double SingleRecoveryLowVolThresh
        {
            get => _singleRecoveryLowVolThresh; 
            set
            {
                SetProperty(ref _singleRecoveryLowVolThresh, value);
            }
        }

        /// <summary>
        /// 单体高温保护阈值
        /// </summary>
        private double _singleHighTempThresh;
        public double SingleHighTempThresh
        {
            get => _singleHighTempThresh;
            set
            {
                SetProperty(ref _singleHighTempThresh, value);
            }
        }
        
        /// <summary>
        /// 单体高温保护恢复阈值
        /// </summary>
        private double _singleRecoveryHighTempThresh;
        public double SingleRecoveryHighTempThresh
        {
            get => _singleRecoveryHighTempThresh; 
            set
            {
                SetProperty(ref _singleRecoveryHighTempThresh, value);
            }
        }

        /// <summary>
        /// 单体低温保护阈值
        /// </summary>
        private double _singleLowTempThresh;
        public double SingleLowTempThresh
        {
            get => _singleLowTempThresh;
            set
            {
                SetProperty(ref _singleLowTempThresh, value);
            }
        }

        /// <summary>
        /// 单体低温保护恢复阈值
        /// </summary>
        private double _singleRecoveryLowTempThresh;
        public double SingleRecoveryLowTempThresh
        {
            get => _singleRecoveryLowTempThresh;
            set
            {
                SetProperty(ref _singleRecoveryLowTempThresh, value);
            }
        }
        /// <summary>
        /// 簇充电过流保护阈值
        /// </summary>
        private double _clusterCharOverCurrentThresh;
        public double ClusterCharOverCurrentThresh
        {
            get => _clusterCharOverCurrentThresh;
            set
            {
                SetProperty(ref _clusterCharOverCurrentThresh, value);
            }
        }
        /// <summary>
        /// 簇充电过流保护恢复阈值
        /// </summary>
        private double _clusterRecoveryCharOverCurrentThresh;
        public double ClusterRecoveryCharOverCurrentThresh
        {
            get => _clusterRecoveryCharOverCurrentThresh;
            set
            {
                SetProperty(ref _clusterRecoveryCharOverCurrentThresh, value);
            }
        }
        /// <summary>
        /// 簇放电过流保护阈值
        /// </summary>
        private double _clusterDisCharOverCurrentThresh;
        public double ClusterDisCharOverCurrentThresh
        {
            get => _clusterDisCharOverCurrentThresh;
            set
            {
                SetProperty(ref _clusterDisCharOverCurrentThresh, value);
            }
        }
        /// <summary>
        /// 簇放电过流保护恢复阈值
        /// </summary>
        private double _clusterRecoveryDisCharOverCurrentThresh;
        public double ClusterRecoveryDisCharOverCurrentThresh
        {
            get => _clusterRecoveryDisCharOverCurrentThresh;
            set
            {
                SetProperty(ref _clusterRecoveryDisCharOverCurrentThresh, value);
            }
        }


        private int _iralarmThresh;
        public int IralarmThresh
        {
            get => _iralarmThresh;
            set
            {
                SetProperty(ref _iralarmThresh, value);
            }
        }

        private int _hboxtempalarmThresh;
        public int HboxtempalarmThresh
        {
            get => _hboxtempalarmThresh;
            set
            {
                SetProperty(ref _hboxtempalarmThresh, value);
            }
        }

        private double _lowsocalarm1;
        public double Lowsocalarm1
        {
            get => _lowsocalarm1;
            set
            {
                SetProperty(ref _lowsocalarm1 , value);
            }
        }

        private double _lowsocalarm2;
        public double Lowsocalarm2
        {
            get => _lowsocalarm2;
            set
            {
                SetProperty(ref _lowsocalarm2, value);
            }
        }









        public RelayCommand ReadClusterThreshInfoCommand { get; set; }
        public RelayCommand SyncClusterThreshInfofCommand { get; set; }
        public RelayCommand ReadSingleVolThreshInfoCommand { get; set; }
        public RelayCommand SyncSingleVolThreshInofCommand { get; set; }
        public RelayCommand ReadSingleTempThreshInfoCommand { get; set; }
        public RelayCommand SyncSingleTempThreshInfoCommand { get; set; }

        public RelayCommand ReadOtherThreshInfoCommand { get; set; }
        public RelayCommand SyncOtherThreshInfoCommand { get;set; }
        private ModbusClient ModbusClient;
        public ParameterSettingViewModel(ModbusClient client) 
        {
            ModbusClient = client;
            ReadClusterThreshInfoCommand = new RelayCommand(ReadClusterThreshInfo);
            SyncClusterThreshInfofCommand = new RelayCommand(SyncClusterThreshInfo);
            ReadSingleVolThreshInfoCommand = new RelayCommand(ReadSingleVolThreshInfo);
            SyncSingleVolThreshInofCommand = new RelayCommand(SyncSingleVolThreshInof);
            ReadSingleTempThreshInfoCommand=new RelayCommand(ReadSingleTempThreshInfo);
            SyncSingleTempThreshInfoCommand=new RelayCommand(SyncSingleTempThreshInfo);
            ReadOtherThreshInfoCommand = new RelayCommand(ReadOtherThreshInfo);
            SyncOtherThreshInfoCommand = new RelayCommand(SyncOtherThreshInfo);
        }

        private void SyncSingleTempThreshInfo()
        {
            ModbusClient.WriteFunc(40208, (ushort)(SingleHighTempThresh+2731));
            ModbusClient.WriteFunc(40209, (ushort)(SingleRecoveryHighTempThresh +2731));
            ModbusClient.WriteFunc(40210, (ushort)(SingleLowTempThresh +2731));
            ModbusClient.WriteFunc(40211, (ushort)(SingleRecoveryLowTempThresh +2731));

        }

        private void ReadSingleTempThreshInfo()
        {
            byte[] data = ModbusClient.ReadFunc(40208, 4);
            SingleHighTempThresh = BitConverter.ToUInt16(data, 0) -2731;
            SingleRecoveryHighTempThresh = BitConverter.ToUInt16(data, 2) -2731;
            SingleLowTempThresh = BitConverter.ToUInt16(data, 4) -2731;
            SingleRecoveryLowTempThresh = BitConverter.ToUInt16(data, 6) -2731;

        }

        private void SyncSingleVolThreshInof()
        {
            ModbusClient.WriteFunc(1,40200, (ushort)(SingleOverVolThresh * 1000));
            ModbusClient.WriteFunc(1,40201, (ushort)(SingleRecoveryOverVolThresh * 1000));
            ModbusClient.WriteFunc(1, 40202, (ushort)(SingleLowVolThresh * 1000));
            ModbusClient.WriteFunc(1, 40203, (ushort)(SingleRecoveryLowVolThresh * 1000));
        }

        private void ReadSingleVolThreshInfo()
        {
            byte[] data = ModbusClient.ReadFunc(40200, 4);
            SingleOverVolThresh = BitConverter.ToUInt16(data, 0) * 0.001;
            SingleRecoveryOverVolThresh =BitConverter.ToUInt16(data, 2)*0.001;
            SingleLowVolThresh =BitConverter.ToUInt16(data,4 )*0.001;
            SingleRecoveryLowVolThresh =BitConverter.ToUInt16(data, 6)*0.001;
        }

        private void SyncClusterThreshInfo()
        {
            ModbusClient.WriteFunc(40204, (ushort)(ClusterCharOverCurrentThresh));
            ModbusClient.WriteFunc(40205,(ushort)(ClusterRecoveryCharOverCurrentThresh));
            ModbusClient.WriteFunc(40206,(ushort)(ClusterDisCharOverCurrentThresh));
            ModbusClient.WriteFunc(40207,(ushort)(ClusterRecoveryDisCharOverCurrentThresh));
           
        }

        private void ReadClusterThreshInfo()
        {
            byte[] data = ModbusClient.ReadFunc(40204, 4);
            ClusterCharOverCurrentThresh = BitConverter.ToUInt16(data, 0) ;
            ClusterRecoveryCharOverCurrentThresh = BitConverter.ToUInt16(data, 2);
            ClusterDisCharOverCurrentThresh = BitConverter.ToUInt16(data, 4);
            ClusterRecoveryDisCharOverCurrentThresh = BitConverter.ToUInt16(data, 6);
        }

        private void ReadOtherThreshInfo()
        {
            byte[] data = ModbusClient.ReadFunc(40212, 4);
            IralarmThresh = BitConverter.ToUInt16(data,0);
            HboxtempalarmThresh = BitConverter.ToUInt16(data,2)-2731;
            Lowsocalarm1 = BitConverter.ToUInt16(data,4)*0.1;
            Lowsocalarm2 =  BitConverter.ToUInt16(data,6)*0.1;
        }

    private void SyncOtherThreshInfo()
        {
            ModbusClient.WriteFunc(40212, (ushort)IralarmThresh);
            ModbusClient.WriteFunc(40213, (ushort)(HboxtempalarmThresh+2731));
            ModbusClient.WriteFunc(40214,(ushort)(Lowsocalarm1*10) );
            ModbusClient.WriteFunc(40215, (ushort)(Lowsocalarm2*10));

        }
    }
}
