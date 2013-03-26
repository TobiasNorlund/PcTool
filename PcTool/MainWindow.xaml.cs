using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PcTool.ViewModel;

namespace PcTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            dockingManager.Theme = new AvalonDock.Themes.VS2010Theme();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ViewModel = ((MainViewModel)App.Current.Resources["ViewModel"]);
            var dict = new Dictionary<string, int>();
            dict.Add("test1", 123);
            dict.Add("test2", 234);
            ViewModel.DebugDataDictionary = dict;
            
        }


    }
}
