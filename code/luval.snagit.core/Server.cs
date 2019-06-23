using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace luval.snagit.core
{
    public class Server
    {
        public StreamReader Start(string pipeName)
        {
            var server = new NamedPipeServerStream(pipeName);
            server.WaitForConnection();
            return new StreamReader(server);
        }
    }
}
