using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace KEngine.Classes
{
    public static class KCore
    {
        public static KEngineFSHelper FS = new KEngineFSHelper();
        public static string GetAssemblyVersion() { return Assembly.GetCallingAssembly().GetName().Version.ToString(); }
        public static void ShowKError(string err)
        {
            StackTrace st = new StackTrace();
            MessageBox.Show(err, string.Format($"{st.GetFrame(1).GetMethod().ReflectedType.Name} - {st.GetFrame(1).GetMethod().Name}"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static Window GenerateWindow(string t, object c, Window owner ,ResizeMode r = ResizeMode.NoResize, WindowState ws = WindowState.Normal)
        {
            return new Window()
            {
                Title = $"{t} - {GetAssemblyVersion()}",
                Content = c,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Icon = owner.Icon,
                Owner = owner,
                ResizeMode = r
            };
        }
    }
}
