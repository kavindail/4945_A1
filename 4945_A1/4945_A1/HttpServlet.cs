using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;


namespace _4945_A1 {
    public abstract class HttpServlet : Servlet
    {
        protected int port;
        private TcpListener listener;
        private bool isActive = true;

        public HttpServlet(int port)
        {
            this.port = port;
        }

        public void Listen()
        {
            listener = new TcpListener(port);
            listener.Start();

            while (isActive)
            {
                TcpClient client = listener.AcceptTcpClient();

                HttpProcessor processor = new HttpProcessor(client, this);
                Thread thread = new Thread(processor.Process);
                thread.Start();
            }
        }

        public abstract override void doGet(HttpRequest request, HttpResponse response);
        public abstract override void doPost(HttpRequest request, HttpResponse response);

    }
}