using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ScadaScripting;
namespace AlarmCounter
{
    public static class Actions_Feedback
    {

        public static void Set_Alarm_Inhibit(DigitalMeasurement digi, string user)

        {
            digi.SetMarker(15, "Marker - Alarm Inhibit", $"This marker set by {user}");
        }

        public static void Clear_Alarm_Inhibit(DigitalMeasurement digi, string user)
        {
            digi.ClearMarkers(15, "Marker - Alarm Inhibit");
        }

        public static void Set_Marker_Blocked(DigitalMeasurement digi, string user)

        {
            try
            {
                digi.SetMarker(16, "Marker - Blocked", $"This marker set by {user}");
                digi.FetchValue();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            
        }

        public static void Clear_Marker_Blocked(DigitalMeasurement digi, string user)

        {
            try
            {
                digi.ClearMarkers(16, "Marker - Blocked");
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }

        }
    }
}
