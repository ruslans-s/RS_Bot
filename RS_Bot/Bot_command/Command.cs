using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command
{
    public abstract class Command
    {
        public abstract string[] Names { get; set; }

        public abstract void Execute(Message message, TelegramBotClient client, SqlConnection sql);

        public bool Contains(string message)
        {
            foreach (var mess in Names)
            {
                if (message.Contains(mess))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
