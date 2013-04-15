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
            // Sätt upp kommandon
            ConnectCommand = new RelayCommand(() => RobotConnector.Connect(), isCommandsDisabled);
            DisconnectCommand = new RelayCommand(() => RobotConnector.Disconnect(), isCommandsEnabled);
            SendManualCommand = new RelayCommand<string>((string c) => RobotConnector.SendCommand((ManualCommand)Enum.Parse(typeof(ManualCommand), c)), delegate(string s) { return RobotConnector.IsHandshaked; });
            UpdateControlParamCommand = new RelayCommand<KeyValuePair<ControlParam, byte>>(UpdateControlParamHandler, delegate(KeyValuePair<ControlParam, byte> o) { return RobotConnector.IsHandshaked; });

            // Skapa karthantere
            Map = new MapHandler();

            // Sätt upp Eventhanterare
            RobotConnector.ConnectionChanged += delegate()
            {
                RaisePropertyChanged("IsConnected");
                RaisePropertyChanged("IsHandshaked");
            };
            RobotConnector.MapUpdate += Map.UpdatePosition;
        }

        public MapHandler Map { get; set; }

        #region Bindable Properties

        public bool IsConnected
        {
            get { return RobotConnector.IsConnected; }
        }

        public bool IsHandshaked
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
        public RelayCommand DisconnectCommand { get; private set; }

        public RelayCommand<string> SendManualCommand { get; private set; }
        public RelayCommand<KeyValuePair<ControlParam, byte>> UpdateControlParamCommand { get; private set; }
            
        #endregion

        private bool isCommandsEnabled()
        {
            return RobotConnector.IsHandshaked;
        }

        private bool isCommandsDisabled()
        {
            return !RobotConnector.IsHandshaked;
        }

        #region Event Handlers

        private void UpdateControlParamHandler(KeyValuePair<ControlParam, byte> controlparam)
        {
            RobotConnector.UpdateControlParam(controlparam.Key, controlparam.Value);
        }

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
