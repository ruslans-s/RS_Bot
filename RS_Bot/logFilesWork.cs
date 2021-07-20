using System;
using System.IO;

namespace RS_Bot
{
    class logFilesWork
    {
        string path = "log.txt";

        public logFilesWork()
        {
           //File.Create(path);
        }
        public void addToLogFile(string logText)
        {
            using (var writer = new StreamWriter(path, true))
            {
                //Добавляем к старому содержимому файла
                writer.WriteLine(DateTime.Now + logText);
            }
        }
    }
}
