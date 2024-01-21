using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4945_A1
{
    public class Server
    {
        private TcpListener listener;
        private bool isRunning;

        public Server(int port)
        {
            listener = new TcpListener(port);
        }

        public void Start()
        {
            isRunning = true;
            listener.Start();

            while (isRunning)
            {
                TcpClient client = listener.AcceptTcpClient();
                ServerThread thread = new ServerThread(client, new FileUploadServlet());
                new Thread(new ThreadStart(thread.Run)).Start();
            }
        }
    }
}
