using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ScadaScripting;

namespace AlarmCounter
{
    public class Signal
    {
        public string objid { get; set; }
        public int count { get; set; }
        public DigitalMeasurement digital;
        public Signal(string guid)
        {
            this.objid = "{" + guid.ToUpper() + "}";
            digital = new DigitalMeasurementClass { ObjectIdStr = objid };
            
        }
        
    }
}
