using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace EllipseTestProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow ActiveWindow { get; private set; }

        public Ellipse CurrentEllipse { get; private set; }

        public MainWindow()
        {
            if (ActiveWindow != null)
            {
                throw new NotSupportedException("No more than one window is supported!");
            }
            ActiveWindow = this;

            InitializeComponent();

            EllipseImportParameters eip = new EllipseImportParameters() { RadiusX = 500, RadiusY = 100, StartAngleDeg = 0, DeltaAngleDeg = 270 };
            CurrentEllipse = new Ellipse(eip);

        }

        private void On_NewEllipse_Clicked(object sender, RoutedEventArgs e)
        {
            NewEllipseDialog ned = new NewEllipseDialog();

            ned.ShowDialog();

            if (ned.OkClicked)
            {
                CurrentEllipse = new Ellipse(ned.ImportParameters);
                ellipseCanvas.InvalidateVisual();
            }
        }

        private void On_ButtonApplyScale_Clicked(object sender, RoutedEventArgs e)
        {
            if (CurrentEllipse == null)
            {
                return;
            }

            float cx = CurrentEllipse.VectorList[1];
            float cy = CurrentEllipse.VectorList[2];

            
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
            m.Translate(cx, cy);
            m.Scale((float) spinnerScaleX.Value, (float) spinnerScaleY.Value);
            m.Translate(-cx, -cy);
            

            /*
            float scaleX = (float)spinnerScaleX.Value;
            float scaleY = (float)spinnerScaleY.Value;
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(scaleX,0,scaleY,0, -cx * scaleX + cx, -cy * scaleY + cy);
            */

            CurrentEllipse.MultiplyWithMatrix(m);
            ellipseCanvas.InvalidateVisual();

        }

        private void On_ButtonApplyRotation_Clicked(object sender, RoutedEventArgs e)
        {
            if (CurrentEllipse == null)
            {
                return;
            }

            float cx = CurrentEllipse.VectorList[1];
            float cy = CurrentEllipse.VectorList[2];

            
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
            m.Translate(-cx, -cy);
            m.Rotate( -(float) spinnerRotationAngle.Value ); //Counter clock wise rotation
            m.Translate(cx, cy);
            

            /*
            float a = (float) spinnerRotationAngle.Value;
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix( (float) Math.Cos(a), (float) -Math.Sin(a), (float) Math.Sin(a), (float) Math.Cos(a), (float) ( -cx * Math.Cos(a) + cy * Math.Sin(a) + cx ), (float) (-cx * Math.Sin(a) - cy * Math.Cos(a) + cy) );
            */

            CurrentEllipse.MultiplyWithMatrix(m);
            ellipseCanvas.InvalidateVisual();
        }

        private void On_ButtonMirrorX_Clicked(object sender, RoutedEventArgs e)
        {
            if (CurrentEllipse == null)
            {
                return;
            }

            float cx = CurrentEllipse.VectorList[1];
            float cy = CurrentEllipse.VectorList[2];

            System.Drawing.Drawing2D.Matrix total = new System.Drawing.Drawing2D.Matrix();
            total.Translate(cx, cy);
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 0);
            total.Multiply(m);
            total.Translate(-cx, -cy);

            CurrentEllipse.MultiplyWithMatrix(total);
            ellipseCanvas.InvalidateVisual();
        }

        private void On_ButtonMirrorY_Clicked(object sender, RoutedEventArgs e)
        {
            if (CurrentEllipse == null)
            {
                return;
            }

            float cx = CurrentEllipse.VectorList[1];
            float cy = CurrentEllipse.VectorList[2];

            System.Drawing.Drawing2D.Matrix total = new System.Drawing.Drawing2D.Matrix();
            total.Translate(cx, cy);
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(-1,0,0,1,0,0);
            total.Multiply(m);
            total.Translate(-cx, -cy);

            CurrentEllipse.MultiplyWithMatrix(total);
            ellipseCanvas.InvalidateVisual();
        }

        private void On_ButtonApplyTranslation_Clicked(object sender, RoutedEventArgs e)
        {
            if (CurrentEllipse == null)
            {
                return;
            }

            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, (float) spinnerTranslateX.Value, (float) spinnerTranslateY.Value);
            CurrentEllipse.MultiplyWithMatrix(m);

            ellipseCanvas.InvalidateVisual();
        }

        private void On_MenuItemAbout_Clicked(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(System.Windows.Forms.Application.ProductName);
            sb.Append("Version: ");
            sb.AppendLine(System.Windows.Forms.Application.ProductVersion);
            sb.AppendLine();
            sb.Append("© ");
            sb.AppendLine(System.Windows.Forms.Application.CompanyName);
            MessageBox.Show(sb.ToString(), "About");
        }

        
    }
}
