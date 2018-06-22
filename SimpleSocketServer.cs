using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
       private string httpRoot;
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSocketServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public SimpleSocketServer(int port,string httpRoot="")
       {
           server=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
           server.Bind(new IPEndPoint(IPAddress.Loopback, port));
           this.httpRoot = httpRoot;
       }

       public void Run()
       {
           server.Listen(8086);
           Console.WriteLine("Waiting new request.");
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
           Console.WriteLine("---------------------");
            string line;
           do
           {
               line = SocketRead(socket);
               if (line!=null)
               {
                   Console.WriteLine(line);
               }
           } while (!string.IsNullOrEmpty(line));


           string[] tok = first.Split(' ');
           string vaerb = tok[0];
           string path = tok[1];
           string version = tok[2];
           if (String.Compare(vaerb,"GET",StringComparison.OrdinalIgnoreCase)==0)
           {
               SendFile(socket, path);
           }
           else
           {
               Error(socket, 500, "Unsupported command");
           }
            //NewMethod(socket);

            socket.Close();
       }
        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="path">The path. The request path.</param>
        private void SendFile(Socket socket, string path)
       {
           // parse the file by /'s and build a local file
            string[] tok = path.Split('/');
           Console.WriteLine(path);
           StringBuilder physicalPath = new StringBuilder(httpRoot);
           AddSlash(physicalPath);
           foreach (var e in tok)
           {
               if (!e.Trim().Equals("\\"))
               {
                   if (e.Equals("..") || e.Equals("."))
                   {
                       Error(socket, 500, "Invalid request");
                       return;
                    }
                   AddSlash(physicalPath);
                   physicalPath.Append(e);

                }
           }
            // if there is no file specified, default to index.html
           if (!physicalPathAnyfile(physicalPath))
           {
               physicalPath.Append("index.html");
           }

           string fileName = physicalPath.ToString();
           //open "indexl.html" file and send it to cilent
           FileInfo file= new FileInfo(fileName);
           if (file.Exists)
           {
               //buffer is the html's body
               FileStream fis = File.Open(fileName, FileMode.Open);
               byte[] buffer= new byte[(int)file.Length];
               fis.Read(buffer, 0, buffer.Length);
               fis.Close();
               this.Transmit(socket,200,"OK",buffer,GetContent(fileName));
           }
           else
           {
               this.Error(socket,404,"File NOt Found");
           }
       }

       private string GetContent(string path)
       {
           path = path.ToLower();
           if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
               return "image/jpeg";
           else if (path.EndsWith(".gif"))
               return "image/gif";
           else if (path.EndsWith(".png"))
               return "image/png";
           else return "text/html";
        }

       private bool physicalPathAnyfile(StringBuilder physicalPath)
       {
           if (physicalPath.ToString().EndsWith("\\"))
           {
               return false;
           }
           return true;
       }

       private void Error(Socket socket, int code, String message)
       {
           StringBuilder body = new StringBuilder();
           body.Append("<html><head><title>");
           body.Append(code + ":" + message);
           body.Append("</title></head><body><p>An error occurred.</p><h1>");
           body.Append(code);
           body.Append("</h1><p>");
           body.Append(message);
           body.Append("</p></body></html>");
           Transmit(socket,code, message, Encoding.ASCII.GetBytes(body.ToString()), "text/html") ;
       }
        /// <summary>
        /// Transmits the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="body">The body.</param>
        /// <param name="content">The content.</param>
        private void Transmit(Socket socket, int code, string message, byte[] body, string content)
       {
          StringBuilder headers=new StringBuilder();
           headers.Append("HTTP/1.1 "); headers.Append(code); headers.Append(' '); headers.Append(message);
           headers.Append("\n");
           headers.Append("Content-Length: " + body.Length + "\n");
           headers.Append("Server: Demo Example Server\n");
           headers.Append("Connection: close\n");
           headers.Append("Content-Type: " + content + "\n");
           headers.Append("\n");

           socket.Send(Encoding.ASCII.GetBytes(headers.ToString()));
           socket.Send(body);
        }

       private void AddSlash(StringBuilder physicalPath)
       {
           if (!physicalPath.ToString().EndsWith("\\"))
           {
               physicalPath.Append("\\");
           }
       }

    
        private void ToHelloResponse(Socket socket)
        {
            //to http response
            SocketWrite(socket, "HTTP/1.1 200 OK");
            SocketWrite(socket, "");
            SocketWrite(socket, "<html>");
            SocketWrite(socket, "<head><title>    Simple Web Server</title></head>");
            SocketWrite(socket, "<body>");
            SocketWrite(socket, "<h1>Hello World</h1>");
            SocketWrite(socket, "<//body>");
            SocketWrite(socket, "</html>");
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
