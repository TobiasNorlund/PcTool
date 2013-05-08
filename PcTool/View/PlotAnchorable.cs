using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Visifire.Charts;

namespace PcTool.View
{
    class PlotAnchorable : AvalonDock.Layout.LayoutAnchorable
    {
        public PlotAnchorable(string ID)
        {
            this.ID = ID;

            this.Title = ID;
            this.Content = chart;

            chart.AnimationEnabled = false;
            chart.LightingEnabled = false;
            chart.ScrollingEnabled = false;
            chart.AxesX.Add(new Axis() { Interval = 8 });

            serie = new DataSeries();
            serie.RenderAs = RenderAs.QuickLine;
                        
            chart.Series.Add(serie);
        }

        private string ID;
        private Chart chart = new Chart();
        private DataSeries serie;
        private int i = 0;

        public void UpdatePlot(ref Dictionary<string, int> data)
        {
            if (serie.DataPoints.Count > 100)
                serie.DataPoints.RemoveAt(0);
            try
            {
                serie.DataPoints.Add(new DataPoint() { YValue = data[ID], XValue = i });
                i++;
            }
            catch(Exception)
            {

            }

        }
    }
}
