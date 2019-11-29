using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    internal static class Query
    {
        public static string CreateDb()
        {
            return @"CREATE TABLE [user](
                [id] INTEGER PRIMARY KEY AUTOINCREMENT,
                [login] TEXT NOT NULL, 
                [password] TEXT NOT NULL);

            CREATE TABLE[log](

                [id] INTEGER PRIMARY KEY AUTOINCREMENT,

                [id_user] INTEGER NOT NULL REFERENCES[user]([id]), 
                [time] DATETIME NOT NULL, 
                [description] TEXT NOT NULL);

               insert into user (login, password) values ('admin', '123');
            ";
        }

        public static string RegUser(string login, string password)
        {
            return $"insert into user(login, password) values('{login}', '{password}')";
        }

        public static string GetUserInfo(string login, string password)
        {
            return $@"select * from user where login = ""{login}"" and password = ""{password}""";
        }

        public static string AddToLog(int idUser, string message)
        {
            return
                $@"insert into log(id_user, time, description) values ({idUser}, '{DateTime.Now.ToString("g")}', '{message}')";
        }

        public static string HasUser(string login)
        {
            return $"select * from user where login ='{login}'";
        }

        public static string GetLog()
        {
            return $"select * from log";
        }

        public static string GetLogin(int id)
        {
            return $"select login from user where id = {id}";
        }
    }
}
