using System;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandlerTest
{
    class Threadsx
    {
       
        public EventHandler_Analog EventHandler_Analog { get; set; }
        public EventHandler_Digital EventHandler_Digital { get; set; }
        int count, total_count_analog, total_count_digital;
        List<Thread> lstThreadsAnalogs = new List<Thread>();
        List<Thread> lstThreadsDigitals = new List<Thread>();
        Thread th;
        Dictionary<string, ScadaSignal> signals_analog = new Dictionary<string, ScadaSignal>();
        Dictionary<string, ScadaSignal> signals_digital = new Dictionary<string, ScadaSignal>();
        public int EventCount { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public Threadsx(Dictionary<string, ScadaSignal> signal_list, string datatype) //temp dictionary'sini threadlerin kullanacağı signal list dictionarylerine çevirme
        {

            if (datatype.ToLower() == "analog")
            {
                Console.WriteLine("analog işleniyor");
                foreach (var item in signal_list)
                {
                    signals_analog.Add(item.Key, item.Value);
                }
            }
            if (datatype.ToLower() == "digital")
            {
                Console.WriteLine("digital işleniyor");
                foreach (var item in signal_list)
                {
                    signals_digital.Add(item.Key, item.Value);
                }
            }

        }
       
        #region Analog

        public void CreateThreadsAnalog()

        {
            th = new Thread(() => { ThreadAnalog(signals_analog); });
            th.Start();
        } //Thread analog metoduna signals_analog dictionary'sini göndererek yeni bir thread oluşturur ve onu thread listesine alır

        private void ThreadAnalog(Dictionary<string, ScadaSignal> signals)//yeni bir eventhandler analog objesi oluşturur ve prepare eventhandler metoduna dictionary gönderir

        {
            EventHandler_Analog = new EventHandler_Analog();
            EventHandler_Analog.PrepareEventHandler(signals);

        }

        public void StopMyThreadsAnalog() //threadi durdurur

        {
            EventHandler_Analog.ReleaseEventHandler();
            Console.WriteLine($"Thread Number: { th.ManagedThreadId} :::::::: {(EventHandler_Analog.EventCount)} of Analog signals has been changed during {(EventHandler_Analog.elapsed_time.TotalSeconds)} seconds. Change per second is:{ (EventHandler_Analog.EventCount / EventHandler_Analog.elapsed_time.TotalSeconds)}");
            Report.PrintLog($"Thread Number: { th.ManagedThreadId} :::::::: {(EventHandler_Analog.EventCount)} of Analog signals has been changed during {(EventHandler_Analog.elapsed_time.TotalSeconds)} seconds. Change per second is:{ (EventHandler_Analog.EventCount / EventHandler_Analog.elapsed_time.TotalSeconds)}");
        }
        #endregion


        #region Digital

        public void CreateThreadsDigital()//Thread digital metoduna signals_analog dictionary'sini göndererek yeni bir thread oluşturur ve onu thread listesine alır

        {

            th = new Thread(() => { ThreadDigital(signals_digital); });
            th.Start();
        }
        private void ThreadDigital(Dictionary<string, ScadaSignal> signals)//yeni bir eventhandler digital objesi oluşturur ve prepare eventhandler metoduna dictionary gönderir

        {
            EventHandler_Digital = new EventHandler_Digital();
            EventHandler_Digital.PrepareEventHandler(signals);
            Console.WriteLine("Event Handler Digital Passed");

        }
        public void StopMyThreadsDigital()//Thread digital metoduna signals_digital dictionary'sini göndererek yeni bir thread oluşturur ve onu thread listesine alır

        {
            EventHandler_Digital.ReleaseEventHandler();
            Console.WriteLine($"Thread Number: { th.ManagedThreadId} :::::::: {(EventHandler_Digital.EventCount)} of Digital signals has been changed during {(EventHandler_Digital.elapsed_time.TotalSeconds)} seconds. Change per second is:{ (EventHandler_Digital.EventCount / EventHandler_Digital.elapsed_time.TotalSeconds)}");
            Report.PrintLog($"Thread Number: { th.ManagedThreadId} :::::::: {(EventHandler_Digital.EventCount)} of Digital signals has been changed during {(EventHandler_Digital.elapsed_time.TotalSeconds)} seconds. Change per second is:{ (EventHandler_Digital.EventCount / EventHandler_Digital.elapsed_time.TotalSeconds)}");
            th.Abort();
        }
        #endregion

        
    }
    
}
