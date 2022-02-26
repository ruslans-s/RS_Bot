using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace RS_Bot
{
    class PictureFromSroreTable
    {
        static string userId;
       public static bool GetPic(string userIdNew)
        {
            userId = userIdNew;
            List<string> stringFromFile = Clearing(userId);

            List<List<string>> toDArray = new List<List<string>>();

            if (stringFromFile.Count == 0)
            {
                Console.WriteLine("error");

                return false;
            }
            else
            {
                toDArray = FormingArrayFromHtml(stringFromFile);

                 DrawingToJpeg(toDArray);

                return true;
            }

        }


        //Удаляет html код
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        //Достает из html массив с данными таблицы
        static List<string> Clearing(string user_id)
        {
            List<string> stringFromFile = new List<string>();
            bool openTbody,
                closeTbody;

            openTbody = false;
            closeTbody = false;

            StreamReader Sites = new StreamReader(@"./reit/" + user_id + "/OLDball.txt");
            string line;
            while (!Sites.EndOfStream)
            {

                line = Sites.ReadLine();
                if (!openTbody)
                {
                    if (line.IndexOf("<tbody>") != -1)
                    {
                        openTbody = true;
                        continue;
                    }
                }
                else
                {
                    if (line.IndexOf("</tbody>") != -1)
                    {
                        closeTbody = true;
                        continue;
                    }
                }

                if (openTbody == true & closeTbody == false)
                {
                    
                    if (line.IndexOf("<td></td>") != -1) line = "<td>Пусто</td>";
                    if (line.IndexOf($"\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>") != -1)
                    {
                        line = "<td>Пусто</td>";
                        Console.Write("Верстальшик сайта не человек");
                    }
                    stringFromFile.Add(line.Trim().Replace("\t", ""));
                }

            }
            Sites.Close();

            //Очищаем html код
            for (int i = 0; i < stringFromFile.Count; i++)
            {
                // Console.WriteLine();
                stringFromFile[i] = StripHTML(stringFromFile[i]);
            }

            stringFromFile.RemoveAll(x => x == "");

            return stringFromFile;
        }
        //Формируем из одномерного массива с данными двумерный
        static List<List<string>> FormingArrayFromHtml(List<string> stringFromFile)
        {

            List<List<string>> toDArray = new List<List<string>>();
            List<string> tempArray = new List<string>();

            //17
            int i = 0;
            while (i < stringFromFile.Count)
            {
                //  Console.WriteLine(stringFromFile[i]);

                //Курс c https://do.swsu.ru/ не привязан к дисциплине
                if (stringFromFile[i].IndexOf("Заказать") < 0 & stringFromFile[i].IndexOf("Курс") < 0 & stringFromFile[i].IndexOf("курс") < 0)
                {
                    tempArray.Add(stringFromFile[i]);
                }
                if (tempArray.Count == 19)
                {
                    toDArray.Add(tempArray);
                    tempArray = new List<string>();
                }
                i++;
            }

            return toDArray;
        }
        //Рисуем картинку
        static void DrawingToJpeg(List<List<string>> array)
        {
            int xo = 0,
                yo = 0,
                xn = 250,
                yn = 100;

            // Создаём битмап с нужными размерами и форматом пикселей.
            Bitmap bmp1 = new Bitmap(14 * 50 + 730, array.Count * 100, PixelFormat.Format24bppRgb);
            for (int j = 0; j < array.Count; j++)
            {
                for (int i = 0; i < array[j].Count; i++)
                {
                    string text = "  " + array[j][i];
                    Rectangle rect = new Rectangle(xo, yo, xn, yn);

                    using (Graphics g = Graphics.FromImage(bmp1))
                    using (Font font = new Font("Arial", 16))
                    {

                        // Заливаем фон нужным цветом.
                        g.FillRectangle(GetColor(i, j), rect);

                        // Выводим текст.
                        g.DrawString(
                            text,
                            font,
                            Brushes.Black, // цвет текста
                            rect, // текст будет вписан в указанный прямоугольник
                            StringFormat.GenericTypographic
                            );
                    }

                    xo = xn;

                    if (i == 0 | i == 1 | i == 3)
                    {
                        xn += 100;
                    }
                    else if (i == 2)
                    {
                        xn += 180;
                    }
                    else
                    {
                        xn += 50;
                    }
                }
                xo = 0;
                xn = 250;
                yo = yn;
                yn += 100;
            }

            System.IO.File.Delete(@"reit/" + userId + "/ball.jpg");
            bmp1.Save(AppDomain.CurrentDomain.BaseDirectory + @"reit/" + userId+"/ball.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        //выбо кисти
        static Brush GetColor(int i, int j)
        {
            if (j % 2 == 0)
            {
                if (i % 2 != 0)
                {
                    // Заливаем фон нужным цветом.
                    return Brushes.LightGray;
                }
                else
                {
                    // Заливаем фон нужным цветом.
                    return Brushes.White;
                }
            }
            else
            {
                if (i % 2 == 0)
                {
                    // Заливаем фон нужным цветом.
                    return Brushes.LightGray;
                }
                else
                {
                    // Заливаем фон нужным цветом.
                    return Brushes.White;
                }
            }

        }
    }
}
