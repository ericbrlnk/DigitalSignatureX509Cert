using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DigitalSignatureX509Validate
{
    class Program
    {
        private static string currentPath = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            try
            {
                // added as a link to an existing file
                string xmlString = File.ReadAllText("SignedDocument.xml");
                XmlDocument xmlDoc = new XmlDocument();
                // load string to xml-Doc
                xmlDoc.LoadXml(xmlString);

                X509Certificate2 publicCert = new X509Certificate2();

                Console.WriteLine(xmlString);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
