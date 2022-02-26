using System;
using System.Collections.Generic;
using System.Data;
using SQLite;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class AddC : Command
    {
        static SQLiteConnection sql = null;
        public override string[] Names { get; set; } = new string[] { "/add" };

        static private void AddNewUserId(string id, string trak)
        {
           /* SQLiteCommand command = new SQLiteCommand(
                $"insert into [UserData] (user_id, tracking) values ('{id}', '{trak}')",
                sql);
            */
            sql.Execute("insert into UserData (user_id, tracking) values(?,?)", id, trak);


         //   Console.WriteLine(command.ExecuteNonQuery().ToString());
        }

     
        static private bool getData(string id,string trak)
        {
           /* SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(
                $"Select user_id, tracking from UserData Where user_id = '{id}' AND tracking = '{trak}'",
                sql
                );*/

            var query = sql.Query<UserData>("Select * from UserData  Where user_id = (?) AND tracking = (?)", id, trak);

            /*
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            */

            return (query.Count == 0);
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


        public override async void Execute(Message message, TelegramBotClient client, SQLiteConnection sqlN)
        {

            if (message.Text.Length < 5) return; 
            
                //Удаляем команду и приводим к нижнему регистру
                string name = message.Text.Remove(0, 5).ToLower();
            
           // await client.SendTextMessageAsync(message.Chat.Id, $"Напиши название сериала:)");
            sql = sqlN;
           if(getData(message.Chat.Id.ToString(), name))
            {
                AddNewUserId(message.Chat.Id.ToString(), name);
                await client.SendTextMessageAsync(message.Chat.Id, $"Добавлено");
            } else {
                await client.SendTextMessageAsync(message.Chat.Id, $"Такой запрос от такого Id уже есть в базе");
            }
        }
    }
}
