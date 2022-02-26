//using System.Data.SQLite;
using RS_Bot.Bot_command.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;
using SQLite;

namespace RS_Bot
{
    class Program
    {
      
        private static string token;
        static TelegramBotClient client;
        private static List<Bot_command.Command> comands;

        static string botVersion = "v0.5";

        public static SQLiteConnection sql = null;

        static void Main(string[] args)
        {
            startUp();
        }

        static string adminChatId,
            dataBaseConnectionAdres;

        static public logFilesWork log;

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

          /*  log.addToLogFile("Открываем БД");
           // sql.Open();

            if (sql.s == ConnectionState.Open)
            {
                Console.WriteLine("Бд подключена");
                log.addToLogFile("Бд подключена");
            }*/

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

               // log.addToLogFile("Проводится проверка RSS");

                if (cheker.chekRutracker())
                {
                    //207344692                  
                    updateRss(cheker.GetArrayFromFile(@"rutr.txt", @"OLDrutr.txt"));
                }

                if (count % 60 == 0)
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

            try
            {
               /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
                   "Select user_id, tracking from UserData",
                   sql
                   );*/


                var query = sql.Query<UserData>("Select * from UserData");

                /* DataSet dataSet = new DataSet();

                 dataAdapter.Fill(dataSet);*/

                foreach (var s in query)
                {
                    for (int j = 0; j < arrayForOut.Count; j++)
                    {
                        if (arrayForOut[j].IndexOf(s.tracking) > -1)
                        {
                            Console.WriteLine(s.tracking);
                            await client.SendTextMessageAsync(s.user_id, @$"Заметил одно из отслеживаемых {s.tracking} Раздача {arrayForOut[j]}");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                sendMessageForAdmin("Ошибка! В обновлений RSS");
                log.addToLogFile("Ошибка!! " + err.ToString());
            }

            //  await client.SendTextMessageAsync(@"207344692", "Обновился рсс");
        }

        private static async void updateScoreInfo()
        {
            try
            {
                
                //[NewScoresData] (user_id, login, password, tracking )
                //Подгрузка базы
               /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
               "Select user_id, login, password, tracking from NewScoresData",
               sql
               );*/


                var dataSets = sql.Query<NewScoresData>("Select * from NewScoresData");

                /*
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                */

                ScoreCheker scoreCheker = new ScoreCheker();

                //Цикл по базе
                foreach (var s in dataSets)
                {
                    //Проверка баллов
                    // 0 1 2 3 
                    if (scoreCheker.chekScore(s.user_id, s.login, s.password, s.tracking))
                    {

                        Console.WriteLine(s.user_id);
                        await client.SendTextMessageAsync(s.user_id, @$"Замечено отличие в баллах: {s.login}");
                        //Формирование картинки баллов
                        PictureFromSroreTable.GetPic(s.user_id);
                        //Отправка картинки
                        using (var fileStream = new FileStream(@"reit/" + s.user_id + "/ball.jpg", FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await client.SendPhotoAsync(
                        chatId: s.user_id,
                        photo: new InputOnlineFile(fileStream),
                        caption: "Лови баллы!");
                        }

                    }
                }
            }
            catch (Exception err)
            {
                //В случае ошибки 
                sendMessageForAdmin("Ошибка! В обновлений баллов");
                log.addToLogFile("Ошибка!! "+err.ToString());
            }

            //  await client.SendTextMessageAsync(@"207344692", "Обновился рсс");
        }

        [Table("NewScoresData")]
        public class NewScoresData
        {
            [PrimaryKey, AutoIncrement]
            [Column("id")]
            public int id { get; set; }

            [Column("user_id")]
            public string user_id { get; set; }

            [Column("login")]
            public string login { get; set; }

            [Column("password")]
            public string password { get; set; }

            [Column("tracking")]
            public string tracking { get; set; }

        }

        private static async void sendMessageForAdmin(string text)
        {
            await client.SendTextMessageAsync(adminChatId, "[Log]" + text);
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {

            var messange = e.Message;
            try
            {
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
            catch (Exception err)
            {
                log.addToLogFile("Ошибка!! " + err.ToString());
                throw;
            }

        }
    }
}
