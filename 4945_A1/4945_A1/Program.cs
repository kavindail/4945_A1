using System;
using System.Threading;

namespace _4945_A1
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            HttpServlet s = new MyHttpServlet(8080);
            Thread thread = new Thread(new ThreadStart(s.listen));
            thread.Start();
        }
    }
}