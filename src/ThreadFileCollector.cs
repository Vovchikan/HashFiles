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
                foreach (var path in paths)
                {
                    var fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                        EnqueueFile(fullPath);
                    else
                        TryCollectFromDirectory(path);
                }
            });
            thread.Start();
        }

        private void TryCollectFromDirectory(string fullDirectoryPath)
        {
            try
            {
                CollectFromDirectory(fullDirectoryPath);
            }
            catch (ArgumentException e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private void CollectFromDirectory(string fullDirectoryPath)
        {
            if (Directory.Exists(fullDirectoryPath))
            {
                string[] files = Directory.GetFiles(fullDirectoryPath);
                foreach (string file in files)
                    EnqueueFile(file);

                if (recursive)
                {
                    string[] subDirectories = Directory.GetDirectories(fullDirectoryPath);
                    foreach(string subDir in subDirectories)
                        RecursivelyEnqueueDir(subDir);
                }
            }
            else
                throw new ArgumentException($"Wrong path {fullDirectoryPath}");
        }

        private void RecursivelyEnqueueDir(string targetDirectory)
        {
            string[] filesFromTargetDir = Directory.GetFiles(targetDirectory);
            foreach (string file in filesFromTargetDir)
                EnqueueFile(file);

            string[] subDirectories = Directory.GetDirectories(targetDirectory);
            foreach (string subDir in subDirectories)
                RecursivelyEnqueueDir(subDir);
        }

        private void EnqueueFile(string targetFile)
        {
            stash.Enqueue(targetFile);
        }
    }
}
