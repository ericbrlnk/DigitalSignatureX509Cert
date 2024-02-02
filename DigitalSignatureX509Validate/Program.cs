using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DigitalSignatureX509Sign;

namespace DigitalSignatureX509Validate
{
    public class Program
    {
        private static string currentPath = Directory.GetCurrentDirectory();
        private static string certName = "CN=CERT_SIGN";
        static void Main(string[] args)
        {
            try
            {
                // added as a link to an existing file
                string xmlString = File.ReadAllText("SignedDocument.xml");
                XmlDocument xmlDoc = new XmlDocument();
                // load string to xml-Doc
                xmlDoc.LoadXml(xmlString);
                Console.WriteLine(xmlString);

                SignedXml signedXml = new SignedXml(xmlDoc);

                XmlNodeList signatureNodes = xmlDoc.GetElementsByTagName("Signature");
                XmlNodeList certNodes = xmlDoc.GetElementsByTagName("X509Certificate");

                X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(certNodes[0].InnerText));

                foreach (XmlElement element in signatureNodes)
                {
                    // signature nodes
                    signedXml.LoadXml(element);
                    bool result = signedXml.CheckSignature(cert, true);
                    Console.WriteLine("Valid signature: " + result.ToString());
                }
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
