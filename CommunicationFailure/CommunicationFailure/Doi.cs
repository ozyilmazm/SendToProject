using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.CdaDoi;
using Microsoft.Office.Interop.Excel;
using static System.Collections.Specialized.BitVector32;

namespace CommunicationFailure
{
    static class Doi
    {
        public static CxCdaDoiConnection Connection;
        public static Dictionary<string, string> DataDistrict = new Dictionary<string, string>();
        public static void Connect()
        {
            try
            {
                Connection = new CxCdaDoiConnection();
                Connection.Connect("PSOS", "RT");
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public static void FillDistricts()
        {

            string Name;
            string Description;
            CxCdaDoiCommand DoiCommand;
            DoiCommand = Connection.GetCommandObj();
            DoiCommand.AddPrimaryType("sysNetSubstations", 0);
            DoiCommand.AddAttribute("sysNetSubstations.Path");
            DoiCommand.AddAttribute("sysNetSubstations.Description");


            CxCdaDoiRecordset QueryResult;
            QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

            while (QueryResult.IsEOF() == 0)
            {
                try
                {

                    if (!DBNull.Value.Equals(QueryResult.GetAttribute("sysnetSubstations.Description")))
                    {
                        Description = QueryResult.GetAttribute("sysnetSubstations.Description");
                    }
                    else
                    {
                        Description = "No Description";
                    }

                    Name = QueryResult.GetAttribute("sysNetSubstations.Path").ToString().Split('/')[1];
                    Program.DistrictList.Add(Name,Description);
                }
                catch (ArgumentException)
                {
                }
               
                QueryResult.MoveNext();
            }


        }
        public static void FillSubstations()
        {
            CxCdaDoiCommand DoiCommand;
            DoiCommand = Connection.GetCommandObj();
            DoiCommand.AddPrimaryType("Substation", 0);
            DoiCommand.AddAttribute("Substation.Path");

            CxCdaDoiRecordset QueryResult;
            QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

            while (QueryResult.IsEOF() == 0)
            {
                try
                {
                    Program.NetSubstationList.Add(QueryResult.GetAttribute("Substation.Path").Split('/')[2], QueryResult.GetAttribute("Substation.Path").Split('/')[1]);
                }
                catch (ArgumentException)
                {
                }
                QueryResult.MoveNext();
            }


        }
        public static void FillCfeRTUS()
        {
            CxCdaDoiCommand DoiCommand;
            DoiCommand = Connection.GetCommandObj();
            DoiCommand.AddPrimaryType("CfeRemoteTerminalUnit", 0);
            DoiCommand.AddAttribute("CfeRemoteTerminalUnit.Path");

            CxCdaDoiRecordset QueryResult;
            QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

            while (QueryResult.IsEOF() == 0)
            {
                if (QueryResult.GetAttribute("CfeRemoteTerminalUnit.Path").ToString().Split('/').Length > 3)
                {
                    try
                    {
                        Program.NetCFEList.Add(QueryResult.GetAttribute("CfeRemoteTerminalUnit.Path").ToString().Split('/')[3], QueryResult.GetAttribute("CfeRemoteTerminalUnit.Path"));
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                QueryResult.MoveNext();
            }


        }

    }
}
