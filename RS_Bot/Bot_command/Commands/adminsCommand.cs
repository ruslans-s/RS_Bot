using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace RS_Bot.Bot_command.Commands
{
    class adminsCommand : Command
    {
        private string adminId;

        public adminsCommand(string setAdminsId)
        {
            adminId = setAdminsId;
        }
        public override string[] Names { get; set; } = new string[] { "getlog" };

        public override async void Execute(Message message, TelegramBotClient client, SqlConnection sql)
        {
            if (message.Chat.Id.ToString() == adminId)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Привет Admin, секунду...");
                FileStream log = System.IO.File.Open("log.txt", FileMode.Open);
                await client.SendDocumentAsync(adminId, new InputOnlineFile(log, @"log.txt"));
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Сам такой");
                using (var writer = new StreamWriter("log.txt", true))
                {
                    //Добавляем к старому содержимому файла
                    writer.WriteLine(DateTime.Now + @" До меня домогался: " + message.Chat.Id.ToString() + " " + message.Chat.Username.ToString());
                }
            }
        }
    }
}
