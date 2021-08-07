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
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        public string Get()
        {
		// Process.Start(@".\dailySalesReq.exe");
	    ////////////////////////////////////////
			string input = System.IO.File.ReadAllText(@".\dailySalesQuery.xml");
      // string input = template.Replace("STARTDATE", sDate).Replace("ENDDATE", eDate);
			// string input = System.IO.File.ReadAllText(@"salesQuery.xml");

			Console.WriteLine ("XML: {0}", input);

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
				ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare );
				//ticket = rp.BeginSession("C:\\Users\\Public\\Documents\\Intuit\\QuickBooks\\Company Files\\Hannah's Restaurant_3.QBW", QBFileMode.qbFileOpenDoNotCare );
				Console.WriteLine ("check1 {0}", rp);

				response = rp.ProcessRequest(ticket, input);

					

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
