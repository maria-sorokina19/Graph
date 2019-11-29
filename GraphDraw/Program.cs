using System;
using Model;

namespace GraphDraw
{
    class Program
    {
        private static Server _server;
        static void Main(string[] args)
        {
            _server = new Server();
            var log = _server.Database.GetLog();
            foreach (var line in log)
            {
                Console.WriteLine(line);
            }
            _server.InitServer();
            Console.ReadKey();
        }
    }
}
