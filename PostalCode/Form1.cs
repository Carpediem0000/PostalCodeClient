using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PostalCode
{
    public partial class Form1 : Form
    {
        private string _streetsPath = @"D:\Study\СетевоеПрогр\PostalCodeClient\PostalCodeServer\bin\Debug\streets.xml";
        private static UdpClient _client;
        public Form1()
        {
            InitializeComponent();
            _client = new UdpClient();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load(_streetsPath);
            IEnumerable<string> postalCodes = doc.Descendants("IndexPostOfficeStreet")
                                                .Select(s => s.Element("IndexPostOffice").Value)
                                                .Distinct()
                                                .OrderBy(k => int.Parse(k));

            foreach (var item in postalCodes)
            {
                comboBox1.Items.Add(item);
            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbl_PostalCode.Text = $"Postal Code: {comboBox1.SelectedItem}";
            SendRequest(comboBox1.SelectedItem.ToString());
        }
        private void SendRequest(string postalCode)
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 51111);

                byte[] requestBytes = Encoding.UTF8.GetBytes(postalCode);
                _client.Send(requestBytes, requestBytes.Length, serverEndPoint);

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 52222);
                byte[] responseBytes = _client.Receive(ref remoteEndPoint);
                string response = Encoding.UTF8.GetString(responseBytes);

                DisplayStreets(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DisplayStreets(string response)
        {
            string[] streets = response.Split(',');
            listBox1.Items.Clear();
            foreach (var street in streets)
            {
                listBox1.Items.Add(street);
            }
        }
    }
}
