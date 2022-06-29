using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Xml;
using Interop.QBXMLRP2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditTrailController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<AuditTrailController> _logger;

        public AuditTrailController(ILogger<AuditTrailController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //public IEnumerable<AuditTrail> Get()
        public string Get()
        {
		// Process.Start(@".\dailySalesReq.exe");
	    ////////////////////////////////////////
			//string input = System.IO.File.ReadAllText(@".\dailySalesQuery.xml");
      // string input = template.Replace("STARTDATE", sDate).Replace("ENDDATE", eDate);
			// string input = System.IO.File.ReadAllText(@"salesQuery.xml");
			//
			// generate xml query
			//string info = Request.QueryString.Value;
			string info = "GoodQuery";
			if (!Request.Query.ContainsKey("todate") || !Request.Query.ContainsKey("fromdate")) {
				info = "BadQuery";
				return info;
			}	
			Console.WriteLine (info);

			XmlDocument inputXMLDoc = new XmlDocument();
			inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", "utf-8", null));
			inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"13.0\""));
			XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
			inputXMLDoc.AppendChild(qbXML);

			XmlElement qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "stopOnError");

			XmlElement generalDetailReportQueryRq = inputXMLDoc.CreateElement("GeneralDetailReportQueryRq");
			qbXMLMsgsRq.AppendChild(generalDetailReportQueryRq);
			generalDetailReportQueryRq.AppendChild(inputXMLDoc.CreateElement("GeneralDetailReportType")).InnerText="AuditTrail";

			// TODO: switch modifiedData or TxnDate
			XmlElement reportPeriod = inputXMLDoc.CreateElement("ReportPeriod");
			generalDetailReportQueryRq.AppendChild(reportPeriod);
			reportPeriod.AppendChild(inputXMLDoc.CreateElement("FromReportDate")).InnerText=Request.Query["fromdate"];
			reportPeriod.AppendChild(inputXMLDoc.CreateElement("ToReportDate")).InnerText=Request.Query["todate"];

			XmlElement reportTxnTypeFilter = inputXMLDoc.CreateElement("ReportTxnTypeFilter");
			generalDetailReportQueryRq.AppendChild(reportTxnTypeFilter);
			reportTxnTypeFilter.AppendChild(inputXMLDoc.CreateElement("TxnTypeFilter")).InnerText="All";
			
			generalDetailReportQueryRq.AppendChild(inputXMLDoc.CreateElement("ReportModifiedDateRangeMacro")).InnerText="All";
			string generatedXML = inputXMLDoc.OuterXml;

			Console.WriteLine ("XML: {0}", generatedXML);

			//step3: do the qbXMLRP request

			RequestProcessor2 rp = null; 

			string ticket = null;

			string response = null;

			try 

			{

				Console.Write ("check1");
				rp = new RequestProcessor2 ();
				Console.Write ("check2");
				rp.OpenConnection("", "IDN CustomerAdd C# sample" );
				Console.WriteLine ("check3 {0}", rp);
				//string info = Request.QueryString.ToString();
				//ICollection<string> queryKeys = Request.Query.Keys;
				//string info = Request.Query["date"];
				ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare );
				//ticket = rp.BeginSession("C:\\Users\\Public\\Documents\\Intuit\\QuickBooks\\Company Files\\Hannah's Restaurant_3.QBW", QBFileMode.qbFileOpenDoNotCare );
				Console.WriteLine ("check1 {0}", rp);

				response = rp.ProcessRequest(ticket, generatedXML);
				//response = rp.ProcessRequest(ticket, input);

					

			}

			catch( System.Runtime.InteropServices.COMException ex )
			//catch( )

			{

				Console.WriteLine ( "COM Error Description = " +  ex.Message + "COM error" );

				//return;

			}

			finally

			{

				if( ticket != null )

				{

					rp.EndSession(ticket);

				}

				if( rp != null )

				{

					rp.CloseConnection();

				}

			};



			//step4: parse the XML response and export

			XmlDocument outputXMLDoc = new XmlDocument();

			outputXMLDoc.LoadXml(response);
			string json = JsonConvert.SerializeXmlNode(outputXMLDoc);
			//string json = "sucess";
			// JObject json2 = JObject.Parse(json)
      // Console.WriteLine("response {0}", json2.SelectToken("QBXML.QBXMLMsgsRs.GeneralDetailReportQueryRs.GeneralDetailReportRet")); // .QBXML.GeneralDetailReportRet
      System.IO.File.WriteAllText(@".\salesResponse.json", json);
      return json;
      //System.IO.File.WriteAllText(@".\runHistory.ini", eDate);
      ////////////////////////////////////
      /*
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new AuditTrail
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
	    */
        }
    }
}
