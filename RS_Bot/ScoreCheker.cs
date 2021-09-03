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


        // user_id, login, password, tracking
        public bool chekScore(string userId, string login, string password, string url)
        {
           
            //Ссылка на поток
            //string url = "https://info.swsu.ru/run/vkbot/ball_all.php?id=5817&uid=900066587&gr=000000464&semestr=000000007";

            string name = @"reit/"+ userId + "/ball.txt";
            string nameOld = @"reit/"+ userId + "/OLDball.txt";



            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("1");
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://info.swsu.ru/index.php?action=auth&login="+login+"&password="+password+"&click_autorize=");
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            CookieContainer authInfo = new CookieContainer();
            request.CookieContainer = authInfo;

            byte[] EncodedPostParams = Encoding.GetEncoding(1251).GetBytes(data);
            request.ContentLength = EncodedPostParams.Length;
            request.GetRequestStream().Write(EncodedPostParams, 0, EncodedPostParams.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(request.CookieContainer.GetCookieHeader(request.RequestUri));


            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, request.CookieContainer.GetCookieHeader(request.RequestUri));
          //  string url = "https://info.swsu.ru/index.php?action=list_stud_reiting_devel&semestr=000000007&group=000000464&status=true";
          //  string file = "ball.html";
            wc.DownloadFile(url, name);


            /*
            //Грузим
            wc.DownloadFile(url, name);*/

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
