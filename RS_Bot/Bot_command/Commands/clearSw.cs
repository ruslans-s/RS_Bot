using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class clearSw : Command
    {
        public override string[] Names { get; set; } = new string[] { "/clearsw" };

        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sql)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
             $"Select user_id, login, password, tracking from NewScoresData Where user_id = '{message.Chat.Id.ToString()}'",
             sql
             );
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            await client.SendTextMessageAsync(message.Chat.Id, $"Найдено и будет удалено: {dataSet.Tables[0].Rows.Count.ToString()}");

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"№{i}: " + dataSet.Tables[0].Rows[i][1] + dataSet.Tables[0].Rows[i][2] + dataSet.Tables[0].Rows[i][3]);
            }
            SQLiteCommand command = new SQLiteCommand(
                $"Delete from NewScoresData Where user_id = '{message.Chat.Id.ToString()}'",
                sql);

            Console.WriteLine(command.ExecuteNonQuery().ToString());

        }
    }
}
