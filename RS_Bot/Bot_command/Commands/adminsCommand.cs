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
        public override string[] Names { get; set; } = new string[] { "getlog", "Getlog", "Лог" };

        public override async void Execute(Message message, TelegramBotClient client, SqlConnection sql)
        {
          
            if (message.Chat.Id.ToString() == adminId)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Привет Admin, секунду...");
                try
                {
                    using (var log = System.IO.File.Open("log.txt", FileMode.Open))
                    {
                        //Добавляем к старому содержимому файла
                        await client.SendDocumentAsync(adminId, new InputOnlineFile(log, @"log.txt"));
                    }
                }
                catch (IOException ioex)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"Файл занят");
                }
             
           
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"Сам такой");
                try
                {
                    using (var writer = new StreamWriter("log.txt", true))
                    {
                        //Добавляем к старому содержимому файла
                        writer.WriteLine(DateTime.Now + @" До меня домогался: " + message.Chat.Id.ToString() + " " + message.Chat.Username.ToString());
                    }
                }
                catch
                {
                    await client.SendTextMessageAsync(message.Chat.Id, $"Файл занят");
                }
            }
        }
    }
}
