using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Model
{
    public class Database
    {
        private SQLiteConnection _connection;

        private string _pathToDatabase = "db.db";

        public Database()
        {
            _connection = new SQLiteConnection($"Data Source={_pathToDatabase};");

            if (!File.Exists(_pathToDatabase))
            {
                SQLiteCommand command = new SQLiteCommand(Query.CreateDb(), _connection);

                _connection.Open();
                command.ExecuteNonQuery();
                return;
            }

            _connection.Open();
        }

        public int CheckUser(string login, string password)
        {
            SQLiteCommand command = new SQLiteCommand(Query.GetUserInfo(login, password), _connection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }
            return -1;
        }

        public void AddLog(int id, string log)
        {
            SQLiteCommand command = new SQLiteCommand(Query.AddToLog(id, log), _connection);
            Console.WriteLine($"Action: {GetLoginById(id)} at {DateTime.Now} {log}");
            command.ExecuteNonQuery();
        }

        public void RegNewUser(string login, string password)
        {
            SQLiteCommand command = new SQLiteCommand(Query.RegUser(login, password), _connection);
            command.ExecuteNonQuery();
        }

        public bool HasLogin(string login)
        {
            SQLiteCommand command = new SQLiteCommand(Query.HasUser(login), _connection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                return reader.HasRows;
            }
        }

        public string GetLoginById(int id)
        {
            SQLiteCommand command = new SQLiteCommand(Query.GetLogin(id), _connection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetString(0);
                }
            }

            return null;
        }

        public List<string> GetLog()
        {
            var result = new List<string>();
            SQLiteCommand command = new SQLiteCommand(Query.GetLog(), _connection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add($"{GetLoginById(reader.GetInt32(1))} at {reader.GetString(2)} {reader.GetString(3)}");
                }
            }

            return result;
        }

        public void CloseConnection()
        {
            _connection.Close();
        }
    }
}
