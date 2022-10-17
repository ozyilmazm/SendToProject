using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.CdaAlarming;
using Interop.ScadaScripting;

namespace AlarmCounter
{
    class Program
    {

        enum AlarmSummary
        {
            Abnormal_Summary_ = 1,
            E_Alarm_Summary_1 = 3,
            E_Alarm_Summary_2 = 4,
            E_Alarm_Summary_3 = 5,
            E_General_Summary = 6,
            G_Alarm_Prio_1 = 7,
            G_Alarm_Prio_2 = 8,
            G_General_Summary = 9,
            S_System_Alarms = 13,
            S_System_Summary = 14,
            W_Alarm_Prio_1 = 15,
            W_Alarm_Prio_2 = 16,
            W_General_Summary = 17,
            Replay_General_Summary = 19
        }

        public static CxAlarmingEvent xAlarm;
        public static Dictionary<string, Signal> CountofAlarm;
        public static int limit { get; set; }
        public static bool auto { get; set; }
        static void Main(string[] args)
        {

            IxScriptComponent m_ScriptingComponent = new ScadaScriptComponent();
            m_ScriptingComponent.ContextName = "RT";
            m_ScriptingComponent.UserName = Environment.UserName;
            m_ScriptingComponent.ConsoleName = Environment.MachineName;
            m_ScriptingComponent.SetActive(1);
            Report.LogFolders();


            DateTime StartTime = DateTime.Now;
            //limit = 50;
            //auto = true;

            CountofAlarm = new Dictionary<string, Signal>();
            ReadAlarm();
            

            string exit = Console.ReadLine();
            if (exit.ToLower() == "c")
            {
                Report.PrintLog($"Programın başlatıldığı zaman : {StartTime} Programın sonlandırıldığı zaman : {DateTime.Now}  Geçen Süre : {(DateTime.Now - StartTime).TotalMinutes} dakika");
                foreach (var item in CountofAlarm)
                {
                    Report.PrintLog($"Signal Path = {item.Value.digital.ObjectPath}   Signal Guid : {item.Value.objid}  Number Of Signal : {item.Value.count}");
                }
            }
            
        }

        static void ReadAlarm()
        {
            xAlarm = new CxAlarmingEvent();
            xAlarm.AlarmChanged += XAlarm_AlarmChanged;     
               

            xAlarm.AddNotificationStatus(AlarmStatus.asAppearing);
            xAlarm.AddNotificationSummary((int)AlarmSummary.E_General_Summary);
            xAlarm.EnableNotifications();
        }

        private static void XAlarm_AlarmChanged(AlarmType type, AlarmStatus status)
        {

            if (xAlarm.Indicators == "Spont")
            {
                Console.WriteLine(xAlarm.ScadaPath);
                Console.WriteLine(xAlarm.TimeStamp.ToString() + "." + xAlarm.MilliSeconds.ToString());
                try
                {
                    string guid = xAlarm.ObjectId.ToString();
                    try
                    {
                        Signal signal = new Signal(guid);
                        CountofAlarm.Add(guid, signal);
                        Console.WriteLine($"new signal has been added {guid}");
                    }
                    catch (ArgumentException)
                    {

                        if (CountofAlarm.TryGetValue(guid, out Signal value))
                        {
                            value.count++;
                            Console.WriteLine($"{value.digital.ObjectPath} :   Count : {value.count}");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

        }
    }
}
