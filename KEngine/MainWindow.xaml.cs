using KEngine.Classes;
using KEngine.Controls;
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

namespace KEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string TitleMask;

        public MainWindow()
        {
            InitializeComponent();
            this.Title += " " + KCore.GetAssemblyVersion();
            TitleMask = this.Title + " [{0}]";
        }

        public void InitTools()
        {
            this.ResizeMode = ResizeMode.CanResize;
            this.SizeToContent = SizeToContent.Manual;
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            this.Content = new UC_Main();
        }

        public void SetModuleName(string m)
        {
            this.Title = string.Format(TitleMask, m);
        }
    }
}
