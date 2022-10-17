using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.CdaDoi;
using Interop.ScadaScripting;

namespace EventHandlerTest
{
    static class Doi
    {

        public static Dictionary<string, ScadaSignal> Analog_signals = new Dictionary<string, ScadaSignal>();
        public static Dictionary<string, ScadaSignal> Digital_signals = new Dictionary<string, ScadaSignal>();
        public static int not_added_analog, not_added_digital;
        public static void GetAnalog(string network)
        {
            ConnectToScadaScripting.ConnectToScada();

            try
            {
                CxCdaDoiConnection Connection = new CxCdaDoiConnection();
                Connection.Connect("PSOS", "RT");

                CxCdaDoiCommand DoiCommand;
                DoiCommand = Connection.GetCommandObj();
                DoiCommand.AddPrimaryType("Analog", 0);
                DoiCommand.AddAttribute("Analog.Path");
                DoiCommand.AddAttribute("Analog.ObjectID");

                CxCdaDoiRecordset QueryResult;
                QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

                while (QueryResult.IsEOF() == 0)
                {
                    if (QueryResult.GetAttribute("Analog.Path").Contains(network))
                    {
                        try
                        {
                            string objID = QueryResult.GetAttribute("Analog.ObjectID");
                            ScadaSignal temp = new ScadaSignal(objID, "Analog");
                            temp.analogMeasurement.FetchValue();
                            Analog_signals.Add(objID, temp);
                        }
                        catch (Exception e)
                        {
                            Report.PrintLog_FaultySignals(QueryResult.GetAttribute("Analog.Path") + "-------- eklenemedi." + "exception is: " + e);
                            not_added_analog++;
                        }
                    }

                    QueryResult.MoveNext();

                }
            }
            catch (Exception e)
            {
                Report.PrintLog("Get DOR Config Error " + e.ToString());
            }

        }
        public static void GetDiscrete(string network)
        {
            ConnectToScadaScripting.ConnectToScada();

            try
            {
                CxCdaDoiConnection Connection = new CxCdaDoiConnection();
                Connection.Connect("PSOS", "RT");

                CxCdaDoiCommand DoiCommand;
                DoiCommand = Connection.GetCommandObj();
                DoiCommand.AddPrimaryType("Discrete", 0);
                DoiCommand.AddAttribute("Discrete.Path");
                DoiCommand.AddAttribute("Discrete.ObjectID");
                DoiCommand.AddAttribute("Discrete.MeasurementType");

                CxCdaDoiRecordset QueryResult;
                QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

                while (QueryResult.IsEOF() == 0)
                {
                    if (QueryResult.GetAttribute("Discrete.Path").Contains(network) /*&& QueryResult.GetAttribute("Discrete.MeasurementType") != 101 && QueryResult.GetAttribute("Discrete.MeasurementType") != 102*/ )
                    {
                        try
                        {
                            string objID = QueryResult.GetAttribute("Discrete.ObjectID");
                            //Discrete_signals.Add(new ScadaSignal(QueryResult.GetAttribute("Discrete.Path"), QueryResult.GetAttribute("Discrete.ObjectID"),signal_type));
                            ScadaSignal temp = new ScadaSignal(objID, "Discrete");
                            temp.digitalMeasurement.FetchValue();
                            Digital_signals.Add(objID, temp);
                            // Console.WriteLine(QueryResult.GetAttribute("Discrete.MeasurementType"));
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(QueryResult.GetAttribute("Discrete.Path") + "-------- eklenemedi." + "exception is: " + e);
                            Report.PrintLog_FaultySignals(QueryResult.GetAttribute("Discrete.Path") + "-------- eklenemedi." + "exception is: " + e);
                            not_added_digital++;
                        }

                    }

                    QueryResult.MoveNext();

                }
            }
            catch (Exception e)
            {
                Report.PrintLog("Get DOR Config Error " + e.ToString());
            }

        }
        public static void GetAll(string network)
        {
            GetAnalog(network);
            GetDiscrete(network);
        }
        public static void GetAnalog()
        {
            ConnectToScadaScripting.ConnectToScada();

            try
            {
                CxCdaDoiConnection Connection = new CxCdaDoiConnection();
                Connection.Connect("PSOS", "RT");

                CxCdaDoiCommand DoiCommand;
                DoiCommand = Connection.GetCommandObj();
                DoiCommand.AddPrimaryType("Analog", 0);
                DoiCommand.AddAttribute("Analog.Path");
                DoiCommand.AddAttribute("Analog.ObjectID");

                CxCdaDoiRecordset QueryResult;
                QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

                while (QueryResult.IsEOF() == 0)
                {
                    try
                    {
                        string objID = QueryResult.GetAttribute("Analog.ObjectID");
                        ScadaSignal temp = new ScadaSignal(objID, "Analog");
                        temp.analogMeasurement.FetchValue();
                        Analog_signals.Add(objID, temp);
                    }
                    catch (Exception e)
                    {
                        Report.PrintLog_FaultySignals(QueryResult.GetAttribute("Analog.Path") + "-------- eklenemedi." + "exception is: " + e);
                        not_added_analog++;
                    }
                    QueryResult.MoveNext();
                }

            }
            catch (Exception e)
            {
                Report.PrintLog("Get DOR Config Error " + e.ToString());
            }

        }
        public static void GetDiscrete()
        {
            ConnectToScadaScripting.ConnectToScada();

            try
            {
                CxCdaDoiConnection Connection = new CxCdaDoiConnection();
                Connection.Connect("PSOS", "RT");

                CxCdaDoiCommand DoiCommand;
                DoiCommand = Connection.GetCommandObj();
                DoiCommand.AddPrimaryType("Discrete", 0);
                DoiCommand.AddAttribute("Discrete.Path");
                DoiCommand.AddAttribute("Discrete.ObjectID");

                CxCdaDoiRecordset QueryResult;
                QueryResult = Connection.GetCdaDoiRecordset(DoiCommand);

                while (QueryResult.IsEOF() == 0)
                {
                    try
                    {
                        string objID = QueryResult.GetAttribute("Discrete.ObjectID");
                        //Discrete_signals.Add(new ScadaSignal(QueryResult.GetAttribute("Discrete.Path"), QueryResult.GetAttribute("Discrete.ObjectID"),signal_type));
                        ScadaSignal temp = new ScadaSignal(objID, "Discrete");
                        temp.digitalMeasurement.FetchValue();
                        Digital_signals.Add(objID, temp);
                        // Console.WriteLine(QueryResult.GetAttribute("Discrete.MeasurementType"));
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(QueryResult.GetAttribute("Discrete.Path") + "-------- eklenemedi." + "exception is: " + e);
                        Report.PrintLog_FaultySignals(QueryResult.GetAttribute("Discrete.Path") + "-------- eklenemedi." + "exception is: " + e);
                        not_added_digital++;
                    }
                    QueryResult.MoveNext();
                }
            }
            catch (Exception e)
            {
                Report.PrintLog("Get DOR Config Error " + e.ToString());
            }

        }
        public static void GetAll()
        {
            GetAnalog();
            GetDiscrete();
        }
    }
}

