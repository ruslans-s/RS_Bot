using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{

    class Del : Command
    {
        public override string[] Names { get; set; } = new string[] { "del" };

        public override async void Execute(Message message, TelegramBotClient client, SqlConnection sql)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
             $"Select user_id, tracking from UserData Where user_id = N'{message.Chat.Id.ToString()}' AND tracking = N'{message.Text.ToString().Remove(0, 4)}'",
             sql
             );
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            await client.SendTextMessageAsync(message.Chat.Id, $"Найдено и будет удалено: {dataSet.Tables[0].Rows.Count.ToString()}");

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"№{i}: " + dataSet.Tables[0].Rows[i][1]);
            }

            SqlCommand command = new SqlCommand(
                $"Delete from [UserData] from UserData Where user_id = N'{message.Chat.Id.ToString()}' AND tracking = N'{message.Text.ToString().Remove(0,4)}'",
                sql);
            
            Console.WriteLine(command.ExecuteNonQuery().ToString());

        }
    }
}
