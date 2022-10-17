using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ScadaScripting;

namespace EventHandlerTest
{
    class ScadaSignal
    {
        public string Path { get; set; }
        public string ObjId { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }
        public IxAnalogMeasurement analogMeasurement { get; set; }
        public DigitalMeasurementClass digitalMeasurement { get; set; }
        public bool FirstInitialize { get; set; }
        public ScadaSignal(string ObjId, string signal_type)
        {
            if (signal_type == "Analog")
            {
                FirstInitialize = true;
                this.Type = signal_type;
                this.ObjId = ObjId.ToString();
                this.Path = Path;
                this.analogMeasurement = new AnalogMeasurement() { ObjectIdStr = this.ObjId };
            }
            else if (signal_type == "Discrete")
            {
                FirstInitialize = true;
                this.Type = signal_type;
                this.ObjId = ObjId.ToString();
                this.Path = Path;
                this.digitalMeasurement = new DigitalMeasurementClass() { ObjectIdStr = this.ObjId };
            }
           
        }
    }
}
