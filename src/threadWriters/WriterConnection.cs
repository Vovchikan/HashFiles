namespace HashFiles.src.threadWriters
{
    public abstract class WriterConnection
    {
        public abstract void SendHashData(HashFunctionResult res);
        public abstract void Open();
        public abstract void Close();
    }
}
