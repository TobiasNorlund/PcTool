using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PcTool.Logic;
using GalaSoft.MvvmLight.Command;

namespace PcTool.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
            // Set up eventhandlers
            RobotConnector.ConnectionChanged += () => RaisePropertyChanged("IsConnected");

            // Set up commands
            ConnectCommand = new RelayCommand(() => RobotConnector.Connect());

            Map = new MapHandler();
        }

        public MapHandler Map { get; set; }

        #region Bindable Properties

        public bool IsConnected
        {
            get { return RobotConnector.IsConnected; }
        }

        private IDictionary<string, int> _DebugDataDictionary;
        public IDictionary<string, int> DebugDataDictionary { 
            get { return _DebugDataDictionary; }
            set
            {   _DebugDataDictionary = value;
                RaisePropertyChanged("DebugDataDictionary");
            }        
        }

        #endregion

        #region Commands

        public RelayCommand ConnectCommand { get; private set; }

        #endregion

        #region Event Handlers



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string PropName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
        #endregion

        #endregion              

    }
}
