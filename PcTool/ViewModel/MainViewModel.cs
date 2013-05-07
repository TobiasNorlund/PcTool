using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PcTool.Logic;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace PcTool.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
            // Sätt upp kommandon
            ConnectCommand = new RelayCommand(() => RobotConnector.Connect(), delegate() { return !RobotConnector.IsConnected; });
            DisconnectCommand = new RelayCommand(() => RobotConnector.Disconnect(), delegate() { return RobotConnector.IsConnected; });
            EmergencyStopCommand = new RelayCommand(() => RobotConnector.SendEmergencyStop(), isCommandsEnabled);
            SendManualCommand = new RelayCommand<string>((string c) => RobotConnector.SendCommand((ManualCommand)Enum.Parse(typeof(ManualCommand), c)), delegate(string s) { return RobotConnector.IsHandshaked; });
            UpdateControlParamCommand = new RelayCommand<KeyValuePair<ControlParam, byte>>(UpdateControlParamHandler, delegate(KeyValuePair<ControlParam, byte> o) { return RobotConnector.IsHandshaked; });

            // Skapa karthantere
            Map = new MapHandler();

            // Sätt upp Eventhanterare
            RobotConnector.ConnectionChanged += onConnectionChanged;
            RobotConnector.MapUpdate += Map.UpdatePosition;
            RobotConnector.DebugDataUpdate += onDebugData;

        }

        public MapHandler Map { get; set; }

        #region Bindable Properties

        /// <summary>
        /// Returnerar om en anslutning till Firefly:en finns
        /// </summary>
        public bool IsConnected
        {
            get { return RobotConnector.IsConnected; }
        }

        /// <summary>
        /// Returnerar om en lyckad handskakning har genomförts
        /// </summary>
        public bool IsHandshaked
        {
            get { return RobotConnector.IsHandshaked; }
        }

        private ObservableDictionary<string, int> _DebugDataDictionary;
        public ObservableDictionary<string, int> DebugDataDictionary
        { 
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

        public RelayCommand EmergencyStopCommand { get; private set; }
        public RelayCommand<string> SendManualCommand { get; private set; }
        public RelayCommand<KeyValuePair<ControlParam, byte>> UpdateControlParamCommand { get; private set; }
            
        #endregion

        private void onConnectionChanged()
        {
            RaisePropertyChanged("IsConnected");
            RaisePropertyChanged("IsHandshaked");

            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            EmergencyStopCommand.RaiseCanExecuteChanged();
            SendManualCommand.RaiseCanExecuteChanged();
            UpdateControlParamCommand.RaiseCanExecuteChanged();

            if (IsHandshaked)
            {
                onHandshaked();
            }
            else if(!IsConnected)
            {
                onDisconnected();
            }
        }

        private void onHandshaked()
        {
            // Sätt första rutan som fri
            Map.UpdatePosition(8, 8, true);

            // Cleara debugdata
            DebugDataDictionary = new ObservableDictionary<string, int>();
        }

        private void onDisconnected()
        {
            // Cleara hela kartan
            Map.Clear();
        }

        private void onDebugData(Dictionary<string, int> data)
        {
            if (!App.Current.Dispatcher.CheckAccess())
            {
                App.Current.Dispatcher.Invoke(new Action(() => onDebugData(data)), null);
                return;
            }

            foreach (string d in data.Keys)
            {
                if (_DebugDataDictionary.Keys.Contains(d))
                    _DebugDataDictionary[d] = data[d];
                else
                    _DebugDataDictionary.Add(d, data[d]);
            }
            //RaisePropertyChanged("DebugDataDictionary");
            //DebugDataDictionary = new ObservableDictionary<string, int>(DebugDataDictionary);
        }

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
