using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RS_Bot
{
    class ScoreCheker
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

       

        public bool chekScore(string url, string userId)
        {
            //Создаем веб клинет
            WebClient wc = new WebClient();

            //Ссылка на поток
            //string url = "https://info.swsu.ru/run/vkbot/ball_all.php?id=5817&uid=900066587&gr=000000464&semestr=000000007";

            string name = @"reit/"+ userId + "/ball.txt";
            string nameOld = @"reit/"+ userId + "/OLDball.txt";

            //Грузим
            wc.DownloadFile(url, name);

            //Если сервак лег
            using (var f = File.OpenText(name))
            {
                while (!f.EndOfStream)
                {
                    var line = f.ReadLine();
                    if(line.IndexOf("error.php") != -1)
                    {
                        return false;
                    }
                }
            }

            /*
            //Записываем обратно удаляе первые 6 строк
            string[] s = File.ReadAllLines(name);
            File.WriteAllLines(name, s.Skip(6));
            */

            //Сравнение
            string file1 = GetMd5(File.ReadAllBytes(name));
            string file2 = GetMd5(File.ReadAllBytes(nameOld));
            if (file1 == file2)
            {
                Console.WriteLine(DateTime.Now + " Поток баллов не изменился");
                return false;

            }
            else
            {
                Console.WriteLine(DateTime.Now + " Поток баллов изменился");

                System.IO.File.Delete(nameOld);
                File.Move(name, nameOld);

                return true;
            }
        }

    }
}
