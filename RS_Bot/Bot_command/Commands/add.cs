using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RS_Bot.Bot_command.Commands
{
    class AddC : Command
    {
        static SqlConnection sql = null;
        public override string[] Names { get; set; } = new string[] { "add" };

        static private void AddNewUserId(string id, string trak)
        {
            //insert into UserData (name, surname) values (N'R', N's')
            SqlCommand command = new SqlCommand(
                $"insert into [UserData] (user_id, tracking) values (N'{id}', N'{trak}')",
                sql);

            Console.WriteLine(command.ExecuteNonQuery().ToString());
        }

        //ВЫвод Select id, name from UserData Where name = 'RS'
        static private bool getData(string id,string trak)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(
                $"Select user_id, tracking from UserData Where user_id = {id}",
                sql
                );

            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            return (dataSet.Tables[0].Rows.Count == 0);
            
        }

       

        public override async void Execute(Message message, TelegramBotClient client, SqlConnection sqlN)
        {
            string name = message.Text.Remove(0,4);
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
