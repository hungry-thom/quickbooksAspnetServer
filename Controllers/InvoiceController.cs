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
    public class InvoiceController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        public string Get()
        {
		// Process.Start(@".\dailySalesReq.exe");
	    ////////////////////////////////////////
      // string input = template.Replace("STARTDATE", sDate).Replace("ENDDATE", eDate);
			// string input = System.IO.File.ReadAllText(@"salesQuery.xml");
			//
			// generate xml query
			//string info = Request.QueryString.Value;

			XmlDocument inputXMLDoc = new XmlDocument();
			inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", "utf-8", null));
			inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"13.0\""));
			XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
			inputXMLDoc.AppendChild(qbXML);

			XmlElement qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "stopOnError");

			XmlElement invoiceQueryRq = inputXMLDoc.CreateElement("InvoiceQueryRq");
			qbXMLMsgsRq.AppendChild(invoiceQueryRq);
			invoiceQueryRq.SetAttribute("metaData", "MetaDataAndResponseData");

			if (Request.Query.ContainsKey("todate") && Request.Query.ContainsKey("fromdate")) {
				// TODO: switch modifiedData or TxnDate
				XmlElement txnDateRangeFilter = inputXMLDoc.CreateElement("TxnDateRangeFilter");
				invoiceQueryRq.AppendChild(txnDateRangeFilter);
				txnDateRangeFilter.AppendChild(inputXMLDoc.CreateElement("FromTxnDate")).InnerText=Request.Query["fromdate"];
				txnDateRangeFilter.AppendChild(inputXMLDoc.CreateElement("ToTxnDate")).InnerText=Request.Query["todate"];
			}

			
			// TODO: switch entitiy for listid and FullName
			if (Request.Query.ContainsKey("entity")) {
				XmlElement entityFilter = inputXMLDoc.CreateElement("EntityFilter");
				invoiceQueryRq.AppendChild(entityFilter);
				entityFilter.AppendChild(inputXMLDoc.CreateElement("ListID")).InnerText=Request.Query["entity"];
			}

			// TODO: switch PaidStatus [ All, PaidOnly, NotPaidOnly]
			invoiceQueryRq.AppendChild(inputXMLDoc.CreateElement("PaidStatus")).InnerText="NotPaidOnly";
			invoiceQueryRq.AppendChild(inputXMLDoc.CreateElement("IncludeLineItems")).InnerText="true";

			// Include Linked txns?
			// invoiceQueryRq.AppendChild(inputXMLDoc.CreateElement("IncludeLinkedTxns")).InnerText="true";
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
      // Console.WriteLine("response {0}", json2.SelectToken("QBXML.QBXMLMsgsRs.SalesReceiptQueryRs.SalesReceiptRet")); // .QBXML.SalesReceiptRet
      System.IO.File.WriteAllText(@".\salesResponse.json", json);
      return json;
      //System.IO.File.WriteAllText(@".\runHistory.ini", eDate);
      ////////////////////////////////////
      /*
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
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
