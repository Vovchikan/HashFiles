using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCollectingFiles
{
    public static class GlobalVars
    {
        public static readonly int tempFilesCount = 2;
        public static readonly string TEMP_DIRRECTORY_NAME = "temp22052020";
        public static readonly string PARRENT_DIRRERCTORY_NAME = "tests";
        public static string tempDirPath;

        public static void InitTempDir()
        {
            var workDir = TestContext.CurrentContext.TestDirectory;
            
            while(String.IsNullOrEmpty(tempDirPath))
            {
                var parrentInfo = System.IO.Directory.GetParent(workDir); ;
                if (parrentInfo.Name == PARRENT_DIRRERCTORY_NAME)
                    CombineParentFullNameAndTempDirName(parrentInfo.FullName);
                workDir = parrentInfo.FullName;
            }
        }

        private static void CombineParentFullNameAndTempDirName(string parentFullName)
        {
            tempDirPath = Path.Combine(parentFullName, TEMP_DIRRECTORY_NAME);
        }
    }
}
