using KEngine.Attributes.UIA;
using KEngine.Classes.Entities;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace KEngine.Controls
{
    /// <summary>
    /// Interaction logic for UC_Main.xaml
    /// </summary>
    public partial class UC_Main : UserControl
    {
        public UC_Main()
        {
            InitializeComponent();
            InitTools();
        }

        protected void InitTools()
        {
            _generateEntitiesList();
        }

        private void _generateEntitiesList()
        {
            foreach (var v in Assembly.GetExecutingAssembly().GetTypes().Where(a => a.IsSubclassOf(typeof(KBaseEntity))).Where(a => Attribute.IsDefined(a, typeof(UIAEntityAttribute))))
            {
                foreach (UIAEntityAttribute a in v.GetCustomAttributes(typeof(UIAEntityAttribute), false))
                {
                    if (a.bAddMenuRecord)
                    {
                        MenuItem i = new MenuItem() { Header = a.MenuRecordName, CommandParameter = a};
                        i.Click += I_Click;
                        mi_Tools.Items.Add(i);
                    }
                }
            }
        }

        private void I_Click(object sender, RoutedEventArgs e)
        {
            var o = (MenuItem)sender;
            var p = o.CommandParameter as UIAEntityAttribute;
            SpawnEntityCompendium(p);
        }

        private void SpawnEntityCompendium(UIAEntityAttribute module)
        {
            string capt = (module == null) ? "UNDEFINED" : module.MenuRecordName;
            UserControl cont = null;
            if (module == null) cont = new UC_Dummy();
            else
            {
                Type typeEntity = module.EntityType;
                Type genericCompendium = typeof(UC_Compendium<>);
                Type constructedCompendium = genericCompendium.MakeGenericType(typeEntity);
                var cv = Activator.CreateInstance(constructedCompendium, null);
                cont = cv as UserControl;
                grd_Tool.Children.Clear();
                grd_Tool.Children.Add(cont);
            }
            ((MainWindow)this.Parent).SetModuleName(capt);
        }
    }
}
