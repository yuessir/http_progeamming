using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SocketWebServer
{
    class Program
    {

        static void Main(string[] args)
        {
            SimpleSocketServer webServer = new SimpleSocketServer(8086);
            webServer.Run();

        }
    }
}
