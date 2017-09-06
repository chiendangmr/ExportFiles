using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace HDCore
{
    public class Log
    {
        public DateTime LogTime { get; set; }
        public string LogText { get; set; }
    }

    public static class LogProcess
    {
        public static string LogFolder = "";

        private static bool IsWriteLog = false;
        private static bool LogRunning = false;
        private static Queue<Log> lstLog = new Queue<Log>();
        private static void LogThread()
        {
            LogRunning = true;

            StreamWriter writer = null;
            DateTime DateLog = DateTime.Now.Date;
            string logFilePathStart = "";
            string logFilePath = "";
            int extFile = 0;

        TryCreateLogFile:
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);

            if (writer != null)
            {
                try
                {
                    writer.Flush();
                    writer.Close();
                }
                catch { }
            }
            writer = null;
            DateLog = DateTime.Now.Date;
            logFilePathStart = Path.Combine(LogFolder, Process.GetCurrentProcess().ProcessName + "_" + DateLog.ToString("ddMMyyyy") + ".");
            extFile = 0;

            while (writer == null)
            {
                try
                {
                    writer = new StreamWriter(logFilePathStart + (extFile == 0 ? "" : extFile.ToString() + ".") + "log", true);
                    writer.AutoFlush = true;
                    logFilePath = logFilePathStart + (extFile == 0 ? "" : extFile.ToString() + ".") + "log";
                }
                catch
                {
                    writer = null;
                    extFile++;
                }
            }

            do
            {
                Log logFirst = null;
                Monitor.Enter(lstLog);
                while (IsWriteLog && lstLog.Count <= 0)
                {
                    Monitor.Wait(lstLog);
                }
                if (IsWriteLog)
                    logFirst = lstLog.Dequeue();
                Monitor.Exit(lstLog);

                if (logFirst != null)
                {
                    try
                    {
                        writer.WriteLine("[" + logFirst.LogTime.ToString("yyyyMMdd_HHmmss") + "]:" + logFirst.LogText);
                    }
                    catch
                    {
                        try
                        {
                            writer.Flush();
                        }
                        catch { }

                        try
                        {
                            writer.Close();
                        }
                        catch { }

                        goto TryCreateLogFile;
                    }

                    continue;
                }

                if (DateTime.Now.Date != DateLog)
                {
                    try
                    {
                        writer.Flush();
                        writer.Close();

                        writer = null;
                    }
                    catch { }
                    goto TryCreateLogFile;
                }

                try
                {
                    writer.Flush();
                }
                catch { }
            }
            while (IsWriteLog);

            LogRunning = false;
        }

        private static void CompressingThread()
        {
            while (IsWriteLog)
            {
                try
                {
                    if (Directory.Exists(LogFolder))
                    {
                        // Lấy các file log
                        string[] lstLogFile = Directory.GetFiles(LogFolder, "*.log");
                        foreach (var logFile in lstLogFile)
                        {
                            try
                            {
                                string fileName = Path.GetFileNameWithoutExtension(logFile).Substring((Process.GetCurrentProcess().ProcessName + "_").Length, 8);
                                DateTime logDate = DateTime.ParseExact(fileName, "ddMMyyyy", null);
                                if (logDate < DateTime.Now.Date.AddDays(-1))
                                {
                                    FileStream sourceFileStream = File.OpenRead(logFile);
                                    FileStream destFileStream = File.Create(logFile + ".gz");

                                    GZipStream compressingStream = new GZipStream(destFileStream,
                                        CompressionMode.Compress);

                                    byte[] bytes = new byte[2048];
                                    int bytesRead;
                                    while ((bytesRead = sourceFileStream.Read(bytes, 0, bytes.Length)) != 0)
                                    {
                                        compressingStream.Write(bytes, 0, bytesRead);
                                    }

                                    sourceFileStream.Close();

                                    compressingStream.Flush();
                                    compressingStream.Close();

                                    File.Delete(logFile);
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }

                for (int time = 0; IsWriteLog && time < 5000; time += 100)
                    Thread.Sleep(100);
            }
        }

        public static void Start()
        {
            if (!LogRunning)
            {
                IsWriteLog = true;

                Thread thrCompressing = new Thread(CompressingThread);
                thrCompressing.IsBackground = true;
                thrCompressing.Start();

                Thread thrLog = new Thread(LogThread);
                thrLog.IsBackground = true;
                thrLog.Start();
            }
        }

        public static void Stop()
        {
            IsWriteLog = false;

            Monitor.Enter(lstLog);
            Monitor.PulseAll(lstLog);
            Monitor.Exit(lstLog);
        }

        public static bool IsRun()
        {
            return LogRunning;
        }

        public static void AddLog(string mess)
        {
            try
            {
                Monitor.Enter(lstLog);
                lstLog.Enqueue(new Log() { LogTime = DateTime.Now, LogText = mess });
                Monitor.PulseAll(lstLog);
                Monitor.Exit(lstLog);
            }
            catch { }
        }
    }
}
