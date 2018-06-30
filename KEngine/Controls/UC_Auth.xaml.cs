using KEngine.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KEngine.Controls
{
    /// <summary>
    /// Interaction logic for UC_Auth.xaml
    /// </summary>
    public partial class UC_Auth : UserControl
    {
        public UC_Auth()
        {
            InitializeComponent();
        }

        private bool _validateInputs()
        {
            if (string.IsNullOrWhiteSpace(tb_Login.Text)) { KCore.ShowKError("Empty Login"); return false; }
            if (string.IsNullOrWhiteSpace(pb_Password.Password)) { KCore.ShowKError("Empty Password"); return false; }
            return true;
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (_validateInputs())
            {
                if (!DB_Bridge.Instance.CheckIfAdminExist(tb_Login.Text))
                {
                    if (DB_Bridge.Instance.CreateAdmin(tb_Login.Text, pb_Password.Password))
                        KCore.ShowKError("Admin created!");
                }
                else KCore.ShowKError("Such Admin already exist");
            }
        }

        private void btn_Auth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_validateInputs())
                    if (DB_Bridge.Instance.CheckIfAdminExist(tb_Login.Text, pb_Password.Password))
                    {
                        ((MainWindow)this.Parent).InitTools();
                    }
                    else
                    {
                        KCore.ShowKError("Admin Not Found!");
                    }
            }
            catch(Exception ex)
            {
                KCore.ShowKError(ex.ToString());
            }
        }
    }
}
