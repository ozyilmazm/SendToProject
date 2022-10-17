using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Interop.ScadaScripting;




namespace EventHandlerTest
{
    class EventHandler_Digital
    {
        bool FirstEnter = true;
        private ScadaEventList ElectricEvent;
        public string msg_eventhandler;
        public int EventCount=0;
        int FirstInitCount = 0;
        private List<string> signal_info = new List<string>();
        public DateTime BeginTime, EndTime;
        public int EventCount_Periodic { get; set; }
        public DateTime Begin_time_Periodic { get; set; }

        public TimeSpan elapsed_time { get; set; }

        public void PrepareEventHandler(Dictionary<string, ScadaSignal> signals)
        {
            ConnectToScadaScripting.ConnectToScada();
            try
            {
                this.ElectricEvent = new ScadaEventListClass();
                this.ElectricEvent.ValueChanged += new _IxScadaEventListEvents_ValueChangedEventHandler(this.Eventlist_ValueChanged);
                foreach (var xSignal in signals.Values)
                {
                    try
                    {
                        this.ElectricEvent.Add(xSignal.digitalMeasurement);
                    }
                    catch (Exception e)
                    {
                        Report.PrintLog("Error " + e.ToString());
                    }
                    
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
                DigitalMeasurement DigitalEventObject = (DigitalMeasurement)Measurement;
                Doi.Digital_signals.TryGetValue(DigitalEventObject.ObjectIdStr, out xSignal);
                qual = SignalQuality(DigitalEventObject);

                if (xSignal.FirstInitialize == true)
                {
                    xSignal.FirstInitialize = false;
                    FirstInitCount++;
                    if (FirstInitCount == Doi.Digital_signals.Count())
                    {
                        Console.WriteLine("\n BEGIN THE SIMULATION OF DIGITAL");
                    }
                }
                else
                {
                    if (FirstEnter == true)
                    {
                        FirstEnter = false;
                        BeginTime = DateTime.Now;
                        Begin_time_Periodic = DateTime.Now;

                        Console.WriteLine($"//////////////////////TEST STARTED////////////////////////DIGITAL {BeginTime}");
                        Report.PrintLog($"//////////////////////TEST STARTED////////////////////////DIGITAL {BeginTime}");
                        EventCount++;
                    }
                    EventCount++;
                    EventCount_Periodic++;
                    EndTime = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Report.PrintLog("Event Error " + e.ToString());
            }
        }

        public void ReleaseEventHandler()
        {
            elapsed_time = BeginTime - EndTime;
            try
            {
                ElectricEvent.ValueChanged -= new _IxScadaEventListEvents_ValueChangedEventHandler(Eventlist_ValueChanged);
                foreach (var xSignal in Doi.Digital_signals.Values)
                {
                    xSignal.FirstInitialize = true;
                    ElectricEvent.Remove(xSignal.digitalMeasurement);
                    DigitalMeasurementClass mref = (DigitalMeasurementClass)xSignal.digitalMeasurement;
                    this.ReleaseCOMObject<DigitalMeasurementClass>(ref mref);
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
        private string SignalQuality(DigitalMeasurement DigitalStatusObject)
        {
            string Quality;

            if (DigitalStatusObject.IsCalculatedValue() == true)
            {
                Quality = "Calculated - ";
            }
            else if (DigitalStatusObject.IsEnteredValue() == true)
            {
                Quality = "Entered - ";
            }
            else if (DigitalStatusObject.IsEstimatedValue() == true)
            {
                Quality = "Estimated - ";
            }
            else
            {
                Quality = "Telemetered - ";
            }

            if (DigitalStatusObject.IsValidValue() == false)
            {
                Quality = Quality + "Invalid";
            }
            else if (DigitalStatusObject.IsUsableValue() == false)
            {
                Quality = Quality + "Suspect";
            }
            else if (DigitalStatusObject.IsBlocked() == true)
            {
                Quality = Quality + "Blocked";
            }
            else if (DigitalStatusObject.IsUpdatedValue() == true)
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

