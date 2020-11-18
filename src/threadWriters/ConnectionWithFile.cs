using System;
using System.IO;

using HashFiles.src.options;

namespace HashFiles.src.threadWriters
{
    public class ConnectionWithFile : ConnectionWith
    {
        private OptionsForFile fileOpt;
        private StreamWriter outputFile;

        public ConnectionWithFile(OptionsForFile fileOpt)
        {
            this.fileOpt = fileOpt;
        }

        public override void Close()
        {
            if (outputFile != null)
            {
                outputFile.Close();
            }
        }

        public override void PrepareForWriting()
        {
            var filePath = Path.Combine(fileOpt.OutputDirPath, fileOpt.FileName);
            if (!File.Exists(filePath) || fileOpt.Overwrite)
            {
                Directory.CreateDirectory(fileOpt.OutputDirPath);
                outputFile = File.CreateText(filePath);
            }
            else
                outputFile = new StreamWriter(new FileStream(filePath, FileMode.Append));
        }

        public override void SendHashData(HashFunctionResult res)
        {
            outputFile.WriteLine(res.ToString());
        }
    }
}
