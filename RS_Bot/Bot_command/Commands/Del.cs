using System;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Data.SQLite;

namespace RS_Bot.Bot_command.Commands
{

    class Del : Command
    {
        public override string[] Names { get; set; } = new string[] { "del" };

        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sql)
        {
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
             $"Select user_id, tracking from UserData Where user_id = '{message.Chat.Id.ToString()}' AND tracking = '{message.Text.ToString().Remove(0, 4)}'",
             sql
             );
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            await client.SendTextMessageAsync(message.Chat.Id, $"Найдено и будет удалено: {dataSet.Tables[0].Rows.Count.ToString()}");

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                await client.SendTextMessageAsync(message.Chat.Id, $"№{i}: " + dataSet.Tables[0].Rows[i][1]);
            }

            SQLiteCommand command = new SQLiteCommand(
                $"Delete from UserData Where user_id = '{message.Chat.Id.ToString()}' AND tracking = '{message.Text.ToString().Remove(0,4)}'",
                sql);

            Console.WriteLine(command.ExecuteNonQuery().ToString());

        }
    }
}
