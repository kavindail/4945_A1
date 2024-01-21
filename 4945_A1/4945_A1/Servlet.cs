using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4945_A1
{
    public abstract class Servlet
    {
        public abstract void doGet(HttpRequest request, HttpResponse response);
        public abstract void doPost(HttpRequest request, HttpResponse response);
    }
}
