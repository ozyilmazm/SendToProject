using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Interop.ScadaScripting;
using Oracle.ManagedDataAccess.Client;
using Excel = Microsoft.Office.Interop.Excel;

namespace CommunicationFailure
{
    class Program
    {
        private static string ExePath, ExeName, LogPath;
        public static Dictionary<String, String> DistrictList = new Dictionary<string, string>(); // NAME - DESCRIPTION
        public static Dictionary<String, String> NetSubstationList = new Dictionary<string, string>();	//
        public static Dictionary<String, String> NetCFEList = new Dictionary<string, string>();	//
        private static List<Data> RawList = new List<Data>();
        private static List<Data> ExcelList = new List<Data>();
        private static Dictionary<string, Data> ExcelDict = new Dictionary<string, Data>();
        private static System.Timers.Timer EventTimer;
        private static string SystemType;
        private static string Start = "";
        private static string End = "";
        private static DateTime StartDate;
        private static DateTime EndDate;
        private static string PSOS;
        private static IxDigitalMeasurement Digital = new DigitalMeasurementClass(),;

        public static void Main(string[] args)
        {

            int i = 1;

            LogFolders();

            PSOS = Environment.GetEnvironmentVariable("SPC_PSOSHOSTNAME").ToString().Replace("\\", "");

            ScadaScriptComponent connection = new ScadaScriptComponent();
            connection.ContextName = "RT";
            connection.ConsoleName = Environment.MachineName;
            connection.UserName = Environment.UserName;
            connection.SetActive(0);



            if (args.Contains("SCADA"))
            {
                string yy, MM, dd, hh, mm, ss;
                SystemType = "SCADA";
                IxAnalogMeasurement Analog = new AnalogMeasurement { ObjectPath = "/Net-E/.Report/CommunicationFailure-2/Begin/Year" };
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Year";
                Analog.FetchValue();
                yy = Analog.Value.ToString();
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Month";
                Analog.FetchValue();
                MM = AZL(Convert.ToInt32(Analog.Value), 2);
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Day";
                Analog.FetchValue();
                dd = AZL(Convert.ToInt32(Analog.Value), 2);
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Hour";
                Analog.FetchValue();
                hh = AZL(Convert.ToInt32(Analog.Value), 2);
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Minute";
                Analog.FetchValue();
                mm = AZL(Convert.ToInt32(Analog.Value), 2);
                Analog.ObjectPath = "/Net-E/.Report/CommunicationFailure-2/End/Second";
                Analog.FetchValue();
                ss = AZL(Convert.ToInt32(Analog.Value), 2);

                End = dd + "." + MM + "." + yy + " " + hh + ":" + mm + ":" + ss;


                EndDate = DateTime.ParseExact(End, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                Log("TimeStamps Between " + Start + " - " + End);
            }
            else if (args.Contains("ONLINE"))
            {
                SystemType = "ONLINE";
                Log("ONLINE Time");
            }
            else
            {
                Log("Default Time");
                SystemType = "DEFAULT";
                EndDate = DateTime.Now;
                End = EndDate.ToString("dd.MM.yyyy HH:mm:ss");
            }


            if (SystemType == "ONLINE")
            {
                /* Time Event Handler*/
                EventTimer = new System.Timers.Timer(1000);
                EventTimer.Elapsed += OnTimedEvent;
                DateTime Current = DateTime.Now;
                DateTime FirstRun = Current.AddDays(1);
                FirstRun = new DateTime(FirstRun.Year, FirstRun.Month, FirstRun.Day, 0, 0, 0);
                TimeSpan TimeToGo = FirstRun - Current;
                EventTimer.Interval = TimeToGo.TotalMilliseconds;

                EventTimer.Enabled = true;

                ManualResetEvent WaitHandle = new ManualResetEvent(false);

                WaitHandle.WaitOne();
            }
            else
            {
                Report();
            }

        }
        public static void OnTimedEvent(object Source, ElapsedEventArgs e)
        {
            EventTimer.Interval = 24 * 60 * 60 * 1000; // Hour*Minute*Second*MilliSecond
            Report();
        }

        private static void Report()
        {
            Log("Event Started");

            DistrictList.Clear();
            NetSubstationList.Clear();
            NetCFEList.Clear();
            RawList.Clear();
            ExcelList.Clear();
            ExcelDict.Clear();

            if (SystemType == "ONLINE")
            {
                EndDate = DateTime.Now;
                End = EndDate.ToString("dd.MM.yyyy HH:mm:ss");
            }

            int i = 1;
            OracleConnection Connection;
            Connection = new OracleConnection();
            OracleCommand Query;


            try
            {
                Doi.Connect();
            }
            catch (Exception e)
            {
                Log("DOI Connection " + e.ToString());
            }

            Doi.FillDistricts();
            Doi.FillSubstations();
            Doi.FillCfeRTUS();


            #region DOR DB SORGUSU YORUMA ALINDI
           
            
            //Connection.ConnectionString = "Data Source = //" + PSOS + ":1521/DOR;User Id=sys;Password=kn0wn_sys;DBA Privilege=SYSDBA";
            //OracleCommand Query;
            //Query = new OracleCommand();
            //Query.Connection = Connection;
            //Query.CommandText = "ALTER SESSION SET CURRENT_SCHEMA=DOR_RT";
            //Query.CommandType = CommandType.Text;
            //OracleDataReader QueryResult;
            //QueryResult = Query.ExecuteReader();
            //         Query.CommandText = " SELECT NAME,A_DESCRIPTION " +
            //         					" FROM I_SYSNETSUBSTATIONS " +
            //         					" WHERE A_DESCRIPTION <> ' ' AND VERSNO = '1'" ;
            //         QueryResult = Query.ExecuteReader();

            //while (QueryResult.Read())
            //         {
            //             try
            //             {
            //             	if (DistrictList.ContainsKey(QueryResult.GetValue(0).ToString()) == false )
            //             	{
            //             		DistrictList.Add(QueryResult.GetValue(0).ToString(), QueryResult.GetValue(1).ToString());
            //             	}
            //             }
            //             catch (Exception exp)
            //             {
            //                 Log("DOR ALIAS : " + exp.Message);
            //             }
            //         }

            //Query.CommandText = " SELECT SUBSTATION.NAME, NETSUBSTATION.NAME " +
            //					" FROM IMM_NAMESERVICE SUBSTATION " +
            //					" INNER JOIN IMM_NAMESERVICE NETSUBSTATION ON SUBSTATION.PARENTIID = NETSUBSTATION.IID AND NETSUBSTATION.VERSNO = '1' " +
            //					" WHERE SUBSTATION.TYPEID = 'ADB41325641CD41181E300105A24673A' AND SUBSTATION.VERSNO = '1' ";

            //         QueryResult = Query.ExecuteReader(); 
            //while (QueryResult.Read())
            //         {            
            //          try
            //          {
            //          	if (NetSubstationList.ContainsKey(QueryResult.GetValue(0).ToString()) == false)
            //          	{
            //              	NetSubstationList.Add(QueryResult.GetValue(0).ToString(),QueryResult.GetValue(1).ToString() );              		
            //          	}
            //          }
            //             catch (Exception e)
            //             {
            //                 Log("DOR SUBSTATION : " + e.Message);
            //             }
            //         }

            //Query.CommandText = " SELECT SUBSTATION.NAME, NETSUBSTATION.NAME " +
            //                    " FROM IMM_NAMESERVICE SUBSTATION " +
            //                    " INNER JOIN IMM_NAMESERVICE NETSUBSTATION ON SUBSTATION.PARENTIID = NETSUBSTATION.IID AND NETSUBSTATION.VERSNO = '1'" +
            //                    " WHERE SUBSTATION.TYPEID = 'B7CA187E7D89D411820200105A24673A' AND SUBSTATION.VERSNO = '1'";

            //QueryResult = Query.ExecuteReader();
            //while (QueryResult.Read())
            //{
            //    try
            //    {

            //        if (NetCFEList.ContainsKey(QueryResult.GetValue(0).ToString()) == false)
            //        {
            //            NetCFEList.Add(QueryResult.GetValue(0).ToString(), QueryResult.GetValue(1).ToString());
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Log("DOR CFE : " + e.Message);
            //    }
            //}
            //QueryResult.Close();
            //Connection.Close();
            #endregion

            Connection.ConnectionString = "Data Source = //" + PSOS + ":1521/APPSHIS;User Id=sys;Password=kn0wn_sys;DBA Privilege=SYSDBA";
            try
            {
                Connection.Open();
            }
            catch (Exception e)
            {
                Log("HIS Connection " + e.ToString());
            }
            Query = new OracleCommand();
            Query.Connection = Connection;
            Query.CommandText = "ALTER SESSION SET CURRENT_SCHEMA=HISU";
            Query.CommandType = CommandType.Text;
            OracleDataReader QueryResult;
            

            Query.CommandText = " SELECT PATH1,C_TIME.Localtime(TIME_STAMP,42),VALUE,GUID,STATUS,PATH2 " +
                                " FROM HIS_MESSAGE " +
                                " WHERE STATUS = 'App' AND PATH4  = '...Failure' AND STATUS <> 'Ack' AND INDICATOR LIKE '%Alarm%' AND " +
                                " (PATH1 ,C_TIME.Localtime(TIME_STAMP,42)) IN ( " +
                                " SELECT PATH1,   MAX(C_TIME.Localtime(TIME_STAMP,42)) " +
                                " FROM HIS_MESSAGE " +
                                " WHERE  C_TIME.Localtime(TIME_STAMP,42) <= to_timestamp('" + End + "','dd-MM-yyyy hh24:mi:ss') " +
                                    " AND PATH4  = '...Failure' AND PATH1 NOT LIKE '%CFE%' AND STATUS <> 'Ack' AND INDICATOR LIKE '%Alarm%' " +
                                " GROUP BY PATH1 )";

            QueryResult = Query.ExecuteReader();

            while (QueryResult.Read())
            {
                try
                {

                    var xData = new Data(i, NetCFEList[QueryResult.GetValue(0).ToString()], DistrictList[NetSubstationList[QueryResult.GetValue(0).ToString()]], NetSubstationList[QueryResult.GetValue(0).ToString()], QueryResult.GetValue(0).ToString(), QueryResult.GetValue(2).ToString(), Convert.ToDateTime(QueryResult.GetValue(1)), QueryResult.GetValue(4).ToString());
                    xData.ElapsedTime = Convert.ToInt32((EndDate - xData.TimeStamp).TotalSeconds);
                    try
                    {

                        if (ExcelDict.ContainsKey(xData.SubStation.ToString()) == false)
                        {

                            ExcelDict.Add(xData.SubStation, xData);
                            i++;
                        }


                    }
                    catch (Exception)
                    {
                    }
                    //ExcelList.Add (xData);
                    //ExcelList.Last().ElapsedTime = Convert.ToInt32((EndDate - ExcelList.Last().TimeStamp).TotalSeconds ) ;

                }
                catch (Exception e)
                {
                    Log("HIS : " + e.ToString());
                }
            }



            Log("Exporting to Excel");
            /*###############################################WRITING EXCEL##################################################################################################################################*/
            String ExportFileName;
            string FileName, TemplatePath;
            DateTime ExportDate = DateTime.Now;
            FileName = "CommunicationFailure " + ExportDate.Year + AZL(ExportDate.Month, 2) + AZL(ExportDate.Day, 2) + "_" + AZL(ExportDate.Hour, 2) + AZL(ExportDate.Minute, 2) + AZL(ExportDate.Second, 2);

            TemplatePath = @"D:\RAPORLAR\CommunicationFailure\Template\CommunicationFailure.xlsx";

            Excel.Application Template = null;
            Excel.Workbooks xDocuments = null;
            Excel.Workbook xDocument = null;
            Excel.Sheets Pages = null;
            Excel.Worksheet Page = null;

            Template = new Excel.Application();
            xDocuments = Template.Workbooks;
            xDocument = xDocuments.Open(@TemplatePath);
            Pages = xDocument.Worksheets;
            Page = (Excel.Worksheet)Pages["Rapor"];

            try
            {
                int K = 2;
                Page.Cells[1, 7] = (string)(Page.Cells[1, 7] as Excel.Range).Value + "\r\n" + EndDate.ToString("dd-MM-yyyy HH:mm:ss");
                foreach (var obj in ExcelDict.Values)
                {
                    Page.Cells[K, 1] = K - 1;
                    Page.Cells[K, 2] = obj.District;
                    Page.Cells[K, 3] = obj.CommType;
                    Page.Cells[K, 4] = obj.NetSubstation;
                    Page.Cells[K, 5] = obj.SubStation;
                    Page.Cells[K, 6] = obj.TimeStamp;
                    Page.Cells[K, 7] = obj.ElapsedTime;
                    K++;
                }

                if (SystemType == "ONLINE")
                {

                    string xPath = @"D:\RAPORLAR\CommunicationFailure\" + ExportDate.Year;
                    if (!Directory.Exists(@xPath))
                    {
                        Directory.CreateDirectory(@xPath);
                    }
                    Console.WriteLine(xPath);
                    xPath = xPath + @"\" + ExportDate.Month;
                    if (!Directory.Exists(xPath))
                    {
                        Directory.CreateDirectory(xPath);
                    }
                    Console.WriteLine(xPath);
                    if (System.IO.File.Exists(xPath + @"\" + FileName + ".xlsx"))
                    {
                        System.IO.File.Delete(xPath + @"\" + FileName + ".xlsx");
                    }
                    xDocument.SaveAs(xPath + @"\" + FileName + ".xlsx");
                }
                else
                {
                    string xPath = @"D:\RAPORLAR\CommunicationFailure";
                    if (System.IO.File.Exists(@xPath + @"\" + FileName + ".xlsx"))
                    {
                        System.IO.File.Delete(@xPath + @"\" + FileName + ".xlsx");
                    }
                    xDocument.SaveAs(@xPath + @"\" + FileName + ".xlsx");
                }

                xDocument.Close(0);
                Template.Quit();
                Marshal.ReleaseComObject(Page);
                Marshal.ReleaseComObject(Pages);
                Marshal.ReleaseComObject(xDocument);
                Marshal.ReleaseComObject(xDocuments);
                Marshal.ReleaseComObject(Template);
            }

            catch (Exception e)
            {
                Log("Error " + e.ToString());

                if (SystemType == "ONLINE")
                {

                    string xPath = @"D:\RAPORLAR\CommunicationFailure\" + ExportDate.Year;
                    if (!Directory.Exists(@xPath))
                    {
                        Directory.CreateDirectory(@xPath);
                    }
                    Console.WriteLine(xPath);
                    xPath = xPath + @"\" + ExportDate.Month;
                    if (!Directory.Exists(xPath))
                    {
                        Directory.CreateDirectory(xPath);
                    }
                    Console.WriteLine(xPath);
                    if (System.IO.File.Exists(xPath + @"\" + FileName + ".xlsx"))
                    {
                        System.IO.File.Delete(xPath + @"\" + FileName + ".xlsx");
                    }
                    xDocument.SaveAs(xPath + @"\" + FileName + ".xlsx");
                }
                else
                {
                    string xPath = @"D:\RAPORLAR\CommunicationFailure";
                    if (System.IO.File.Exists(@xPath + @"\" + FileName + ".xlsx"))
                    {
                        System.IO.File.Delete(@xPath + @"\" + FileName + ".xlsx");
                    }
                    xDocument.SaveAs(@xPath + @"\" + FileName + ".xlsx");
                }
                xDocument.Close(0);
                Template.Quit();
                Marshal.ReleaseComObject(Page);
                Marshal.ReleaseComObject(Pages);
                Marshal.ReleaseComObject(xDocument);
                Marshal.ReleaseComObject(xDocuments);
                Marshal.ReleaseComObject(Template);
            }
            Log("Program Finished");
            /*###############################################END OF WRITING EXCEL##################################*/
        }


        public static void Log(string LogState)
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            string LogFile = LogPath + "/" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
            StreamWriter Writer = new StreamWriter(LogFile, true);
            LogState = DateTime.Now + " " + LogState;
            Writer.WriteLine(LogState);
            Writer.Close();
            Writer = null;
        }
        private static void LogFolders()
        {
            int Index;
            ExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            Index = ExePath.LastIndexOf("\\");
            ExeName = ExePath.Substring(ExePath.LastIndexOf("\\") + 1);
            ExeName = ExeName.Replace(".exe", "");
            ExePath = ExePath.Remove(ExePath.LastIndexOf("\\") + 1);
            LogPath = ExePath + ExeName + " Logs";

            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

        }
        public static string AZL(int C, int L)
        {
            int Z;
            int Y;
            string xText;
            xText = Convert.ToString(C);
            Y = L - xText.Length;
            for (Z = 0; Z < Y; Z++)
            {
                xText = "0" + xText;
            }

            return xText;
        }


    }
}