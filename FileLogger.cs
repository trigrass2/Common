using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace Common
{
    public class FileLogger
    {
        string path;
        FileStream stream;
        StreamWriter writer;
        Thread dumpThread;
        ConcurrentQueue<LogMessage> logQueue;
        bool running;
        LogLevel logLevel;

        public FileLogger(string filename, LogLevel level)
        {
            this.logLevel = level;
            path = Path.Combine(Common.Utils.StoragePath, filename);
            stream = new FileStream(this.path, FileMode.Append);
            writer = new StreamWriter(stream);
            logQueue = new ConcurrentQueue<LogMessage>();
            Logger.AddQueue(logQueue);


            dumpThread = new Thread(dumpThreadStart);
            running = true;
            dumpThread.Start();
	        Logger.Info("Started file logger in " + path);
        }

        public void Stop()
        {
            this.running = false;
            dumpThread.Join();
        }

        private void dumpThreadStart()
        {
            while (running)
            {
                while (logQueue.Count > 0)
                {
                    LogMessage entry;
                    if (logQueue.TryDequeue(out entry))
                    {
                        if (entry.level > logLevel) continue;
                        string message = entry.timestamp.ToString().PadRight(22) + entry.level.ToString().PadRight(15) + entry.message;
                        writer.WriteLine(message);
                    }
                }
                writer.Flush();
                Thread.Sleep(50);
            }
        }
    }
}
