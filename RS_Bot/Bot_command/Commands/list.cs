using System;
using System.Collections.Generic;
using System.Data;
using SQLite;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class List : Command
    {
        public override string[] Names { get; set; } = new string[] { "/list", "/List" };

        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sql)
        {
           /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
               $"Select user_id, tracking from UserData Where user_id = '{message.Chat.Id.ToString()}'",
               sql
               );
            */

            var query = sql.Query<UserData>("Select * from UserData Where user_id = (?)", message.Chat.Id.ToString());

          /*  DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);*/

            //Если база пуста написать 
            if (query.Count > 0)
            {
                foreach (var s in query)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, s.tracking);
                }
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Записы в базе не обнаружены");
            }

        //    return (dataSet.Tables[0].Rows.Count == 0);
        }
    }
}
