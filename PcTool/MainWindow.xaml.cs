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
            Terminal.RegisteredCommands.Add("update");

            ViewModel = ((MainViewModel)App.Current.Resources["ViewModel"]);
            ViewModel.Map.PositionUpdated += mapView.PositionUpdated;
            ViewModel.Map.PositionUpdated += temp;
            ViewModel.Map.WallDetected += mapView.WallDetected;
            ViewModel.Map.Cleared += () => mapView.Clear();

            // Skriv ut vilka bytes som kommer
            PcTool.Logic.RobotConnector.newByte += temp2;
        }

        private MainViewModel ViewModel;

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            dockingManager.Theme = new AvalonDock.Themes.VS2010Theme();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
            
        //    var dict = new Dictionary<string, int>();
        //    dict.Add("test1", 123);
        //    dict.Add("test2", 234);
        //    ViewModel.DebugDataDictionary = dict;
            
        //}

        private void Terminal_CommandEntered(object sender, AurelienRibon.Ui.Terminal.Terminal.CommandEventArgs e)
        {
            switch(e.Command.Name){
                case "map":
                    ViewModel.Map.UpdatePosition(int.Parse(e.Command.Args[0]), int.Parse(e.Command.Args[1]), bool.Parse(e.Command.Args[2]));
                    break;
                case "update":
                    ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>((PcTool.Logic.ControlParam)Enum.Parse(typeof(PcTool.Logic.ControlParam), e.Command.Args[0]), Byte.Parse(e.Command.Args[1])));
                    break;
                default:
                    break;
            }
            
            ((AurelienRibon.Ui.Terminal.Terminal)sender).InsertNewPrompt();
        }

        private void temp(int x, int y, bool isFree){
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(() => temp(x, y, isFree)), null);
                return;
            }

            Terminal.Text += (x+8) + " - " + (y+8) + ": " + isFree + Environment.NewLine;
        }

        private void temp2(byte b){
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(() => temp2(b)), null);
                return;
            }

            Terminal.Text += b + Environment.NewLine;
        }

        private void UpdateControlParam1(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.L1_x, Byte.Parse(ControlParam1.Text)));
        }
        private void UpdateControlParam2(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.L2_theta, Byte.Parse(ControlParam2.Text)));
        }
        private void UpdateControlParam3(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.L3_omega, Byte.Parse(ControlParam3.Text)));
        }
        private void UpdateControlParam4(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.L1_theta, Byte.Parse(ControlParam4.Text)));
        }
        private void UpdateControlParam5(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.L2_omega, Byte.Parse(ControlParam5.Text)));
        }
        private void UpdateControlParam6(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.PowerRightPair, Byte.Parse(ControlParam6.Text)));
        }
        private void UpdateControlParam7(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UpdateControlParamCommand.Execute(new KeyValuePair<PcTool.Logic.ControlParam, byte>(PcTool.Logic.ControlParam.PowerLeftPair, Byte.Parse(ControlParam7.Text)));
        }
    }
}
