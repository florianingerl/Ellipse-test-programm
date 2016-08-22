using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EllipseTestProgram
{
    public class EllipseImportParameters : INotifyPropertyChanged
    {

        public System.Windows.Point MiddlePoint 
        {
            get;
            set;
        }

        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public double StartAngleDeg { get; set; }
        public double DeltaAngleDeg { get; set; }


        public EllipseImportParameters()
        {
            MiddlePoint = new System.Windows.Point();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName) );
            }
        }
    }
}
