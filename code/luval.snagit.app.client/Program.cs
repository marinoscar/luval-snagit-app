using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luval.snagit.app.client
{
    class Program
    {
        private const string _pipeName = "luval-snagit-app";
        static void Main(string[] args)
        {


            //Client
            var client = new NamedPipeClientStream(_pipeName);
            client.Connect(5000);
            var writer = new StreamWriter(client);

            while (true)
            {
                Console.Write("Enter message: ");
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)) break;
                if (client.IsConnected)
                {
                    writer.WriteLine(input);
                    try
                    {
                        writer.Flush();
                    }
                    catch
                    {
                        Console.WriteLine("Server is unavailable");
                    }
                }
                else
                {
                    Console.WriteLine("Server is unavailable");
                }
            }
        }
    }
}
