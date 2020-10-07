using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HashFiles
{
    static class FindFiles
    {
        public static Queue<string> files = new Queue<string>();

        public static void GetFiles(params string[] args)
        {
            getFiles(args);
        }

        #region Private methods
        private static void getFiles(params string[] args)
        {
            if(args.Length == 0)
            {
                // Do smth
            }

            foreach(string path in args)
            {
                if (File.Exists(path))
                {
                    // Путь указывает на файл -> Добавить файл в очередь
                    processFile(path);
                }
                else if (Directory.Exists(path))
                {
                    // Путь указывает на папку -> Получить файлы из неё
                    processDirectory(path);
                }
                else
                {
                    // Путь не указывает на файл или на директорию -> Вывести ошибку?
                }
            }
        }

        private static void processDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                processFile(fileName);

            string[] subdirEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdir in subdirEntries)
                processDirectory(subdir);
        }

        private static void processFile(string targetFile)
        {
            addToQueue(targetFile);
            Console.WriteLine(targetFile);
        }

        private static void addToQueue(string targetFile)
        {
            lock (FindFiles.files)
            {
                files.Enqueue(targetFile);
            }
        }

        /// <summary>
        /// Возвращает имя файла из хранилища и размер хранилища
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, int> TakeFromStorage()
        {
            try
            {
                lock (FindFiles.files)
                {
                    return new Tuple<string, int>(files.Dequeue(), files.Count);
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        #endregion
    }
}
