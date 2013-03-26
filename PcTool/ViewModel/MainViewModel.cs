using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PcTool.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
            
        }

        private IDictionary<string, int> _DebugDataDictionary;
        public IDictionary<string, int> DebugDataDictionary { 
            get { return _DebugDataDictionary; }
            set
            {   _DebugDataDictionary = value;
                RaisePropertyChanged("DebugDataDictionary");
            }        
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string PropName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }

    }
}
