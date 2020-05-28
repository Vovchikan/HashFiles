using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashFiles
{
    public static class RecursiveFilesCollector
    {
        private static MyConcurrentQueue<string> filesQueue;

        public static MyConcurrentQueue<string> GetFileQueue(params string[] paths)
        {
            filesQueue = new MyConcurrentQueue<string>();
            if (paths.Length > 0)
                enqueueAllFiles(paths);

            return filesQueue;
        }

        private static void enqueueAllFiles(params string[] paths)
        {
            foreach(string path in paths)
            {
                if (File.Exists(path))
                {
                    // Путь указывает на файл -> Добавить файл в очередь
                    EnqueueFile(path);
                }
                if (Directory.Exists(path))
                {
                    // Путь указывает на папку -> Добавить файлы/папки из неё в очередь
                    recursivelyEnqueueDirs(path);
                }
                else
                    throw new ArgumentException(String.Format("Wrong path {0}", path));
            }
        }

        private static void recursivelyEnqueueDirs(string targetDirectory)
        {
            string[] filesOfTargetDir = Directory.GetFiles(targetDirectory);
            foreach (string file in filesOfTargetDir)
                EnqueueFile(file);

            string[] subDirectories = Directory.GetDirectories(targetDirectory);
            foreach (string subdir in subDirectories)
                recursivelyEnqueueDirs(subdir);
        }

        private static void EnqueueFile(string targetFile)
        {
            filesQueue.Enqueue(targetFile);
        }
    }
}
