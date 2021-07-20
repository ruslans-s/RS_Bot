using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace RS_Bot.RssAnalize
{
    class RssCheker
    {
        private static string GetMd5(byte[] b)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(b);
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                hash.Append(data[i].ToString("x2"));
            }
            return hash.ToString();
        }

        string name = @"rutr.txt";
        string nameOld = @"OLDrutr.txt";

        public bool chekRutracker()
        {
            //Создаем веб клинет
            WebClient wc = new WebClient();
            //Ссылка на поток
            string url = "http://feed.rutracker.cc/atom/f/0.atom";
            //Пути файлов
           
            //Грузим
            wc.DownloadFile(url, name);

            //Записываем обратно удаляе первые 6 строк
            string[] s = File.ReadAllLines(name);
            File.WriteAllLines(name, s.Skip(6));

            //Сравнение
            string file1 = GetMd5(File.ReadAllBytes(name));
            string file2 = GetMd5(File.ReadAllBytes(nameOld));
            if (file1 == file2)
            {
                Console.WriteLine(DateTime.Now + " Поток не изменился");
                return false;
            }
            else
            {
                Console.WriteLine(DateTime.Now + " Поток изменился");
                return true;
            }
        }


        public List<string> GetArrayFromFile()
        {
            List<string> arrayN = new List<string>();
            StreamReader sr = new StreamReader(name);
            string line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line.IndexOf("title") > 0)
                {
                    arrayN.Add(line);
                }
            }
            sr.Close();

            List<string> arrayO = new List<string>();
            StreamReader sr2 = new StreamReader(nameOld);
            while (!sr2.EndOfStream)
            {
                line = sr2.ReadLine();
                if (line.IndexOf("title") > 0)
                {
                    arrayO.Add(line);
                }
            }
            sr2.Close();

            List<string> arrayForOut = new List<string>();
            for (int i = 0; i < arrayN.Count; i++)
            {
                for (int j = 0; j < arrayO.Count; j++)
                {
                    if (arrayN[i] == arrayO[j])
                    {
                        break;
                    }
                    if (j==(arrayO.Count-1))
                    {
                        //
                        line = Regex.Replace(arrayN[i], @"\[.*?\]", "");
                        line = Regex.Replace(line, @"\(.*?\)", "");
                        line = Regex.Replace(line, @"\<.*?\>", "").Trim();
                        arrayForOut.Add(line);
                        Console.WriteLine(line);
                    }
                }
            }

            System.IO.File.Delete(nameOld);
            File.Move(name, nameOld);

            return arrayForOut;
        }
    }
}
