using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Models
{
    public class GUlViewModel : PropertyChangedBase
    {
        private bool isStopButtonEnaled = false;
        public bool IsStopButtonEnaled
        {
            get { return isStopButtonEnaled; }
            set
            {
                isStopButtonEnaled = value;
                OnPropertyChanged("IsStopButtonEnaled");
            }
        }

        private bool isStartButtonEnaled = true;
        public bool IsStartButtonEnaled
        {
            get { return isStartButtonEnaled; }
            set
            {
                isStartButtonEnaled = value;
                OnPropertyChanged("IsStartButtonEnaled");
            }
        }

        private bool isPauseButtonEnaled = false;
        public bool IsPauseButtonEnaled
        {
            get { return isPauseButtonEnaled; }
            set
            {
                isPauseButtonEnaled = value;
                OnPropertyChanged("IsPauseButtonEnaled");
            }
        }

        private bool isSettingsButtonEnaled = true;
        public bool IsSettingsButtonEnaled
        {
            get { return isSettingsButtonEnaled; }
            set
            {
                isSettingsButtonEnaled = value;
                OnPropertyChanged("IsSettingsButtonEnaled");
            }
        }

        private string pauseButtonText = "Pause";
        public string PauseButtonText
        {
            get { return pauseButtonText; }
            set
            {
                pauseButtonText = value;
                OnPropertyChanged("PauseButtonText");
            }
        }
    }
}
