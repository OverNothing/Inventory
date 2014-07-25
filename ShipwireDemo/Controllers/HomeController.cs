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

namespace MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //You need to replace Username and Password with your own account in requestXml
            string requestXml = "<InventoryUpdate><Username>api_user@example.com</Username><Password>yourpassword</Password><Server>Production</Server><Warehouse></Warehouse><ProductCode></ProductCode><IncludeEmpty/></InventoryUpdate>";
            string destinationUrl = "https://api.shipwire.com/exec/InventoryServices.php";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);

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

                XDocument xDoc = XDocument.Load(reader);
                string s = xDoc.ToString();
                XDocument xDoc2 = XDocument.Parse(s);
                string TotalProducts = xDoc.Element("InventoryUpdateResponse").Element("TotalProducts").Value;
                int totalproducts = Convert.ToInt32(TotalProducts);

                //18 Attributes in Shipwire Inventory Response. And convert attributes to array which is easier to use
                //Product Code
                var CodeValue = from x in xDoc2.Descendants("Product") select x.Attribute("code").Value;
                string[] CodeArray = CodeValue.ToArray();
                //Quantity
                var QuantityValue = from x in xDoc2.Descendants("Product") select x.Attribute("quantity").Value;
                string[] QuantityArray = QuantityValue.ToArray();
                //Pending
                var PendingValue = from x in xDoc2.Descendants("Product") select x.Attribute("pending").Value;
                string[] PendingArray = PendingValue.ToArray();
                //Good
                var GoodValue = from x in xDoc2.Descendants("Product") select x.Attribute("good").Value;
                string[] GoodArray = GoodValue.ToArray();
                //Backordered
                var BackorderedValue = from x in xDoc2.Descendants("Product") select x.Attribute("backordered").Value;
                string[] BackorderedArray = BackorderedValue.ToArray();
                //Reserved
                var ReservedValue = from x in xDoc2.Descendants("Product") select x.Attribute("reserved").Value;
                string[] ReservedArray = ReservedValue.ToArray();
                //Shipping
                var ShippingValue = from x in xDoc2.Descendants("Product") select x.Attribute("shipping").Value;
                string[] ShippingArray = ShippingValue.ToArray();
                //Shipped
                var ShippedValue = from x in xDoc2.Descendants("Product") select x.Attribute("shipped").Value;
                string[] ShippedArray = ShippedValue.ToArray();
                //Consuming
                var ConsumingValue = from x in xDoc2.Descendants("Product") select x.Attribute("consuming").Value;
                string[] ConsumingArray = ConsumingValue.ToArray();
                //Creating
                var CreatingValue = from x in xDoc2.Descendants("Product") select x.Attribute("creating").Value;
                string[] CreatingArray = CreatingValue.ToArray();
                //Consumed
                var ConsumedValue = from x in xDoc2.Descendants("Product") select x.Attribute("consumed").Value;
                string[] ConsumedArray = ConsumedValue.ToArray();
                //Created
                var CreatedValue = from x in xDoc2.Descendants("Product") select x.Attribute("created").Value;
                string[] CreatedArray = CreatedValue.ToArray();
                //ShippedLastDay
                var ShippedLastDayValue = from x in xDoc2.Descendants("Product") select x.Attribute("shippedLastDay").Value;
                string[] ShippedLastDayArray = ShippedLastDayValue.ToArray();
                //ShippedLastWeek
                var ShippedLastWeekValue = from x in xDoc2.Descendants("Product") select x.Attribute("shippedLastWeek").Value;
                string[] ShippedLastWeekArray = ShippedLastWeekValue.ToArray();
                //ShippedLast4Weeks
                var ShippedLast4WeeksValue = from x in xDoc2.Descendants("Product") select x.Attribute("shippedLast4Weeks").Value;
                string[] ShippedLast4WeeksArray = ShippedLast4WeeksValue.ToArray();
                //OrderedLastDay
                var OrderedLastDayValue = from x in xDoc2.Descendants("Product") select x.Attribute("orderedLastDay").Value;
                string[] OrderedLastDayArray = OrderedLastDayValue.ToArray();
                //OrderedLastWeek
                var OrderedLastWeekValue = from x in xDoc2.Descendants("Product") select x.Attribute("orderedLastWeek").Value;
                string[] OrderedLastWeekArray = OrderedLastWeekValue.ToArray();
                //OrderedLast4Weeks
                var OrderedLast4WeeksValue = from x in xDoc2.Descendants("Product") select x.Attribute("orderedLast4Weeks").Value;
                string[] OrderedLast4WeeksArray = OrderedLast4WeeksValue.ToArray();

                //Close XMLReader
                reader.Close();
                responseStream.Close();
                //response.Close();

                //Pass 18 attributes to ViewBag so that View can make use of 18 attributes
                ViewBag.Name = xDoc;
                ViewBag.TotalProducts = totalproducts;

                ViewBag.Code = CodeArray;
                ViewBag.Quantity = QuantityArray;
                ViewBag.Pending = PendingArray;
                ViewBag.Good = GoodArray;
                ViewBag.Backordered = BackorderedArray;
                ViewBag.Reserved = ReservedArray;
                ViewBag.Shipping = ShippingArray;
                ViewBag.Shipped = ShippedArray;
                ViewBag.Consuming = ConsumingArray;
                ViewBag.Creating = CreatingArray;
                ViewBag.Consumed = ConsumedArray;
                ViewBag.Created = CreatedArray;
                ViewBag.ShippedLastDay = ShippedLastDayArray;
                ViewBag.ShippedLastWeek = ShippedLastWeekArray;
                ViewBag.ShippedLast4Weeks = ShippedLast4WeeksArray;
                ViewBag.OrderedLastDay = OrderedLastDayArray;
                ViewBag.OrderedLastWeek = OrderedLastWeekArray;
                ViewBag.OrderedLast4Weeks = OrderedLast4WeeksArray;


            }
            response.Close();
            return View();
        }
    }
}