using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashFiles
{
    public class RecursiveFilesCollector
    {
        private static MyConcurrentQueue<string> filesQueue;

        public RecursiveFilesCollector(MyConcurrentQueue<string> filesQueue)
        {
            RecursiveFilesCollector.filesQueue = filesQueue;
        }

        public void CollectFilesToQueue(params string[] paths)
        {
            if (paths.Length > 0)
                enqueueAllFiles(paths);
        }

        private void enqueueAllFiles(params string[] paths)
        {
            foreach(string path in paths)
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    // Путь указывает на файл -> Добавить файл в очередь
                    EnqueueFile(fullPath);
                }
                if (Directory.Exists(fullPath))
                {
                    // Путь указывает на папку -> Добавить файлы/папки из неё в очередь
                    recursivelyEnqueueDirs(fullPath);
                }
                else
                    throw new ArgumentException(String.Format("Wrong path {0}", path));
            }
        }

        private void recursivelyEnqueueDirs(string targetDirectory)
        {
            string[] filesOfTargetDir = Directory.GetFiles(targetDirectory);
            foreach (string file in filesOfTargetDir)
                EnqueueFile(file);

            string[] subDirectories = Directory.GetDirectories(targetDirectory);
            foreach (string subdir in subDirectories)
                recursivelyEnqueueDirs(subdir);
        }

        private void EnqueueFile(string targetFile)
        {
            filesQueue.Enqueue(targetFile);
        }
    }
}
