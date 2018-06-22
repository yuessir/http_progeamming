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
            args = new string[2];
            args[0] = "8086";
            args[1] = @"C:\inetpub\wwwroot";
            try
            {
                SimpleSocketServer webServer;
                int port;
                if (args.Length < 1)
                {
                     port = int.Parse(args[0]);
                     webServer = new SimpleSocketServer(port);
                   

                }
                else
                {
                    port = int.Parse(args[0]);
                    webServer = new SimpleSocketServer(port, args[1]);
                }
                webServer.Run();
               
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid port number");
       
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid port number");
           
            }
            catch (OverflowException)
            {
                Console.WriteLine("Invalid port number");
               
            }



        }
    }
}
