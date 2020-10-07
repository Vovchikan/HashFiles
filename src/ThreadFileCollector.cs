using HashFiles.src;
using System;
using System.IO;
using System.Threading;

namespace HashFiles
{
    public class ThreadFileCollector
    {
        private static Thread thread;
        private MyConcurrentQueue<string> stash;
        private readonly bool recursive;

        public ThreadFileCollector(bool recursive)
        {
            this.recursive = recursive;
        }

        public void Join()
        {
            thread.Join();
        }

        public Thread GetThread()
        {
            return thread;
        }

        public ThreadState ThreadState
        {
            get => thread.ThreadState;
        }

        public void ExecuteToFrom(MyConcurrentQueue<string> stash, params string[] paths)
        {
            this.stash = stash;
            thread = new Thread(() =>
            {
                foreach(var path in paths)
                    TryCollectFrom(path);
            });
            thread.Start();
        }

        private void TryCollectFrom(string path)
        {
            try
            {
                CollectFrom(path);
            }
            catch (ArgumentException e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private void CollectFrom(string path)
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
                if(recursive)
                    RecursivelyEnqueueDir(fullPath);
            }
            else
                throw new ArgumentException($"Wrong path {path}");
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
            stash.Enqueue(targetFile);
        }
    }
}
