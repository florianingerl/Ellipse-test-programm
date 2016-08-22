using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EllipseTestProgram.Tests
{
    [TestFixture]
    public class EllipseTest
    {
        [Test]
        public void EllipseCtorTest()
        {
            var importParameters = new EllipseImportParameters()
            {
                MiddlePoint = new System.Windows.Point(0,0),
                RadiusX = 500,
                RadiusY = 100,
                StartAngleDeg = 0,
                DeltaAngleDeg = 90
            };

            var ellipse = new Ellipse(importParameters);

            var centerPointParameterization = ellipse.GetCenterPointParameterization();

            Assert.That(centerPointParameterization.CenterX, Is.EqualTo(0).Within(1e-5 ) );
            Assert.That(centerPointParameterization.CenterY, Is.EqualTo(0).Within(1e-5) );
            Assert.That(centerPointParameterization.RadiusX, Is.EqualTo(importParameters.RadiusX).Within(1e-5) ) ;
            Assert.That(centerPointParameterization.RadiusY, Is.EqualTo(importParameters.RadiusY).Within(1e-5) );
            Assert.That(centerPointParameterization.Phi, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.StartAngleRad , Is.EqualTo(importParameters.StartAngleDeg * Math.PI/180).Within(1e-5) );
            Assert.That(centerPointParameterization.DeltaAngleRad, Is.EqualTo(importParameters.DeltaAngleDeg * Math.PI / 180).Within(1e-5) );

        }

        [Test]
        public void EllipseMirrorTest()
        {
            var importParameters = new EllipseImportParameters()
            {
                MiddlePoint = new System.Windows.Point(0, 0),
                RadiusX = 500,
                RadiusY = 100,
                StartAngleDeg = 0,
                DeltaAngleDeg = 90
            };

            var ellipse = new Ellipse(importParameters);
            var matrix = new System.Drawing.Drawing2D.Matrix(-1,0,0,1,0,0);
            ellipse.MultiplyWithMatrix(matrix);

            var centerPointParameterization = ellipse.GetCenterPointParameterization();

            Assert.That(centerPointParameterization.CenterX, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.CenterY, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusX, Is.EqualTo(importParameters.RadiusX).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusY, Is.EqualTo(importParameters.RadiusY).Within(1e-5));
            if (Math.Abs(centerPointParameterization.Phi) < 1e-5)
            {
                Assert.That(centerPointParameterization.StartAngleRad, Is.EqualTo(Math.PI).Within(1e-5));
            }
            else if (Math.Abs(centerPointParameterization.Phi - Math.PI) < 1e-5)
            {
                Assert.That(centerPointParameterization.StartAngleRad, Is.EqualTo(0).Within(1e-5));
            }
            else
            {
                Assert.Fail();
            }
            Assert.That(centerPointParameterization.DeltaAngleRad, Is.EqualTo(-Math.PI / 2).Within(1e-5));
        }

        [Test]
        public void EllipseScaleTest()
        {
            var importParameters = new EllipseImportParameters()
            {
                MiddlePoint = new System.Windows.Point(0, 0),
                RadiusX = 500,
                RadiusY = 100,
                StartAngleDeg = 0,
                DeltaAngleDeg = 90
            };

            var ellipse = new Ellipse(importParameters);
            var matrix = new System.Drawing.Drawing2D.Matrix(1/5.0f, 0, 0, 4, 0, 0);
            ellipse.MultiplyWithMatrix(matrix);

            var centerPointParameterization = ellipse.GetCenterPointParameterization();

            Assert.That(centerPointParameterization.CenterX, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.CenterY, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusX, Is.EqualTo(400).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusY, Is.EqualTo(100).Within(1e-5));

            Assert.That(centerPointParameterization.Phi, Is.EqualTo(Math.PI/2).Within(1e-5));
            Assert.That(centerPointParameterization.StartAngleRad, Is.EqualTo(-Math.PI/2).Within(1e-5) );
            Assert.That(centerPointParameterization.DeltaAngleRad, Is.EqualTo(Math.PI/2).Within(1e-5) );
        }

        [Test]
        public void Ellipse360DegreesTest()
        {
            var importParameters = new EllipseImportParameters()
            {
                MiddlePoint = new System.Windows.Point(0, 0),
                RadiusX = 500,
                RadiusY = 100,
                StartAngleDeg = 0,
                DeltaAngleDeg = 360
            };

            var ellipse = new Ellipse(importParameters);
            var centerPointParameterization = ellipse.GetCenterPointParameterization();

            Assert.That(centerPointParameterization.CenterX, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.CenterY, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusX, Is.EqualTo(500).Within(1e-5));
            Assert.That(centerPointParameterization.RadiusY, Is.EqualTo(100).Within(1e-5));

            Assert.That(centerPointParameterization.Phi, Is.EqualTo(0).Within(1e-5));
            Assert.That(centerPointParameterization.StartAngleRad, Is.EqualTo(0).Within(1e-5) );
            Assert.That(centerPointParameterization.DeltaAngleRad, Is.EqualTo(2 * Math.PI).Within(1e-5) );
        }

        }

    
}
