using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EMS.Model
{
    /// <summary>
    /// 电池串
    /// </summary>
    public class BatterySeriesBase : ObservableObject
    {
        /// <summary>
        /// BMUID
        /// </summary>
        /// 
        private string _bMUID;
        public string BMUID
        {
            get => _bMUID;
            set
            {
                SetProperty(ref _bMUID, value);
            }
        }
        //private int _bMUID1;
        //public int BMUID1
        //{
        //    get => _bMUID1;
        //    set
        //    {
        //        SetProperty(ref _bMUID1, value);
        //    }
        //}

        //private int _bMUID2;
        //public int BMUID2
        //{
        //    get => _bMUID2;
        //    set
        //    {
        //        SetProperty(ref _bMUID2, value);
        //    }
        //}

        //private int _bMUID3;
        //public int BMUID3
        //{
        //    get => _bMUID3;
        //    set
        //    {
        //        SetProperty(ref _bMUID3, value);
        //    }
        //}

        //private int _bMUID4;
        //public int BMUID4
        //{
        //    get => _bMUID4;
        //    set
        //    {
        //        SetProperty(ref _bMUID4, value);
        //    }
        //}

        //private int _bMUID5;
        //public int BMUID5
        //{
        //    get => _bMUID5;
        //    set
        //    {
        //        SetProperty(ref _bMUID5, value);
        //    }
        //}

        //private int _bMUID6;
        //public int BMUID6
        //{
        //    get => _bMUID6;
        //    set
        //    {
        //        SetProperty(ref _bMUID6, value);
        //    }
        //}

        //private int _bMUID7;
        //public int BMUID7
        //{
        //    get => _bMUID7;
        //    set
        //    {
        //        SetProperty(ref _bMUID7, value);
        //    }
        //}

        //private int _bMUID8;
        //public int BMUID8
        //{
        //    get => _bMUID8;
        //    set
        //    {
        //        SetProperty(ref _bMUID8, value);
        //    }
        //}
        /// <summary>
        /// BMU告警
        /// </summary>
        private int _alarmStateFlagBMU;
        public int AlarmStateFlagBMU
        {
            get => _alarmStateFlagBMU;
            set
            {
                SetProperty(ref _alarmStateFlagBMU, value);
            }
        }

        private ObservableCollection<string> _alarmState;
        public ObservableCollection<string> AlarmStateBMU
        {

            get => _alarmState;
            set
            {
                SetProperty(ref _alarmState, value);
            }
        }
        private SolidColorBrush _alarmColorBMU;
        public SolidColorBrush AlarmColorBMU
        {
            get => _alarmColorBMU;
            set
            {
                SetProperty(ref _alarmColorBMU, value);
            }
        }


        /// <summary>
        /// BMU故障状态
        /// </summary>
        private int  _faultyStateFlagBMU;
        
        public int FaultyStateFlagBMU
        {

            get => _faultyStateFlagBMU;
            set
            {
                SetProperty(ref _faultyStateFlagBMU, value);
            }
        }

        private ObservableCollection<string> _faultyStateBMU;
        public ObservableCollection<string> FaultyStateBMU
        {
            get => _faultyStateBMU;
            set
            {
                SetProperty(ref _faultyStateBMU, value);
            }
        }
        private SolidColorBrush _faultyColorBMU;
        public SolidColorBrush FaultyColorBMU
        {
            get => _faultyColorBMU;
            set
            {
                SetProperty(ref _faultyColorBMU, value);
            }
        }


        private string _chargeChannelStateNumber;
        public string ChargeChannelStateNumber
        {
            get => _chargeChannelStateNumber;
            set
            {
                SetProperty(ref _chargeChannelStateNumber, value);
            }
        }


        private ushort _chargeChannelState;
        /// <summary>
        /// 充电通道状态
        /// </summary>
        public ushort ChargeChannelState
        {

            get => _chargeChannelState;
            set
            {
                SetProperty(ref _chargeChannelState, value);
            }
        }

        private double _chargeCapacitySum;
        /// <summary>
        /// 充电累计容量
        /// </summary>
        public double ChargeCapacitySum
        {

            get => _chargeCapacitySum;
            set
            {
                SetProperty(ref _chargeCapacitySum, value);
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

        /// <summary>
        /// 电池串id
        /// </summary>
        public string SeriesId { get; set; }

        /// <summary>
        /// 电池单体集合
        /// </summary>
        public ObservableCollection<BatteryBase> Batteries { get; set;}

        public BatterySeriesBase()
        {
            Batteries = new ObservableCollection<BatteryBase>();
        }
    }
}
