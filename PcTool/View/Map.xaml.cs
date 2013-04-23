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
using System.Windows.Media.Media3D;

namespace PcTool.View
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();

            initViewMap();
        }

        private MaterialGroup availableBlockMaterial = new MaterialGroup();
        private MaterialGroup unavailableBlockMaterial = new MaterialGroup();
        private Model3DGroup viewMap = new Model3DGroup();

        private void initViewMap()
        {
            availableBlockMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(200, 200, 200))));
            availableBlockMaterial.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 100));

            unavailableBlockMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            unavailableBlockMaterial.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 100));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = viewMap;
            this.mainViewport.Children.Add(model);
        }

        public void PositionUpdated(int x, int y, bool isFree)
        {
            if (!viewMap.Dispatcher.CheckAccess())
            {
                viewMap.Dispatcher.Invoke(new Action(() => PositionUpdated(x,y,isFree)), null);
                return;
            }

            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(x, y, 0));
            mesh.Positions.Add(new Point3D(x + 1, y, 0));
            mesh.Positions.Add(new Point3D(x + 1, y + 1, 0));
            mesh.Positions.Add(new Point3D(x, y + 1, 0));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            viewMap.Children.Add(new GeometryModel3D(mesh, (isFree)?availableBlockMaterial:unavailableBlockMaterial));
        }

        public void WallDetected(int x, int y, bool isX)
        {
            if (!viewMap.Dispatcher.CheckAccess())
            {
                viewMap.Dispatcher.Invoke(new Action(() => WallDetected(x, y, isX)), null);
                return;
            }

            var z = 0.5;

            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(x, y, 0));
            if (isX)
            {
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Positions.Add(new Point3D(x + 1, y, 0));
                mesh.Positions.Add(new Point3D(x + 1, y, z));
            }
            else
            {
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Positions.Add(new Point3D(x, y + 1, 0));
                mesh.Positions.Add(new Point3D(x, y + 1, z));
            }

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);

            var model = new GeometryModel3D(mesh, availableBlockMaterial);
            model.BackMaterial = availableBlockMaterial;

            viewMap.Children.Add(model);
        }

        /// <summary>
        /// Clearar hela kartan
        /// </summary>
        public void Clear()
        {
            viewMap.Children.Clear();
        }
    }
}
