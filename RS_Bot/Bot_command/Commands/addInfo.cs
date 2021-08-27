using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class addInfo : Command
    {
        static SQLiteConnection sql = null;

        public override string[] Names { get; set; } = new string[] { "infosw" };
        static private void AddNewUserId(string id, string trak)
        {
            SQLiteCommand command = new SQLiteCommand(
                $"insert into [scoresData] (user_id, tracking) values ('{id}', '{trak}')",
                sql);
            Console.WriteLine(command.ExecuteNonQuery().ToString());
        }
        //[scoresData]
        ///scoresData
        static private bool getData(string id)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
                $"Select user_id, tracking from scoresData Where user_id = '{id}'",
                sql
                );
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);
            return (dataSet.Tables[0].Rows.Count == 0);
        }

        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sqlN)
        {
            string name = message.Text.Remove(0, 6);
            // await client.SendTextMessageAsync(message.Chat.Id, $"Напиши название сериала:)");
            sql = sqlN;
            if (getData(message.Chat.Id.ToString()))
            {
                AddNewUserId(message.Chat.Id.ToString(), name);
                await client.SendTextMessageAsync(message.Chat.Id, $"Добавлено");
                perebor_updates("reit/1", "reit/"+message.Chat.Id);
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Запрос от такого Id уже есть в базе");
            }
        }

        //Копирования конечных файлов
        //begin_dir - директория источник.
        //end_dir - директория приёмник.
        public void perebor_updates(string begin_dir, string end_dir)
        {
            //Создаем папку
            Directory.CreateDirectory(end_dir);

            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
            //Перебираем все внутренние папки
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(end_dir + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                perebor_updates(dir.FullName, end_dir + "\\" + dir.Name);
            }

            //Перебираем файлики в папке источнике.
            foreach (string file in Directory.GetFiles(begin_dir))
            {
                //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                //Копируем файлик с перезаписью из источника в приёмник.
                System.IO.File.Copy(file, end_dir + "\\" + filik, true);
            }
        }
    }
}
