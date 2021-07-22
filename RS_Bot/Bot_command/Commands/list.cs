using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class list : Command
    {
        public override string[] Names { get; set; } = new string[] { "list", "List", "список", "Список" };

        public override async void Execute(Message message, TelegramBotClient client, SqlConnection sql)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
               $"Select user_id, tracking from UserData Where user_id = N'{message.Chat.Id.ToString()}'",
               sql
               );
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"№{i}: "+ dataSet.Tables[0].Rows[i][1]);
            }
        //    return (dataSet.Tables[0].Rows.Count == 0);
        }
    }
}
