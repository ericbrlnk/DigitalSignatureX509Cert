using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

// X509 Certificate is created using makecert.exe
// The certificate is placed in user store
// In order to generate an exchange key, following command should be used:
// makecert -r -pe -n "CN=CERT_SIGN" -b 01/01/2022 -e 01/01/2024 -sky exchange -ss my

namespace DigitalSignatureX509Sign
{
    class Program
    {
        // path variables
        private static string uri = "https://www.w3schools.com/xml/simple.xml";
        private static string certName = "CN = CERT_SIGN";

        static void Main(string[] args)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;


            using (WebClient client = new WebClient())
            {
                byte[] xmlArray = client.DownloadData(uri);
                xmlDoc.Load(Encoding.Default.GetString(xmlArray));
                Console.WriteLine(Encoding.UTF8.GetString(xmlArray));
            }

            X509Certificate2 cert = GetCertificateFromTheStore(certName);

            if (cert == null)
            {
                Console.WriteLine("Certificate " + certName + " not found."); 
            }

            SignXmlFile(xmlDoc, cert);
            

            Console.ReadLine();
        }

        private static void SignXmlFile(XmlDocument xmlDoc, X509Certificate cert)
        {


        }

        private static X509Certificate2 GetCertificateFromTheStore(string name)
        {
            X509Store store = new X509Store(StoreLocation.CurrentUser);

            try
            {
                store.Open(OpenFlags.ReadOnly);
                // store all certificates in a collection
                X509Certificate2Collection certs = store.Certificates;
                X509Certificate2Collection currentCerts = certs.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindByIssuerDistinguishedName, name, false);

                if (signingCert.Count == 0)
                {
                    return null;
                }
                // return the first certificate in a collection
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
