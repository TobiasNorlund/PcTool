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

        //private MeshGeometry3D[,] viewMapBricks;
        private MaterialGroup availableBlockMaterial = new MaterialGroup();
        private MaterialGroup unavailableBlockMaterial = new MaterialGroup();
        private Model3DGroup viewMap = new Model3DGroup();

        private void initViewMap()
        {
            //viewMapBricks = new MeshGeometry3D[16, 16];

            //for (int y = -8; y < 8; y++)
            //{
            //    for (int x = -8; x < 8; x++)
            //    {
            //        var mesh = new MeshGeometry3D();
            //        mesh.Positions.Add(new Point3D(x, y, 0));
            //        mesh.Positions.Add(new Point3D(x + 1, y, 0));
            //        mesh.Positions.Add(new Point3D(x + 1, y + 1, 0));
            //        mesh.Positions.Add(new Point3D(x, y + 1, 0));

            //        mesh.TriangleIndices.Add(0);
            //        mesh.TriangleIndices.Add(1);
            //        mesh.TriangleIndices.Add(2);

            //        mesh.TriangleIndices.Add(0);
            //        mesh.TriangleIndices.Add(2);
            //        mesh.TriangleIndices.Add(3);

            //        viewMap.Children.Add(new GeometryModel3D(mesh, group));
            //    }
            //}
            availableBlockMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(200, 200, 200))));
            availableBlockMaterial.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 100));

            unavailableBlockMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            unavailableBlockMaterial.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 100));

            ModelVisual3D model = new ModelVisual3D();
            model.Content = viewMap;
            this.mainViewport.Children.Add(model);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var materialGroup = new MaterialGroup();
            //materialGroup.Children.Add(availableBlockMaterial);
            //materialGroup.Children.Add(emissive);
            //viewMap.Children.Add(new GeometryModel3D(viewMapBricks[rand.Next(15), rand.Next(15)], materialGroup));
            /*MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Point3D point0 = new Point3D(0, 0, 0);
            Point3D point1 = new Point3D(5, 0, 0);
            Point3D point2 = new Point3D(0, 0, 5);
            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);
            triangleMesh.Positions.Add(new Point3D(1,5,10));
            triangleMesh.TriangleIndices.Add(0);
            triangleMesh.TriangleIndices.Add(2);
            triangleMesh.TriangleIndices.Add(1);
            triangleMesh.TriangleIndices.Add(3);
            Vector3D normal = new Vector3D(0, 1, 0);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(
                new SolidColorBrush(Colors.DarkKhaki));
            GeometryModel3D triangleModel = new GeometryModel3D(
                triangleMesh, material);
            ModelVisual3D model = new ModelVisual3D();
            model.Content = triangleModel;
            this.mainViewport.Children.Add(model);*/
        }

        public void PositionUpdated(int x, int y, bool isFree)
        {
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
    }
}
