using RS_Bot.Bot_command.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RS_Bot
{
    class Program
    {
        //Удалить токен!!!!!!!
        private static string token;
        static TelegramBotClient client;
        private static List<Bot_command.Command> comands;

        static string botVersion = "v0.421";

        public static SQLiteConnection sql = null;

        static void Main(string[] args)
        {
            startUp();
        }

        static string adminChatId,
            dataBaseConnectionAdres;

        static private logFilesWork log;

        static public void startUp()
        {
            log = new logFilesWork();

            Console.WriteLine($"RS_bot {botVersion} startup...");
            log.addToLogFile($"RS_bot {botVersion} startup...");

            log.addToLogFile("Чтение файла настроек");

            StreamReader sr2 = new StreamReader(@"setting.ini");
            //Читаем файл настроек
            adminChatId = sr2.ReadLine();
            sql = new SQLiteConnection(sr2.ReadLine());
            token = sr2.ReadLine();

            sr2.Close();
            log.addToLogFile("Файл настроек прочитан");

            log.addToLogFile("Открываем БД");
            sql.Open();

            if (sql.State == ConnectionState.Open)
            {
                Console.WriteLine("Бд подключена");
                log.addToLogFile("Бд подключена");
            }

            log.addToLogFile("Инцилизация команд");
            //Команды 
            comands = new List<Bot_command.Command>();
            comands.Add(new AddC());
            comands.Add(new adminsCommand(adminChatId));
            comands.Add(new List());
            comands.Add(new Del());
            comands.Add(new addInfo());
            comands.Add(new clearSw());

            log.addToLogFile("Соединение с ботом");

            client = new TelegramBotClient(token);

            client.StartReceiving();

            client.OnMessage += OnMessageHandler;

            RssAnalize.RssCheker cheker = new RssAnalize.RssCheker();

            int count = 0;

            sendMessageForAdmin("Bot_start");

            log.addToLogFile("Бот запущен вход в цикл");

            while (true)
            {
                log.addToLogFile("Проводится проверка RSS");
                if (cheker.chekRutracker())
                {
                    //207344692                  
                    updateRss(cheker.GetArrayFromFile(@"rutr.txt", @"OLDrutr.txt"));
                }

                if (count == 60 ^ count ==0)
                {
                    log.addToLogFile("Начало проверки баллов");
                    updateScoreInfo();
                    log.addToLogFile("Проведена проверка баллов");
                }

                /*
                //rutor blocked proxy mb?
                if (cheker.chekRutor())
                {
                    updateRss(cheker.GetArrayFromFile(@"rutor.txt", @"OLDrutor.txt"));
                }*/

                Thread.Sleep(60000);

                //Чек инфо
                if (count == 1440)
                {
                    sendMessageForAdmin("Я пишу тебе что я жив)");
                    log.addToLogFile("Я пишу тебе что я жив");
                    count = 0;
                }
                else
                {
                    count++;
                }

            }

            client.StopReceiving();

        }

        private static async void updateRss(List<string> list)
        {
            List<string> arrayForOut = list;

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
               "Select user_id, tracking from UserData",
               sql
               );

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < arrayForOut.Count; j++)
                {
                    if (arrayForOut[j].IndexOf((string)dataSet.Tables[0].Rows[i][1]) > -1)
                    {
                        Console.WriteLine((string)dataSet.Tables[0].Rows[i][1]);
                        await client.SendTextMessageAsync((string)dataSet.Tables[0].Rows[i][0], @$"Заметил одно из отслеживаемых {dataSet.Tables[0].Rows[i][1]} Раздача {arrayForOut[j]}");
                    }
                }
            }

            //  await client.SendTextMessageAsync(@"207344692", "Обновился рсс");
        }

        private static async void updateScoreInfo()
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
               "Select user_id, tracking from scoresData",
               sql
               );

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            ScoreCheker scoreCheker = new ScoreCheker();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                    if (scoreCheker.chekScore((string)dataSet.Tables[0].Rows[i][1], (string)dataSet.Tables[0].Rows[i][0]))
                    {
                        Console.WriteLine((string)dataSet.Tables[0].Rows[i][1]);
                        await client.SendTextMessageAsync((string)dataSet.Tables[0].Rows[i][0], @$"Замечено отличие в баллах ссылка: {dataSet.Tables[0].Rows[i][1]}");
                    }
            }

            //  await client.SendTextMessageAsync(@"207344692", "Обновился рсс");
        }


        private static async void sendMessageForAdmin(string text)
        {
            await client.SendTextMessageAsync(adminChatId, "[Log]" + text);
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {

            var messange = e.Message;
            if (messange.Text != null)
            {
                log.addToLogFile($"[Log]: Пришло сообщение От:{messange.From.FirstName} {messange.From.LastName} с текстом {messange.Text}");
                //await client.SendTextMessageAsync(messange.Chat.Id, messange.Text, replyToMessageId: messange.MessageId);
                foreach (var comn in comands)
                {
                    if (comn.Contains(messange.Text))
                    {
                        comn.Execute(messange, client, sql);
                    }
                }
            }
        }
    }
}
