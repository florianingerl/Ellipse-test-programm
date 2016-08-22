using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EllipseTestProgram
{
    public class EllipseCenterPointParameterization
    {
        public double StartAngleRad { get; set; }
        public double DeltaAngleRad { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Phi { get; set; }
    }

    public class EllipseEndPointParameterization
    {
        public double StartPointX { get; set; }
        public double StartPointY { get; set; }
        public double EndPointX { get; set; }
        public double EndPointY { get; set; }
        public float ArcFlag { get; set; }
        public float SweepFlag { get; set; }
        public double Phi { get; set; }

    }

    public class Ellipse
    {
        public float[] VectorList { get; private set; }

        public Ellipse(EllipseImportParameters eip)
        {
            VectorList = new float[LaserCommands.EllipseCommandLength];

            VectorList[0] = LaserCommands.EllipseCenter;
            VectorList[1] = (float)eip.MiddlePoint.X;
            VectorList[2] = (float)eip.MiddlePoint.Y;

            VectorList[3] = LaserCommands.EllipseRight;
            VectorList[4] = (float)(eip.MiddlePoint.X + eip.RadiusX);
            VectorList[5] = (float)eip.MiddlePoint.Y;

            VectorList[6] = LaserCommands.EllipseTop;
            VectorList[7] = (float)eip.MiddlePoint.X;
            VectorList[8] = (float)(eip.MiddlePoint.Y + eip.RadiusY);

            VectorList[9] = LaserCommands.EllipseStart;
            VectorList[10] = (float)(eip.RadiusX * Math.Cos(eip.StartAngleDeg * Math.PI / 180) + eip.MiddlePoint.X);
            VectorList[11] = (float)(eip.RadiusY * Math.Sin(eip.StartAngleDeg * Math.PI / 180) + eip.MiddlePoint.Y);

            VectorList[12] = LaserCommands.EllipseEnd;
            VectorList[13] = (float)(eip.RadiusX * Math.Cos((eip.StartAngleDeg + eip.DeltaAngleDeg) * Math.PI / 180) + eip.MiddlePoint.X);
            VectorList[14] = (float)(eip.RadiusY * Math.Sin((eip.StartAngleDeg + eip.DeltaAngleDeg) * Math.PI / 180) + eip.MiddlePoint.Y);

            VectorList[15] = LaserCommands.EllipseArcAndSweepFlag;
            VectorList[16] = eip.DeltaAngleDeg >= 0 ? 1 : 0;
            VectorList[17] = Math.Abs(eip.DeltaAngleDeg) > 180 ? 1 : 0;
        }


        public void MultiplyWithMatrix(Matrix matrix)
        {
            float m11 = matrix.Elements[0];
            float m12 = matrix.Elements[1];
            float m21 = matrix.Elements[2];
            float m22 = matrix.Elements[3];
            float m13 = matrix.Elements[4];
            float m23 = matrix.Elements[5];

            for (int i = 0; i < VectorList.Length; i += 3)
            {
                switch ((int)VectorList[i])
                {
                    case LaserCommands.EllipseCenter:
                    case LaserCommands.EllipseRight:
                    case LaserCommands.EllipseTop:
                    case LaserCommands.EllipseStart:
                    case LaserCommands.EllipseEnd:
                        float oldX = VectorList[i + 1];
                        float oldY = VectorList[i + 2];
                        VectorList[i + 1] = m11 * oldX + m12 * oldY + m13;
                        VectorList[i + 2] = m21 * oldX + m22 * oldY + m23;
                        break;
                    case LaserCommands.EllipseArcAndSweepFlag:
                        double det = m11 * m22 - m12 * m21;
                        if (det < 0)
                        {
                            VectorList[i + 1] = Math.Abs(VectorList[i + 1]) < 1e-30 ? 1 : 0;
                        }
                        break;

                }

            }

        }

        public
            EllipseCenterPointParameterization GetCenterPointParameterization()
        {
            
            EllipseEndPointParameterization eepp = GetEndPointParameterization();
            if (eepp == null) return null;

            EllipseCenterPointParameterization ecpp = new EllipseCenterPointParameterization();

            float cx = VectorList[1];
            float cy = VectorList[2];

            float rightx = VectorList[4];
            float righty = VectorList[5];

            float topx = VectorList[7];
            float topy = VectorList[8];

            //Calculate rx and ry and phi

            double rx = Math.Sqrt(Math.Pow(rightx - cx, 2) + Math.Pow(righty - cy, 2));
            double ry = Math.Sqrt(Math.Pow(topx - cx, 2) + Math.Pow(topy - cy, 2));

            if (ry > rx)
            {
                double z = ry;
                ry = rx;
                rx = z;
            }

            ecpp.CenterX = cx;
            ecpp.CenterY = cy;
            ecpp.RadiusX = rx;
            ecpp.RadiusY = ry;
            ecpp.Phi = eepp.Phi;

            double _x = Math.Cos(eepp.Phi) * (eepp.StartPointX - eepp.EndPointX) / 2 + Math.Sin(eepp.Phi) * (eepp.StartPointY - eepp.EndPointY) / 2;
            double _y = -Math.Sin(eepp.Phi) * (eepp.StartPointX - eepp.EndPointX) / 2 + Math.Cos(eepp.Phi) * (eepp.StartPointY - eepp.EndPointY) / 2;

            double someFactor = Math.Sqrt((Math.Pow(rx * ry, 2) - Math.Pow(rx, 2) * Math.Pow(_y, 2) - Math.Pow(ry, 2) * Math.Pow(_x, 2)) / (Math.Pow(rx, 2) * Math.Pow(_y, 2) + Math.Pow(ry, 2) * Math.Pow(_x, 2)));

            double _cx = someFactor * rx * _y / ry;
            double _cy = someFactor * (-1) * ry * _x / rx;

            if ( Math.Abs(eepp.SweepFlag - eepp.ArcFlag) <= 1e-15 )
            {
                _cx *= (-1);
                _cy *= (-1);
            }

            if ( Math.Abs(eepp.StartPointX - eepp.EndPointX) < 1e-5 && Math.Abs(eepp.StartPointY - eepp.EndPointY) < 1e-5 )
            {
                //eepp.StartAngle is the current point of the laser
                if (Math.Abs(eepp.SweepFlag) < 1e-15)
                {
                    ecpp.DeltaAngleRad = -2*Math.PI;
                }
                else
                {
                    ecpp.DeltaAngleRad = 2 * Math.PI;
                }
                return ecpp;
            }

            //make a sanity check for cx
            double cxSanity = Math.Cos(eepp.Phi) * _cx - Math.Sin(eepp.Phi) * _cy + (eepp.StartPointX + eepp.EndPointX) / 2;
            double cySanity = Math.Sin(eepp.Phi) * _cx + Math.Cos(eepp.Phi) * _cy + (eepp.StartPointY + eepp.EndPointY) / 2;

            Debug.Assert(Math.Abs(cxSanity - cx) <= 1e-3 && Math.Abs(cySanity - cy) <= 1e-3);

            double cosStartAngle = ((_x - _cx) / rx) / (Math.Sqrt(Math.Pow((_x - _cx) / rx, 2) + Math.Pow((_y - _cy) / ry, 2)));
            if (cosStartAngle > 1) cosStartAngle = 1;
            else if (cosStartAngle < -1) cosStartAngle = -1;
            double startAngle = Math.Acos(cosStartAngle);

            if ((_y - _cy) / ry < 0)
            {
                startAngle *= (-1);
            }

            double ux = (_x - _cx) / rx;
            double uy = (_y - _cy) / ry;
            double vx = (-_x - _cx) / rx;
            double vy = (-_y - _cy) / ry;

            double cosDeltaAngle = (ux * vx + uy * vy) / (Math.Sqrt(Math.Pow(ux, 2) + Math.Pow(uy, 2)) * Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2)));
            if (cosDeltaAngle > 1) cosDeltaAngle = 1;
            else if (cosDeltaAngle < -1) cosDeltaAngle = -1;
            double deltaAngle = Math.Acos(cosDeltaAngle);
            if (ux * vy - uy * vx < 0)
            {
                deltaAngle *= (-1);
            }

            if (Math.Abs(eepp.SweepFlag) <= 1e-15 && deltaAngle >= 0)
            {
                deltaAngle = deltaAngle - 2 * Math.PI;
            }
            else if (Math.Abs(eepp.SweepFlag - 1) <= 1e-15 && deltaAngle <= 0)
            {
                deltaAngle = deltaAngle + 2 * Math.PI;
            }

            ecpp.StartAngleRad = startAngle;
            ecpp.DeltaAngleRad = deltaAngle;
           

            return ecpp;

        }

        public EllipseEndPointParameterization GetEndPointParameterization()
        {
            EllipseEndPointParameterization eepp = new EllipseEndPointParameterization();
            float cx = VectorList[1];
            float cy = VectorList[2];

            float rightx = VectorList[4];
            float righty = VectorList[5];

            float topx = VectorList[7];
            float topy = VectorList[8];

            //Calculate rx and ry and phi

            double rx = Math.Sqrt(Math.Pow(rightx - cx, 2) + Math.Pow(righty - cy, 2));
            double ry = Math.Sqrt(Math.Pow(topx - cx, 2) + Math.Pow(topy - cy, 2));

            if (rx == 0 || ry == 0)
            {
                return null;
            }

            double phi = 0;
            if (rx > ry)
            {
                float vectorx = rightx - cx;
                float vectory = righty - cy;

                double cosAngle = vectorx / Math.Sqrt(Math.Pow(vectorx, 2) + Math.Pow(vectory, 2));
                if (cosAngle > 1) cosAngle = 1;
                else if (cosAngle < -1) cosAngle = -1;

                phi = Math.Acos(cosAngle);

                double sanityCheckX = Math.Cos(-phi) * vectorx - Math.Sin(-phi) * vectory;
                double sanityCheckY = Math.Sin(-phi) * vectorx + Math.Cos(-phi) * vectory;

                if (Math.Abs(sanityCheckY) > 1e-3)
                {
                    phi *= (-1);
                    sanityCheckX = Math.Cos(-phi) * vectorx - Math.Sin(-phi) * vectory;
                    sanityCheckY = Math.Sin(-phi) * vectorx + Math.Cos(-phi) * vectory;
                }

                Debug.Assert(Math.Abs(sanityCheckY) <= 1e-3);

            }
            else if (ry > rx)
            {
                double z = ry;
                ry = rx;
                rx = z;

                float vectorx = topx - cx;
                float vectory = topy - cy;

                double cosAngle = vectorx / Math.Sqrt(Math.Pow(vectorx, 2) + Math.Pow(vectory, 2));
                if (cosAngle > 1) cosAngle = 1;
                else if (cosAngle < -1) cosAngle = -1;
                phi = Math.Acos(cosAngle);

                double sanityCheckX = Math.Cos(-phi) * vectorx - Math.Sin(-phi) * vectory;
                double sanityCheckY = Math.Sin(-phi) * vectorx + Math.Cos(-phi) * vectory;

                if (Math.Abs(sanityCheckY) > 1e-3)
                {
                    phi *= (-1);
                    sanityCheckX = Math.Cos(-phi) * vectorx - Math.Sin(-phi) * vectory;
                    sanityCheckY = Math.Sin(-phi) * vectorx + Math.Cos(-phi) * vectory;
                }

                Debug.Assert(Math.Abs(sanityCheckY) <= 1e-3);

            }

            //startPoint and enpoint need to be rotated with phi around middlepoint
            eepp.StartPointX = VectorList[10];
            eepp.StartPointY = VectorList[11];

            eepp.EndPointX = VectorList[13];
            eepp.EndPointY = VectorList[14];

            eepp.SweepFlag = (int)VectorList[16];
            eepp.ArcFlag = (int)VectorList[17];
            eepp.Phi = phi;

            return eepp;


        }



    }
}
