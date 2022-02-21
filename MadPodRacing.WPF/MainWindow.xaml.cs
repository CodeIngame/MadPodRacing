using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MadPodRacing.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create a PathFigure to be used for the PathGeometry of myPath.
            PathFigure myPathFigure = new PathFigure();

            // Set the starting point for the PathFigure specifying that the
            // geometry starts at point 10,100.
            myPathFigure.StartPoint = new Point(10, 40) ;

            // Create a PointCollection that holds the Points used to specify 
            // the points of the PolyBezierSegment below.
            PointCollection myPointCollection = new PointCollection(4);
            myPointCollection.Add(new Point(10, 10));
            myPointCollection.Add(new Point(40, 10));
            myPointCollection.Add(new Point(40, 40));
            myPointCollection.Add(new Point(10, 40));

            //myPointCollection.Add(new Point(300, 0));


            // The PolyBezierSegment specifies two cubic Bezier curves.
            // The first curve is from 10,100 (start point specified by the PathFigure)
            // to 300,100 with a control point of 0,0 and another control point 
            // of 200,0. The second curve is from 300,100 (end of the last curve) to 
            // 600,100 with a control point of 300,0 and another control point of 400,0.
            PolyBezierSegment myBezierSegment = new PolyBezierSegment();
            myBezierSegment.Points = myPointCollection;

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myBezierSegment);

            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            // Create a path to draw a geometry with.
            Path myPath = new Path();
            myPath.Stroke = Brushes.Green;
            myPath.StrokeThickness = 1;

            var ellipses = new List<Ellipse>() { CreateEllipse(5,5 , myPathFigure.StartPoint.X, myPathFigure.StartPoint.Y) };
            myPointCollection.ToList().ForEach(p => ellipses.Add(CreateEllipse(5, 5, p.X, p.Y)));

            // specify the shape (quadratic Bezier curve) of the path using the StreamGeometry.
            myPath.Data = myPathGeometry;

            // Add path shape to the UI.
            StackPanel mainPanel = new StackPanel();
            mainPanel.Children.Add(myPath);
            ellipses.ForEach(e => mainPanel.Children.Add(e));
            this.Content = mainPanel;
        }

        Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            ellipse.Stroke = Brushes.Red;
            return ellipse;
        }
    }
}
