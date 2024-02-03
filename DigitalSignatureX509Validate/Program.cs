using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

                bool result = ValidateXML(xmlDoc);
                if (result)
                {
                    Console.WriteLine("Valid signature: " + result.ToString());
                }
                else
                {
                    Console.WriteLine("Invalid signature");
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

        public static bool ValidateXML(XmlDocument xmlDoc)
        {
            bool result = false;

            if (xmlDoc == null)
            {
                throw new ArgumentException("Argument exception: ", nameof(xmlDoc));
            }

            SignedXml signedXml = new SignedXml(xmlDoc);

            XmlNodeList signatureNodes = xmlDoc.GetElementsByTagName("Signature");
            XmlNodeList certNodes = xmlDoc.GetElementsByTagName("X509Certificate");

            if (signatureNodes.Count == 0)
            {
                throw new CryptographicException("No signature found.");
            }

            if (certNodes.Count == 0)
            {
                throw new CryptographicException("No certificate found.");
            }

            X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(certNodes[0].InnerText));
            Console.WriteLine("Extracted certificate:");
            Console.WriteLine(certNodes[0].InnerText);

            foreach (XmlElement element in signatureNodes)
            {
                // signature nodes
                signedXml.LoadXml(element);
                result = signedXml.CheckSignature(cert, true);
            }

            return result;
        }

    }
}
