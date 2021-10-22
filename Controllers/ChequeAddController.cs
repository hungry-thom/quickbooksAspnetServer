using System;
using System.IO;
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
    public class ChequeAddController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ChequeAddController> _logger;

        public ChequeAddController(ILogger<ChequeAddController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        //public IEnumerable<WeatherForecast> Get()
        public async Task<string> Post()
        {
		// Process.Start(@".\dailySalesReq.exe");
	    ////////////////////////////////////////
      // string input = template.Replace("STARTDATE", sDate).Replace("ENDDATE", eDate);
			// string input = System.IO.File.ReadAllText(@"salesQuery.xml");
			//
			// generate xml query
			//string info = Request.QueryString.Value;

		/*
			XmlDocument inputXMLDoc = new XmlDocument();
			inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", "utf-8", null));
			inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=\"13.0\""));
			XmlElement qbXML = inputXMLDoc.CreateElement("QBXML");
			inputXMLDoc.AppendChild(qbXML);

			XmlElement qbXMLMsgsRq = inputXMLDoc.CreateElement("QBXMLMsgsRq");
			qbXML.AppendChild(qbXMLMsgsRq);
			qbXMLMsgsRq.SetAttribute("onError", "stopOnError");

			XmlElement checkQueryRq = inputXMLDoc.CreateElement("CheckQueryRq");
			qbXMLMsgsRq.AppendChild(checkQueryRq);
			checkQueryRq.SetAttribute("metaData", "MetaDataAndResponseData");
			checkQueryRq.AppendChild(inputXMLDoc.CreateElement("MaxReturned")).InnerText="10";
			
			// mod
			XmlElement txnDateRangeFilter = inputXMLDoc.CreateElement("TxnDateRangeFilter");
			checkQueryRq.AppendChild(txnDateRangeFilter);
			txnDateRangeFilter.AppendChild(inputXMLDoc.CreateElement("DateMacro")).InnerText="All";

			checkQueryRq.AppendChild(inputXMLDoc.CreateElement("IncludeLineItems")).InnerText="true";
			checkQueryRq.AppendChild(inputXMLDoc.CreateElement("IncludeLinkedTxns")).InnerText="true";

			/*
			// TODO: switch entitiy for listid and FullName
			if (Request.Query.ContainsKey("name")) {
				XmlElement nameFilter = inputXMLDoc.CreateElement("NameFilter");
				checkQueryRq.AppendChild(nameFilter);
				// MatchCriterion may have one of the following values: StartsWith, Contains, EndsWith
				nameFilter.AppendChild(inputXMLDoc.CreateElement("MatchCriterion")).InnerText="Contains";
				nameFilter.AppendChild(inputXMLDoc.CreateElement("Name")).InnerText=Request.Query["name"];
			}
			string generatedXML = inputXMLDoc.OuterXml;
			*/
			string generatedXML = "";
			using (StreamReader stream = new StreamReader(Request.Body))
			{
				generatedXML = await stream.ReadToEndAsync();
			}

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
      //System.IO.File.WriteAllText(@".\salesResponse.json", json);
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
