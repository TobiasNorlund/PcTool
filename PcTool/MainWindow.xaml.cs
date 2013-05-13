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

            PcTool.Logic.RobotConnector.DebugDataUpdate += UpdatePlot;

            // Skriv ut vilka bytes som kommer
            PcTool.Logic.RobotConnector.newByte += temp2;

            //var t = new System.Timers.Timer(50);
            //t.Elapsed += delegate(object s, System.Timers.ElapsedEventArgs e)
            //             {
            //                 var r = new Random();
            //                 if (ViewModel.DebugDataDictionary.ContainsKey("test1"))
            //                     ViewModel.DebugDataDictionary["test1"] = r.Next(255);
            //                 //UpdatePlot(dict);
            //             };
            //t.Start();
        }

        private MainViewModel ViewModel;
        private Dictionary<string, View.PlotAnchorable> PlotViews = new Dictionary<string,View.PlotAnchorable>();

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            dockingManager.Theme = new AvalonDock.Themes.VS2010Theme();
        }

        //private Dictionary<string, int> dict = new Dictionary<string, int>();
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var r = new Random();
        //    dict.Add("test1", r.Next(255));
        //    dict.Add("test2", r.Next(255));
        //    dict.Add("test3", r.Next(255));
        //    dict.Add("test4", r.Next(255));
        //    dict.Add("test5", r.Next(255));
        //    dict.Add("test6", r.Next(255));
        //    dict.Add("test7", r.Next(255));
        //    dict.Add("test8", r.Next(255));
        //    dict.Add("test9", r.Next(255));
        //    //ViewModel.DebugDataDictionary = dict;

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

        private void PlotDebugData(object sender, EventArgs e)
        {
            string ID = ((Button)sender).Tag.ToString();

            if (PlotViews.ContainsKey(ID))
                return;

            var plot = new View.PlotAnchorable(ID);
            
            PlotViews.Add(ID, plot);

            PlotContainer.Children.Add(plot);
        }

        private void UpdatePlot(Dictionary<string, int> data)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(() => UpdatePlot(data)), null);
                return;
            }

            foreach (string ID in PlotViews.Keys)
            {
                PlotViews[ID].UpdatePlot(ref data);
            }
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

        private void ControlParam6_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void mapView_ClearMap(object sender, EventArgs e)
        {
            ViewModel.ClearMapCommand.Execute(null);
        }

    }
}
