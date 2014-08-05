using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace Shipwire.Controllers
{
    public class ShipwireClient
    {
        private string requestXml;
        private string destinationUrl = "https://api.shipwire.com/exec/InventoryServices.php";
        private XDocument xDoc; //record response in the type of XDocument

        public ShipwireClient(string username, string password, string server, string warehouse, string productcode)
        {
            string inventoryRequest = "<InventoryUpdate><Username>" + username + "</Username><Password>" + password + "</Password><Server>" + server + "</Server><Warehouse>" + warehouse + "</Warehouse><ProductCode>" + productcode + "</ProductCode><IncludeEmpty/></InventoryUpdate>";
            requestXml = inventoryRequest;
        }
        //Get the response about the Inventory information
        public string GetInventory()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            string results = "ConnectError"; //used to notice the Error
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                XmlReader reader = new XmlTextReader(responseStream);
                xDoc = XDocument.Load(reader);
                results = xDoc.ToString();
                //Close XMLReader
                reader.Close();
                responseStream.Close();
                response.Close();

                return results;
            }
            response.Close();
            return results;
        }
        //Check whether the response is correct
        public string Validate()
        {
            string status = xDoc.Element("InventoryUpdateResponse").Element("Status").Value;
            if (status == "0" || status == "Test")
            {
                //If the response is correct, we need to store TotalProducts which will be used later in loop
                string TotalProducts = xDoc.Element("InventoryUpdateResponse").Element("TotalProducts").Value;
                return TotalProducts;
            }
            else
            {
                return "InfoError";
            }
        }
    }

    public class Products
    {
        private string product = "Product";
        private XDocument xDoc;
        public Products(string results)
        {
            xDoc = XDocument.Parse(results);
        }

        public string[] Parse(string field)
        {
            var Value = from x in xDoc.Descendants(product) select x.Attribute(field).Value;
            string[] Array = Value.ToArray();
            return Array;
        }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            string username = Request["username"];
            string password = Request["password"];
            string server = Request["server"];
            string warehouse = Request["warehouse"];
            string productcode = Request["productcode"];
            ShipwireClient shipwireClient = new ShipwireClient(username, password, server, warehouse, productcode);
            //Check whether the connection is normal
            if (shipwireClient.GetInventory() == "ConnectError")
            {
                return RedirectToAction("ConnectError");
            }
            //Check whether the input is correct
            if (shipwireClient.Validate() == "InfoError")
            {
                return RedirectToAction("InfoError");
            }
            else
            {
                //If the response is correct, we need to store TotalProducts which will be used later in loop
                ViewBag.TotalProducts = Convert.ToInt32(shipwireClient.Validate());
            }

            Products products = new Products(shipwireClient.GetInventory());
            //18 fields
            ViewBag.Code = products.Parse("code");
            ViewBag.Quantity = products.Parse("quantity");
            ViewBag.Pending = products.Parse("pending");
            ViewBag.Good = products.Parse("good");
            ViewBag.Backordered = products.Parse("backordered");
            ViewBag.Reserved = products.Parse("reserved");
            ViewBag.Shipping = products.Parse("shipping");
            ViewBag.Shipped = products.Parse("shipped");
            ViewBag.Creating = products.Parse("creating");
            ViewBag.Consuming = products.Parse("consuming");
            ViewBag.Created = products.Parse("created");
            ViewBag.Consumed = products.Parse("consumed");
            ViewBag.ShippedLastDay = products.Parse("shippedLastDay");
            ViewBag.ShippedLastWeek = products.Parse("shippedLastWeek");
            ViewBag.ShippedLast4Weeks = products.Parse("shippedLast4Weeks");
            ViewBag.OrderedLastDay = products.Parse("orderedLastDay");
            ViewBag.OrderedLastWeek = products.Parse("orderedLastWeek");
            ViewBag.OrderedLast4Weeks = products.Parse("orderedLast4Weeks");

            return View();
        }

        public ActionResult InfoError()
        {
            ViewBag.Message = "Error with Valid Username/EmailAddress and Password Required. There is an error in XML document.";

            return View();
        }

        public ActionResult ConnectError()
        {
            ViewBag.Message = "Cannot get response from shipwire server successfully.";

            return View();
        }


    }
}
