using RS_Bot.Bot_command.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RS_Bot
{
    class Program
    {
        private static string token = "1689017519:AAEY2OC4_rogcWEJ1tXg6MD0ISH_48gGMcQ";
        static TelegramBotClient client;
        private static List<Bot_command.Command> comands;

        public static SqlConnection sql = null;

        static void Main(string[] args)
        {

            sql = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\repos\RS_Bot\RS_Bot\DataBase\database.mdf;Integrated Security=True");

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

            Console.ReadLine();
            client.StopReceiving();
           
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            /*
            var msg = e.Message;
            if(msg.Text != null)
            {
                Console.WriteLine("Сообщение");
                if(msg.Text == "/add")
                {
                    await client.SendTextMessageAsync(msg.Chat.Id, msg.Chat.Id.ToString());
                    await client.SendTextMessageAsync(msg.Chat.Id, "Введите название:");
                }
              
            }*/
            
            
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
