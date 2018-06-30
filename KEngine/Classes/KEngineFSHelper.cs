using System;
using System.IO;

namespace KEngine.Classes
{
    public class KEngineFSHelper
    {
        private static readonly string DirAppData = "AppData";
        private static readonly string DirConfigs = "Configs";
        private static readonly string DirDB = "DB";
        private static readonly string PathRoot = AppDomain.CurrentDomain.BaseDirectory;

        private static string[] RequiredPaths;

        public string PathAppData { get; private set; }
        public string PathConfigs { get; private set; }
        public string PathDB { get; private set; }
        public string ExtConfig { get; private set; }



        public KEngineFSHelper()
        {
            PathAppData = Path.Combine(PathRoot, DirAppData);
            PathConfigs = Path.Combine(PathAppData, DirConfigs);
            PathDB = Path.Combine(PathAppData, DirDB);
            ExtConfig = "kconfig";
            RequiredPaths = new string[] { PathAppData, PathConfigs, PathDB };
            CreateRequiredPathes();
        }
        private void CreateRequiredPathes()
        {
            foreach (var v in RequiredPaths)
                if (!Directory.Exists(v)) Directory.CreateDirectory(v);
        }
    }
}
