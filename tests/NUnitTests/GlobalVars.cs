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
        public static int tempFilesCount = 6;
        public static int onlyParentTempFilesCount = 0;
        public static string tempDirPath;
        private static readonly string TEMP_DIRRECTORY_NAME = "temp22052020";
        private static readonly string TEMP_SUBDIRRECTORY_NAME = "temp15102020";
        private static readonly string[] TEMP_FILES = 
            { $"file1.txt"
            , $"file2.txt"
            , $"file3.txt"
            , $"{TEMP_SUBDIRRECTORY_NAME}\\file1.txt"
            , $"{TEMP_SUBDIRRECTORY_NAME}\\file2.txt"
            , $"{TEMP_SUBDIRRECTORY_NAME}\\file3.txt"};

        public static void InitTempDir()
        {
            var workDir = TestContext.CurrentContext.TestDirectory;
            tempDirPath = Path.Combine(workDir, TEMP_DIRRECTORY_NAME);
            
            Directory.CreateDirectory(Path.Combine(tempDirPath, TEMP_SUBDIRRECTORY_NAME));
            InitTempFiles(TEMP_FILES);
        }

        private static void InitTempFiles(params string[] tempFiles)
        {
            for (int i = 0; i < TEMP_FILES.Length; i++)
            {
                TEMP_FILES[i] = Path.Combine(tempDirPath, TEMP_FILES[i]);
            }

            if (tempFiles.Length == 0)
                for (int i = 0; i < tempFilesCount; i++)
                {
                    File.Create($"{tempDirPath}\\file{i}.txt");
                }
            else
            {
                tempFilesCount = tempFiles.Length;
                foreach (var tempFile in tempFiles)
                {
                    File.Create(tempFile);
                    if (!tempFile.Contains(TEMP_SUBDIRRECTORY_NAME))
                        onlyParentTempFilesCount++;
                }
            }
        }
    }
}
