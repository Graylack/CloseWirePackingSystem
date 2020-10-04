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

namespace CloseWirePackingSystem
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.grid.MouseLeftButtonDown += (o, e) => { DragMove(); };
            var LoginGuiSet = new LoginConfigRead();
        }

        private void LoginQuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var brush = new LinearGradientBrush();

            var StartPoint = new GradientStop();
            var EndPoint = new GradientStop();

            StartPoint.Offset = 0;
            StartPoint.Color = Colors.Red;
            brush.GradientStops.Add(StartPoint);

            EndPoint.Offset = 1;
            EndPoint.Color = Colors.Blue;
            brush.GradientStops.Add(EndPoint);

            logoPanel.Background = brush;
        }
    }
}
