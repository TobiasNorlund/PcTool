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
            Terminal.RegisteredCommands.Add("map");

            ViewModel = ((MainViewModel)App.Current.Resources["ViewModel"]);
            ViewModel.Map.PositionUpdated += mapView.PositionUpdated;
            ViewModel.Map.WallDetected += mapView.WallDetected;
        }

        private MainViewModel ViewModel;

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            dockingManager.Theme = new AvalonDock.Themes.VS2010Theme();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var dict = new Dictionary<string, int>();
            dict.Add("test1", 123);
            dict.Add("test2", 234);
            ViewModel.DebugDataDictionary = dict;
            
        }

        private void Terminal_CommandEntered(object sender, AurelienRibon.Ui.Terminal.Terminal.CommandEventArgs e)
        {
            switch(e.Command.Name){
                case "map":
                    ViewModel.Map.UpdatePosition(int.Parse(e.Command.Args[0]), int.Parse(e.Command.Args[1]), bool.Parse(e.Command.Args[2]));
                    break;
                case "write":
                    PcTool.Logic.RobotConnector.SendByte(byte.Parse(e.Command.Args[0]));
                    break;
                default:
                    //((AurelienRibon.Ui.Terminal.Terminal)sender).InsertLineBeforePrompt("Invalid");
                    break;
            }
            
            ((AurelienRibon.Ui.Terminal.Terminal)sender).InsertNewPrompt();
        }


    }
}
