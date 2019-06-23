using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luval.snagit.app
{
    class Program
    {
        private const string _pipeName = "luval-snagit-app";
        static void Main(string[] args)
        {
            StartServer();
            Task.Delay(1000).Wait();


            //Client
            var client = new NamedPipeClientStream(_pipeName);
            client.Connect();
            var reader = new StreamReader(client);
            var writer = new StreamWriter(client);

            while (true)
            {
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)) break;
                writer.WriteLine(input);
                writer.Flush();
                Console.WriteLine(reader.ReadLine());
            }
        }

        static void StartServer()
        {
            Task.Factory.StartNew(() =>
            {
                var server = new NamedPipeServerStream(_pipeName);
                server.WaitForConnection();
                var reader = new StreamReader(server);
                var writer = new StreamWriter(server);
                while (true)
                {
                    var line = reader.ReadLine();
                    writer.WriteLine("Recieved: {0}", line);
                    writer.Flush();
                }
            });
        }
    }

}
