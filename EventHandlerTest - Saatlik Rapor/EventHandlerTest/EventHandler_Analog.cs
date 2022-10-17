using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Interop.ScadaScripting;




namespace EventHandlerTest
{
    class EventHandler_Analog
    {
        bool FirstEnter = true;
        private ScadaEventList ElectricEvent;
        public string msg_eventhandler;
        public int EventCount { get; set; }
        public int EventCount_Periodic { get; set; }
        int FirstInitCount = 0;
        private List<string> signal_info = new List<string>();
        public DateTime BeginTime, EndTime;
        public DateTime Begin_time_Periodic { get; set; }
        public TimeSpan elapsed_time { get; set; }
        private Dictionary<string, ScadaSignal> signal_list;


        public void PrepareEventHandler(Dictionary<string, ScadaSignal> signals)

        {
            try
            {
                this.signal_list = signals;
                this.ElectricEvent = new ScadaEventListClass();

                this.ElectricEvent.ValueChanged += new _IxScadaEventListEvents_ValueChangedEventHandler(this.Eventlist_ValueChanged);
                foreach (var xSignal in signal_list.Values)
                {
                    this.ElectricEvent.Add(xSignal.analogMeasurement);
                }
            }
            catch (Exception e)
            {
                Report.PrintLog("Prepare Event Error " + e.ToString());
            }
        }
        private void Eventlist_ValueChanged(object Measurement, DateTime TimeStamp, short Milliseconds)
        {
            string qual;
            ScadaSignal xSignal;
            try
            {
                AnalogMeasurement AnalogEventObject = (AnalogMeasurement)Measurement;
                Doi.Analog_signals.TryGetValue(AnalogEventObject.ObjectIdStr, out xSignal);
                qual = SignalQuality(AnalogEventObject);

                if (xSignal.FirstInitialize == true)
                {
                    xSignal.FirstInitialize = false;
                    FirstInitCount++;
                    if (FirstInitCount == signal_list.Count())
                    {
                        Console.WriteLine($"\n BEGIN THE SIMULATION OF ANALOG SIGNALS Thread Number : {Thread.CurrentThread.ManagedThreadId}");
                    }
                }
                else
                {
                    if (FirstEnter)
                    {
                        FirstEnter = false;
                        BeginTime = DateTime.Now;
                        Begin_time_Periodic = DateTime.Now;

                        Console.WriteLine("//////////////////////TEST STARTED////////////////////////ANALOGS {0}", BeginTime);
                        Report.PrintLog($"//////////////////////TEST STARTED////////////////////////ANALOGS {BeginTime}");
                        
                    }
 
                    EventCount++;
                    EventCount_Periodic++;
                    EndTime = DateTime.Now;
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Event Error " + e.ToString());
            }
        }
        public void ReleaseEventHandler()
        {
            elapsed_time = EndTime - BeginTime;
            try
            {
                ElectricEvent.ValueChanged -= new _IxScadaEventListEvents_ValueChangedEventHandler(Eventlist_ValueChanged);
                foreach (var xSignal in Doi.Analog_signals.Values)
                {
                    xSignal.FirstInitialize = true;
                    ElectricEvent.Remove(xSignal.analogMeasurement);
                    AnalogMeasurementClass mref = (AnalogMeasurementClass)xSignal.analogMeasurement;
                    this.ReleaseCOMObject<AnalogMeasurementClass>(ref mref);
                }

            }
            catch (Exception e)
            {
                msg_eventhandler = ("Release Event Error " + e.ToString());
            }
        }
        private void ReleaseCOMObject<T>(ref T o)
        where T : class
        {
            IntPtr iUnknown = Marshal.GetIUnknownForObject(o);
            if (iUnknown != IntPtr.Zero)
            {
                Marshal.Release(iUnknown);
            }
            o = default(T);
        }
        private string SignalQuality(AnalogMeasurement AnalogStatusObject)
        {
            string Quality;

            if (AnalogStatusObject.IsCalculatedValue() == true)
            {
                Quality = "Calculated - ";
            }
            else if (AnalogStatusObject.IsEnteredValue() == true)
            {
                Quality = "Entered - ";
            }
            else if (AnalogStatusObject.IsEstimatedValue() == true)
            {
                Quality = "Estimated - ";
            }
            else
            {
                Quality = "Telemetered - ";
            }

            if (AnalogStatusObject.IsValidValue() == false)
            {
                Quality = Quality + "Invalid";
            }
            else if (AnalogStatusObject.IsUsableValue() == false)
            {
                Quality = Quality + "Suspect";
            }
            else if (AnalogStatusObject.IsBlocked() == true)
            {
                Quality = Quality + "Blocked";
            }
            else if (AnalogStatusObject.IsUpdatedValue() == true)
            {
                Quality = Quality + "Valid";
            }
            else
            {
                Quality = Quality + "Not Updated";
            }
            return Quality;
        }
    }
}

