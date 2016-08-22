using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace EllipseTestProgram
{
    public class EllipseCanvas : Canvas
    {
        private double halfPageWidth = 500; //This will be half the width of the laser field!
        private double halfPageHeight = 500; //This will be half the height of the laser field!

        private double zoom = 1;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double w = ActualWidth;
            double h = ActualHeight;

            double viewportOrgX = 0;
            double viewportOrgY = 0;
            double viewportWidth = w;
            double viewportHeight = h;

            double windowOrgX;
            double windowOrgY;
            double windowWidth;
            double windowHeight;

            if (h > w)
            {
                double newHalfPageWidth = w / h * halfPageHeight;
                windowOrgX = -2 * newHalfPageWidth * 1 / zoom;
                windowOrgY = 2 * halfPageHeight * 1 / zoom;

                windowWidth = 4 * newHalfPageWidth * 1 / zoom;
                windowHeight = -4 * halfPageHeight * 1 / zoom;
            }
            else
            {
                double newHalfPageHeight = h / w * halfPageWidth;
                windowOrgX = -2 * halfPageWidth * 1 / zoom;
                windowOrgY = 2 * newHalfPageHeight * 1 / zoom;
                windowWidth = 4 * halfPageWidth * 1 / zoom;
                windowHeight = -4 * newHalfPageHeight * 1 / zoom;
            }

            //This mimics the GDI methods SetViewportOrgEx, SetViewportExtEx, SetWindowOrgEx, SetWindowExtEx
            dc.PushTransform(new TranslateTransform(viewportOrgX, viewportOrgY));

            dc.PushTransform(new ScaleTransform(viewportWidth / windowWidth, viewportHeight / windowHeight));

            dc.PushTransform(new TranslateTransform(-windowOrgX, -windowOrgY));

            //dc.DrawEllipse(Brushes.Blue, new Pen(Brushes.Red, 2.0), new System.Windows.Point(0, 0), 100, 300);
            DrawEllipse(dc);
        }

        private void DrawEllipse(DrawingContext dc)
        {
            Ellipse ellipse = MainWindow.ActiveWindow.CurrentEllipse;
            if (ellipse == null)
            {
                return;
            }
            
            EllipseEndPointParameterization eepp = ellipse.GetEndPointParameterization();
            if (eepp == null) return;

            EllipseCenterPointParameterization ecpp = ellipse.GetCenterPointParameterization();
            if (ecpp == null) return;

            dc.DrawRectangle(Brushes.Blue, new Pen(Brushes.Black, 1), new System.Windows.Rect(eepp.StartPointX - 5, eepp.StartPointY - 5, 10, 10));
            dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1), new System.Windows.Rect(eepp.EndPointX - 5, eepp.EndPointY - 5, 10, 10));

            double oldPointX = eepp.StartPointX;
            double oldPointY = eepp.StartPointY;

            double stepAngle = ecpp.DeltaAngleRad / 100;
            double currentAngle = ecpp.StartAngleRad;

            for (int i = 0; i < 100; ++i)
            {
                currentAngle += stepAngle;
                double newPointX = Math.Cos(ecpp.Phi) * ecpp.RadiusX * Math.Cos(currentAngle) - Math.Sin(ecpp.Phi) * ecpp.RadiusY * Math.Sin(currentAngle) + ecpp.CenterX;
                double newPointY = Math.Sin(ecpp.Phi) * ecpp.RadiusX * Math.Cos(currentAngle) + Math.Cos(ecpp.Phi) * ecpp.RadiusY * Math.Sin(currentAngle) + ecpp.CenterY;

                dc.DrawLine(new Pen(Brushes.Black, 1), new System.Windows.Point(oldPointX, oldPointY), new System.Windows.Point(newPointX, newPointY));

                oldPointX = newPointX;
                oldPointY = newPointY;
            }
            

        }


    }
}
