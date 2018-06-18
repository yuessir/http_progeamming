using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWebServer
{
   public class SimpleSocketServer
   {
       private Socket server;
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSocketServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public SimpleSocketServer(int port)
       {
           server=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
           server.Bind(new IPEndPoint(IPAddress.Loopback, port));

       }

       public void Run()
       {
           server.Listen(8086);
           for (;;)
           {
               Socket socket = server.Accept();
               HandleClientSesion(socket);
           }
       }

       private void HandleClientSesion(Socket socket)
       {
           // the ﬁrst line of an HTTP request has a special format
           Console.WriteLine($">>>>new http request.....");
           string first = SocketRead(socket);
           Console.WriteLine(first);

           string line;
           do
           {
               line = SocketRead(socket);
               if (line!=null)
               {
                   Console.WriteLine(line);
               }
           } while (!string.IsNullOrEmpty(line));


           //to http response
           SocketWrite(socket, "HTTP/1.1 200 OK");
           SocketWrite(socket, "");
           SocketWrite(socket, "<html>");
           SocketWrite(socket, "<head><title>    Simple Web Server</title></head>");
           SocketWrite(socket, "<body>");
           SocketWrite(socket, "<h1>Hello World</h1>");
           SocketWrite(socket, "<//body>");
           SocketWrite(socket, "</html>");

           socket.Close();
       }

       private void SocketWrite(Socket socket, string str)
       {
           ASCIIEncoding encoding=new ASCIIEncoding();
           socket.Send(encoding.GetBytes(str));
           socket.Send(encoding.GetBytes("\r\n"));

       }

       private string SocketRead(Socket socket)
       {
           StringBuilder result=new StringBuilder();
           byte[] buffer = new byte[1];
           while (socket.Receive(buffer)>0)
           {
               char ch = (char) buffer[0];
               if (ch=='\n')
               {
                   break;
               }

               if (ch!='\r')
               {
                   result.Append(ch);
               }
           }
           return result.ToString();
       }
   }
}
