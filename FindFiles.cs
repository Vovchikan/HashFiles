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
        public static MyTaskQueue<string> files = new MyTaskQueue<string>();

        public static void GetFiles(params string[] args)
        {
            getFiles(args);
        }

        #region Private methods
        private static void getFiles(params string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Где искать???!!");
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
                    Console.WriteLine("{0} - неверно указан путь!", path);
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
            files.Enqueue(targetFile);
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
                    return files.DequeueAndCount();
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
