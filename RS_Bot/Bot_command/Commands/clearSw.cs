using System;
using System.Collections.Generic;
using System.Data;
using SQLite;
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
            /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
              $"Select user_id, login, password, tracking from NewScoresData Where user_id = '{message.Chat.Id.ToString()}'",
              sql
              );
             DataSet dataSet = new DataSet();

             dataAdapter.Fill(dataSet);
            */
            var query = sql.Query<NewScoresData>("Select * from NewScoresData  Where user_id = (?)", message.Chat.Id.ToString());

            await client.SendTextMessageAsync(message.Chat.Id, $"Найдено и будет удалено: {query.Count}");

            foreach (var s in query)
            {
                await client.SendTextMessageAsync(message.Chat.Id, s.user_id + s.login + s.tracking);
            }
            /*
            SQLiteCommand command = new SQLiteCommand(
                $"Delete from NewScoresData Where user_id = '{message.Chat.Id.ToString()}'",
                sql);
            Console.WriteLine(command.ExecuteNonQuery().ToString());
            */

            sql.Execute("Delete from NewScoresData Where user_id (?)", message.Chat.Id.ToString());

        }

        public class NewScoresData
        {
            [PrimaryKey, AutoIncrement]
            [Column("id")]
            public int id { get; set; }

            [Column("user_id")]
            public string user_id { get; set; }

            [Column("login")]
            public string login { get; set; }

            [Column("password")]
            public string password { get; set; }

            [Column("tracking")]
            public string tracking { get; set; }

        }

    }
}
