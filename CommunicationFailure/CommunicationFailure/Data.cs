/*
 * Created by SharpDevelop.
 * User: SPCAdmin
 * Date: 2/19/2020
 * Time: 1:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CommunicationFailure
{
	/// <summary>
	/// Description of Data.
	/// </summary>
	public class Data
	{
		public int OrderNumber { get; set; }
        public string District { get; set; }
        public string NetSubstation { get; set; }
        public string SubStation { get; set; }
        public int FailureCount { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ElapsedTime { get; set; }
        public string Status {get; set; }
        public string Value {get; set; }        
        public string CommType {get; set; }
        public string TimeRange {get; set; }
        
        public Data (int OrderNumber, string CommType, string District, string NetSubstation, string SubStation, string Value, DateTime TimeStamp, string Status)
        {
        	this.OrderNumber 		= OrderNumber;
        	this.CommType 			= CommType;
        	this.District 			= District;
        	this.SubStation 		= SubStation ;
        	this.NetSubstation 		= NetSubstation ;
        	this.TimeStamp 			= TimeStamp ;
        	this.Status 			= Status ;
        	this.Value 				= Value ; 
        	this.FailureCount 		= 0;
        	this.ElapsedTime 		= 0;
        	this.TimeRange 			= TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + " - ";
        }
        public Data (Data Obj)
        {
        	this.OrderNumber 	= Obj.OrderNumber;
        	this.District 		= Obj.District;
        	this.NetSubstation 	= Obj.NetSubstation;
        	this.SubStation 	= Obj.SubStation;
        	this.FailureCount 	= Obj.FailureCount;
        	this.TimeStamp 		= Obj.TimeStamp;
        	this.ElapsedTime 	= Obj.ElapsedTime;
        	this.Status 		= Obj.Status;
        	this.CommType 		= Obj.CommType;
        	this.TimeRange 		= Obj.TimeRange;
        }
        public void Print ()
        {
        	Console.WriteLine (this.OrderNumber + " " + this.SubStation+ " " + this.TimeStamp+ " " + this.ElapsedTime);
        }
        
		
	}
}
