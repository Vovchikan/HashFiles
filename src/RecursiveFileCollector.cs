using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HashFiles.src;
using System.Collections.ObjectModel;

namespace HashFiles
{
    public class RecursiveFileCollector : FileCollector
    {
        private static MyConcurrentQueue<string> filesQueue;

        public void SetStash(MyConcurrentQueue<string> stash)
        {
            filesQueue = stash;
        }

        public void CollectFrom(params string[] paths)
        {
            if (paths.Length > 0)
                EnqueueAllFiles(paths);
            else
                throw new ArgumentException("Массив значений - пуст.");
        }

        private void EnqueueAllFiles(params string[] paths)
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
                    RecursivelyEnqueueDir(fullPath);
                }
                else
                    throw new ArgumentException(String.Format("Wrong path {0}", path));
            }
        }

        private void RecursivelyEnqueueDir(string targetDirectory)
        {
            string[] filesFromTargetDir = Directory.GetFiles(targetDirectory);
            foreach (string file in filesFromTargetDir)
                EnqueueFile(file);

            string[] subDirectories = Directory.GetDirectories(targetDirectory);
            foreach (string subdir in subDirectories)
                RecursivelyEnqueueDir(subdir);
        }

        private void EnqueueFile(string targetFile)
        {
            filesQueue.Enqueue(targetFile);
        }

    }
}
