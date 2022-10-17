using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.ScadaScripting;
using Interop.ScadaInfo;

namespace EventHandlerTest
{
    public static class ConnectToScadaScripting
    { 
        public static void ConnectToScada()
        {
            try
            {
                IxScriptComponent m_ScriptingComponent = new ScadaScriptComponent();
                m_ScriptingComponent.ContextName = "RT";
                m_ScriptingComponent.UserName = Environment.UserName;
                m_ScriptingComponent.ConsoleName = Environment.MachineName;
                m_ScriptingComponent.SetActive(0);

            }
            catch (Exception exp)
            {
             Console.WriteLine($"Scada connection failed due to {exp} ");
            }
            
            
        }
    }
}
