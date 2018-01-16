﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace HDControl
{
    //public class Log
    //{
    //    public DateTime LogTime { get; set; }
    //    public string LogText { get; set; }
    //}

    //public static class LogProcess
    //{
    //    public static string LogFolder = "";

    //    private static bool IsWriteLog = false;
    //    private static bool LogRunning = false;
    //    private static List<Log> lstLog = new List<Log>();
    //    private static void LogThread()
    //    {
    //        LogRunning = true;

    //        StreamWriter writer = null;
    //        DateTime DateLog = DateTime.Now.Date;
    //        string logFilePathStart = "";
    //        string logFilePath = "";
    //        int extFile = 0;

    //    TryCreateLogFile:
    //        if (!Directory.Exists(LogFolder))
    //            Directory.CreateDirectory(LogFolder);

    //        if (writer != null)
    //        {
    //            try
    //            {
    //                writer.Flush();
    //                writer.Close();
    //            }
    //            catch { }
    //        }
    //        writer = null;
    //        DateLog = DateTime.Now.Date;
    //        logFilePathStart = Path.Combine(LogFolder, Process.GetCurrentProcess().ProcessName + "_" + DateLog.ToString("ddMMyyyy") + ".");
    //        extFile = 0;

    //        while (writer == null)
    //        {
    //            try
    //            {
    //                writer = new StreamWriter(logFilePathStart + (extFile == 0 ? "" : extFile.ToString() + ".") + "log", true);
    //                writer.AutoFlush = true;
    //                logFilePath = logFilePathStart + (extFile == 0 ? "" : extFile.ToString() + ".") + "log";
    //            }
    //            catch
    //            {
    //                writer = null;
    //                extFile++;
    //            }
    //        }

    //        do
    //        {
    //            while (IsWriteLog && lstLog.Count == 0 && DateTime.Now.Date == DateLog)
    //                Thread.Sleep(100);

    //            while (lstLog.Count > 0)
    //            {
    //                try
    //                {
    //                    Log log = lstLog[0];
    //                    writer.WriteLine("[" + log.LogTime.ToString("yyyyMMdd_HHmmss") + "]:" + log.LogText);
    //                    lock (lstLog)
    //                    {
    //                        lstLog.RemoveAt(0);
    //                    }
    //                }
    //                catch
    //                {
    //                    try
    //                    {
    //                        writer.Flush();
    //                    }
    //                    catch { }

    //                    try
    //                    {
    //                        writer.Close();
    //                    }
    //                    catch { }

    //                    goto TryCreateLogFile;
    //                }
    //            }

    //            if (DateTime.Now.Date != DateLog)
    //            {
    //                try
    //                {
    //                    writer.Flush();
    //                    writer.Close();

    //                    FileStream sourceFileStream = File.OpenRead(logFilePath);
    //                    FileStream destFileStream = File.Create(logFilePath + ".gz");

    //                    GZipStream compressingStream = new GZipStream(destFileStream,
    //                        CompressionMode.Compress);

    //                    byte[] bytes = new byte[2048];
    //                    int bytesRead;
    //                    while ((bytesRead = sourceFileStream.Read(bytes, 0, bytes.Length)) != 0)
    //                    {
    //                        compressingStream.Write(bytes, 0, bytesRead);
    //                    }

    //                    sourceFileStream.Close();
    //                    compressingStream.Close();
    //                    destFileStream.Close();

    //                    File.Delete(logFilePath);
    //                }
    //                catch { }
    //                goto TryCreateLogFile;
    //            }

    //            try
    //            {
    //                writer.Flush();
    //            }
    //            catch { }
    //        }
    //        while (IsWriteLog);

    //        LogRunning = false;
    //    }

    //    public static void Start()
    //    {
    //        if (!LogRunning)
    //        {
    //            IsWriteLog = true;

    //            Thread thrLog = new Thread(LogThread);
    //            thrLog.IsBackground = true;
    //            thrLog.Start();
    //        }
    //    }

    //    public static void Stop()
    //    {
    //        IsWriteLog = false;
    //    }

    //    public static bool IsRun()
    //    {
    //        return LogRunning;
    //    }

    //    public static void AddLog(string mess)
    //    {
    //        try
    //        {
    //            lock (lstLog)
    //            {
    //                lstLog.Add(new Log() { LogTime = DateTime.Now, LogText = mess });
    //            }
    //        }
    //        catch { }
    //    }
    //}
}
