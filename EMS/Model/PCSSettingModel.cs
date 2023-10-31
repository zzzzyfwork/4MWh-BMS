using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Model
{
    public class PCSSettingModel : ObservableObject
    {
        private string _strategyName;
        public string StrategyName
        {
            get => _strategyName;
            set
            {
                SetProperty(ref _strategyName, value);
            }
        }

        private string _strategyMode;
        public string StrategyMode
        {
            get => _strategyMode;
            set
            {
                SetProperty(ref _strategyMode, value);
            }
        }

        private string _strategyValue;
        public string StrategyValue
        {
            get => _strategyValue;
            set
            {
                SetProperty(ref _strategyValue, value);
            }
        }

        private string _strategyStartTime;
        public string StrategyStartTime
        {
            get => _strategyStartTime;
            set
            {
                SetProperty(ref _strategyStartTime, value);
            }
        }

        private string _strategyEndTime;
        public string StrategyEndTime
        {
            get => _strategyEndTime;
            set
            {
                SetProperty(ref _strategyEndTime, value);
            }
        }

        
    }
}
