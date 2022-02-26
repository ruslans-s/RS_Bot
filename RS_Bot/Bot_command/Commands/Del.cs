using System;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using SQLite;

namespace RS_Bot.Bot_command.Commands
{

    class Del : Command
    {
        public override string[] Names { get; set; } = new string[] { "/del" };

        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sql)
        {
            /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
              $"Select user_id, tracking from UserData Where user_id = '{message.Chat.Id.ToString()}' AND tracking = '{message.Text.ToString().Remove(0, 5)}'",
              sql
              );
             DataSet dataSet = new DataSet();

             dataAdapter.Fill(dataSet);
            */
            var query = sql.Query<UserData>("Select * from UserData  Where user_id = (?) AND tracking = (?)", message.Chat.Id.ToString(), message.Text.ToString().Remove(0, 5));

            await client.SendTextMessageAsync(message.Chat.Id, $"Найдено и будет удалено: {query.Count}");

            foreach (var s in query)
            {
                await client.SendTextMessageAsync(message.Chat.Id,  s.tracking);
            }
            /*
            SQLiteCommand command = new SQLiteCommand(
                $"Delete from UserData Where user_id = '{message.Chat.Id.ToString()}' AND tracking = '{message.Text.ToString().Remove(0,4)}'",
                sql);
            Console.WriteLine(command.ExecuteNonQuery().ToString());
            */
            sql.Execute("Delete from UserData Where user_id = (?) AND tracking = (?)", message.Chat.Id.ToString(), message.Text.ToString().Remove(0,5));


        }
    }

    [Table("UserData")]
    public class UserData
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int id { get; set; }

        [Column("user_id")]
        public string user_id { get; set; }

        [Column("tracking")]
        public string tracking { get; set; }

    }


}
