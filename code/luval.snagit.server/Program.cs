using luval.snagit.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace luval.snagit.server
{
    class Program
    {
        private const string _pipeName = "luval-snagit-app";
        private static string _variable = "START";
        static void Main(string[] args)
        {
            var snag = new SnagIt();
            snag.StartRecording();
            var stream = new Server().Start(_pipeName);
            Thread.Sleep(5000);
            while (true)
            {

                var line = stream.ReadLine();
                _variable = line;
                Console.WriteLine("Recieved: {0}", _variable);
                Thread.Sleep(1000);
                if(line == "STOP")
                {
                    snag.StopRecording();
                }
                var duration = DateTime.UtcNow.Subtract(snag.UtcRecordStartTime).TotalMinutes;
                if(duration > 5)
                {
                    snag.StopRecording();
                }
            }
        }
    }
}
