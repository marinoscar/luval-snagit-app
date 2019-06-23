using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace luval.snagit.server
{
    class Program
    {
        private const string _pipeName = "luval-snagit-app";
        private static string _variable = "START";
        static void Main(string[] args)
        {
            StartServer();
            Task.Delay(1000).Wait();


            //Client
            //var client = new NamedPipeClientStream(_pipeName);
            //client.Connect();
            //var reader = new StreamReader(client);
            while(_variable != "END")
            {
                Thread.Sleep(1000);
            }
        }

        static void StartServer()
        {
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream(_pipeName);
                server.WaitForConnection();
                var reader = new StreamReader(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    _variable = line;
                    Console.WriteLine("Recieved: {0}", _variable);
                }
            });
        }
    }
}
