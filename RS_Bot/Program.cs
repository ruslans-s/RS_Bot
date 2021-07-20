using RS_Bot.Bot_command.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RS_Bot
{
    class Program
    {
        //Удалить токен!!!!!!!
        private static string token = "1689017519:AAEY2OC4_rogcWEJ1tXg6MD0ISH_48gGMcQ";
        static TelegramBotClient client;
        private static List<Bot_command.Command> comands;

        public static SqlConnection sql = null;

        static void Main(string[] args)
        {
            startUp();
        }

        static string adminChatId,
            dataBaseConnectionAdres;

        static public void startUp()
        {
            Console.WriteLine("RS_bot startup...");

            StreamReader sr2 = new StreamReader(@"setting.ini");

            adminChatId = sr2.ReadLine();

            sr2.Close();

            sql = new SqlConnection(@"Data Source=RsDataBase.mssql.somee.com;Initial Catalog=RsDataBase;Persist Security Info=True;User ID=RuslanS_SQLLogin_1;Password=jld1l61bip");

            sql.Open();
            comands = new List<Bot_command.Command>();
            comands.Add(new AddC());
            if (sql.State == ConnectionState.Open)
            {
                Console.WriteLine("Бд подключена");
            }

            client = new TelegramBotClient(token);

            client.StartReceiving();

            client.OnMessage += OnMessageHandler;

            RssAnalize.RssCheker cheker = new RssAnalize.RssCheker();

            int count = 0;

            sendMessageForAdmin("Bot_start");

            while (true)
            {
                if (cheker.chekRutracker())
                {
                    //207344692
                    updateRss(cheker.GetArrayFromFile());

                }

                Thread.Sleep(60000);

                //Чек инфо
                if (count == 30)
                {
                    sendMessageForAdmin("Я пишу тебе что я жив)");
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

            SqlDataAdapter dataAdapter = new SqlDataAdapter(
               "Select user_id, tracking from UserData",
               sql
               );

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                for(int j = 0; j < arrayForOut.Count; j++)
                {
                    if(arrayForOut[j].IndexOf((string)dataSet.Tables[0].Rows[i][1]) > -1)
                    {
                        Console.WriteLine((string)dataSet.Tables[0].Rows[i][1]);
                        await client.SendTextMessageAsync((string)dataSet.Tables[0].Rows[i][0], @$"Заметил одно из отслеживаемых {dataSet.Tables[0].Rows[i][1]}");
                    }
                }
            }

          //  await client.SendTextMessageAsync(@"207344692", "Обновился рсс");
        }

        private static async void sendMessageForAdmin(string text)
        {
            await client.SendTextMessageAsync(@"207344692", text);
        }

            private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {

            var messange = e.Message;
            if (messange.Text != null)
            {
                Console.WriteLine($"[Log]: Пришло сообщение От:{messange.From.FirstName} {messange.From.LastName} с текстом {messange.Text}");
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
