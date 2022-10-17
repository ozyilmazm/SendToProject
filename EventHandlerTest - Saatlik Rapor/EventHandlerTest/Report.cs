using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EventHandlerTest
{
    static class Report
    {
        public static string LogPath, ExePath, ExeName;
        public static void PrintLog(string LogState)
        {
            
            string LogFile = LogPath + "/" + ExeName + "_" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".txt";
            StreamWriter Writer = new StreamWriter(LogFile, true);
            LogState = DateTime.Now + " " + LogState;
            Writer.WriteLine(LogState);
            Writer.Close();
            Writer = null;
        }
        public static void PrintLog_FaultySignals(string LogState)
        {

            string LogFile = LogPath + "/" + ExeName + "_"+ "Faulty_Signals" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + ".txt";
            StreamWriter Writer = new StreamWriter(LogFile, true);
            LogState = DateTime.Now + " " + LogState;
            Writer.WriteLine(LogState);
            Writer.Close();
            Writer = null;
        }
        public static void LogFolders()
        {
            int Index;
            ExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            Index = ExePath.LastIndexOf("\\");
            ExeName = ExePath.Substring(Index + 1, ExePath.Length - Index - 1);
            ExeName = ExeName.Replace(".exe", "");
            ExePath = ExePath.Remove(Index + 1);
            //LogPath = ExePath + ExeName + " Logs";
            LogPath = Environment.GetEnvironmentVariable("SR5_INSTPATH", EnvironmentVariableTarget.Machine).ToString() + "\\Logs\\PE\\" + ExeName + " Logs";

            if (!(Directory.Exists(LogPath)))
            {
                Directory.CreateDirectory(LogPath);
            }
            Console.WriteLine(LogPath + "    Logfile created. ");
        }
    }
}
