using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EventHandlerTest
{
    class Program
    {
        public static Timer EventTimer;
        public static List<Threadsx> threads_analog = new List<Threadsx>();
        public static List<Threadsx> threads_digital = new List<Threadsx>();

        static void Main(string[] args)
        {
            int NumberOfAnalogThread, NumberOfDigitalThread, NumberOfSignals_Analog, NumberOfSignals_Digital;
            Report.LogFolders();
            Doi.GetAll("Net-E");

            Console.WriteLine("Added digital: {0}\n", Doi.Digital_signals.Count());
            Report.PrintLog($"Added digital: {Doi.Digital_signals.Count()} \n");
            Console.WriteLine("Not Added digital: {0}\n", Doi.not_added_digital);
            Report.PrintLog($"Not Added digital: {Doi.not_added_digital}\n");
            Console.WriteLine("Added analog: {0}\n", Doi.Analog_signals.Count());
            Report.PrintLog($"Added analog: {Doi.Analog_signals.Count()}\n");
            Console.WriteLine("Not Added analog: {0}\n", Doi.not_added_analog);
            Report.PrintLog($"Not Added analog: {Doi.not_added_analog}\n");


            NumberOfSignals_Analog = 5000;
            NumberOfSignals_Digital = 5000;
            Dictionary<string, ScadaSignal> temp = new Dictionary<string, ScadaSignal>();
            NumberOfAnalogThread = Doi.Analog_signals.Count() / NumberOfSignals_Analog;
            NumberOfDigitalThread = Doi.Digital_signals.Count() / NumberOfSignals_Digital;



            #region Analog
            for (int i = 0; i <= NumberOfAnalogThread; i++)
            {

                if (NumberOfSignals_Analog < (Doi.Analog_signals.Count() - (i) * NumberOfSignals_Analog))
                {
                    Console.WriteLine($"Group No {i}");
                    for (int j = 0; j < NumberOfSignals_Analog; j++)
                    {
                        temp.Add(Doi.Analog_signals.ElementAt(i * NumberOfSignals_Analog + j).Key, Doi.Analog_signals.ElementAt(i * NumberOfSignals_Analog + j).Value);
                    }
                    threads_analog.Add(new Threadsx(temp, "analog"));
                }
                else
                {
                    Console.WriteLine("Son Grup");

                    for (int j = 0; j < (Doi.Analog_signals.Count() - (i) * NumberOfSignals_Analog); j++)
                    {
                        temp.Add(Doi.Analog_signals.ElementAt(i * NumberOfSignals_Analog + j).Key, Doi.Analog_signals.ElementAt(i * NumberOfSignals_Analog + j).Value);
                    }
                    threads_analog.Add(new Threadsx(temp, "analog"));
                }
                temp.Clear();
            }

            foreach (var item in threads_analog)
            {
                item.CreateThreadsAnalog();
                
            }
            #endregion
            #region Digital
            for (int i = 0; i <= NumberOfDigitalThread; i++)
            {

                if (NumberOfSignals_Digital < (Doi.Digital_signals.Count() - (i) * NumberOfSignals_Digital))
                {
                    Console.WriteLine($"Digital Group No {i}");
                    Report.PrintLog($"Digital Group No {i}");
                    for (int j = 0; j < NumberOfSignals_Digital; j++)
                    {
                        temp.Add(Doi.Digital_signals.ElementAt(i * NumberOfSignals_Digital + j).Key, Doi.Digital_signals.ElementAt(i * NumberOfSignals_Digital + j).Value);
                    }
                    threads_digital.Add(new Threadsx(temp, "digital"));
                }
                else
                {
                    Console.WriteLine("Digital Son Grup");
                    Report.PrintLog("Digital Son Grup");

                    for (int j = 0; j < (Doi.Digital_signals.Count() - (i) * NumberOfSignals_Digital); j++)
                    {
                        temp.Add(Doi.Digital_signals.ElementAt(i * NumberOfSignals_Digital + j).Key, Doi.Digital_signals.ElementAt(i * NumberOfSignals_Digital + j).Value);
                    }
                    threads_digital.Add(new Threadsx(temp, "Digital"));
                }
                temp.Clear();
            }

            foreach (var item in threads_digital)
            {
                item.CreateThreadsDigital();
            }
            #endregion
            EventTimer = new Timer(60000);
            EventTimer.Elapsed += OnTimedEvent;
            EventTimer.Enabled = true;
            #region End of Program
            string end = Console.ReadLine().ToLower();


            if (end == "c")
            {
                foreach (var item in threads_analog)
                {
                    item.StopMyThreadsAnalog();
                }
                foreach (var item in threads_digital)
                {
                    item.StopMyThreadsDigital();
                }
            }
            #endregion


        }
        public static void OnTimedEvent(object Source, ElapsedEventArgs e)
        {
            EventTimer.Interval = 1 * 60 * 60 * 1000; // Hour*Minute*Second*MilliSecond
            TimeSpan elapsed_time_periodic;
            int cnt = 0;
            foreach (var item in threads_analog)
            {
                cnt++;
                elapsed_time_periodic = item.EventHandler_Analog.EndTime - item.EventHandler_Analog.Begin_time_Periodic;
                Console.WriteLine((item.EventHandler_Analog.EventCount_Periodic) + " of analog signals has been changed during " + (elapsed_time_periodic.TotalSeconds) + " seconds. Change per second is: " + (elapsed_time_periodic.TotalSeconds) + " Thread : " + cnt);
                Report.PrintLog((item.EventHandler_Analog.EventCount_Periodic) + " of analog signals has been changed during " + (elapsed_time_periodic.TotalSeconds) + " seconds. Change per second is: " + (elapsed_time_periodic.TotalSeconds) + " Thread : " + cnt);
                item.EventHandler_Analog.EventCount_Periodic = 0;
                item.EventHandler_Analog.Begin_time_Periodic = DateTime.Now;
            }
            cnt = 0;


            foreach (var item in threads_digital)
            {
                cnt++;
                elapsed_time_periodic = item.EventHandler_Digital.EndTime - item.EventHandler_Digital.Begin_time_Periodic;
                Console.WriteLine((item.EventHandler_Digital.EventCount_Periodic) + " of Digital signals has been changed during " + (elapsed_time_periodic.TotalSeconds) + " seconds. Change per second is: " + (elapsed_time_periodic.TotalSeconds) + " Thread : " + cnt);
                Report.PrintLog((item.EventHandler_Digital.EventCount_Periodic) + " of Digital signals has been changed during " + (elapsed_time_periodic.TotalSeconds) + " seconds. Change per second is: " + (elapsed_time_periodic.TotalSeconds) + " Thread : " + cnt);
                item.EventHandler_Digital.EventCount_Periodic = 0;
                item.EventHandler_Digital.Begin_time_Periodic = DateTime.Now;
            }


        }

    }
}
