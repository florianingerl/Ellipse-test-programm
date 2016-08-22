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
using System.Windows.Shapes;

namespace EllipseTestProgram
{
    /// <summary>
    /// Interaction logic for NewEllipseDialog.xaml
    /// </summary>
    public partial class NewEllipseDialog : Window
    {
        public EllipseImportParameters ImportParameters { get; private set; }

        public bool OkClicked { get; set; } 

        public NewEllipseDialog()
        {
            ImportParameters = new EllipseImportParameters();
            InitializeComponent();
            DataContext = ImportParameters;
        }

        private void On_ButtonOk_Clicked(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            Close();
        }

        private void On_ButtonCancel_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
