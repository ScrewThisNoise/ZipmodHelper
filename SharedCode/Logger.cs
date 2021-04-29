using System;
using System.IO;
using System.Text;

namespace ScrewLib
{
    public static class Logger
    {
        private static StreamWriter LoggerFile { get; set; }
        private static string Date { get; set; }

        public static void Initiate()
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            Logger.Date = DateTime.Now.ToString("yyMMdd");

            try
            {
                if (!Directory.Exists("Logs"))
                    Directory.CreateDirectory("Logs");

                LoggerFile = new StreamWriter($"Logs\\{Date}.log", true, Encoding.UTF8) { AutoFlush = true };
                LoggerFile.WriteLine($"{time} : Initiated logging: {Date}.log");
                Console.WriteLine($"{time} : Initiated logging: {Date}.log");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Initiate(string LogName)
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            try
            {
                if (!Directory.Exists("Logs"))
                    Directory.CreateDirectory("Logs");

                LoggerFile = new StreamWriter($"Logs\\{LogName}.log") { AutoFlush = true };
                LoggerFile.WriteLine($"{time} : Initiated logging: {LogName}.log");
                Console.WriteLine($"{time} : Initiated logging: {LogName}.log");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Writer(string LogMessage)
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            string currentdate = DateTime.Now.ToString("yyMMdd");
            if (currentdate != Logger.Date)
            {
                Logger.Destroy();
                Logger.Initiate();
            }
            try
            {
                LoggerFile.WriteLine($"{time} : {LogMessage}");
                Console.WriteLine($"{time} : {LogMessage}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Destroy()
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            try
            {
                LoggerFile.Close();
                Console.WriteLine($"{time} : Closed Logging.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}