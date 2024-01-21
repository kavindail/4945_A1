using System;
using System.Threading;

namespace _4945_A1
{
    internal class Program {
        public static void Main(string[] args)
        {
            Server server = new Server(8080);
            server.Start();
        }
    }
}