using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PostalCodeServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UdpClient server = new UdpClient(51111);
            Console.WriteLine("Server is running...");

            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 52222);
                byte[] buffer = server.Receive(ref clientEndPoint);
                string postalCode = Encoding.UTF8.GetString(buffer);

                Console.WriteLine($"Postal Code Receiving: {postalCode}");

                List<string> streets = GetStreetsByPostalCode(postalCode);
                string response = string.Join(",", streets);

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                server.Send(responseBytes, responseBytes.Length, clientEndPoint);
            }
        }
        static List<string> GetStreetsByPostalCode(string postalCode)
        {
            XDocument doc = XDocument.Load("streets.xml");
            var streets = doc.Descendants("IndexPostOfficeStreet")
                             .Where(s => s.Element("IndexPostOffice").Value == postalCode)
                             .Select(s => s.Element("Street").Value)
                             .ToList();
            return streets;
        }
    }
}
