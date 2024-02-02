using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

// X509 Certificate is created using makecert.exe
// The certificate is placed in user store
// In order to generate an exchange key, following command should be used (in VS-terminal):
// makecert -r -pe -a sha256 -n "CN=CERT_SIGN" -b 01/01/2022 -e 01/01/2024 -sky exchange -ss my

namespace DigitalSignatureX509Sign
{
    class Program
    {
        // path variables
        private static string uri = "https://www.w3schools.com/xml/simple.xml";
        private static string certName = "CN=CERT_SIGN";
        private static string currentPath = Directory.GetCurrentDirectory();
        private static string fileName = "SignedDocument.xml";

        static void Main(string[] args)
        {
            // those two lines are neccessary in .NET Framework 4.7.2
            // it's because SHA256 has been used for signature computing
            // this code restores SHA1
            // AppContext.SetSwitch("Switch.System.Security.Cryptography.Xml.UseInsecureHashAlgorithms", true);
            // AppContext.SetSwitch("Switch.System.Security.Cryptography.Pkcs.UseInsecureHashAlgorithms", true);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;

            using (WebClient client = new WebClient())
            {
                byte[] xmlArray = client.DownloadData(uri);
                xmlDoc.LoadXml(Encoding.Default.GetString(xmlArray));
                Console.WriteLine(Encoding.UTF8.GetString(xmlArray));
            }

            X509Certificate2 cert = GetCertificateFromTheStore(certName);

            if (cert == null)
            {
                Console.WriteLine("Certificate " + certName + " not found.");
                Console.ReadLine();
            }

            SignXmlFile(xmlDoc, cert);
            File.WriteAllText(Path.Combine(currentPath, fileName), xmlDoc.OuterXml);
            Console.WriteLine("File signed");
            Console.ReadLine();
        }

        private static void SignXmlFile(XmlDocument xmlDoc, X509Certificate2 cert)
        {
            SignedXml signedXML = new SignedXml(xmlDoc);
            // sign with the private key
            signedXML.SigningKey = cert.GetRSAPrivateKey();

            // add reference to be signed
            Reference reference = new Reference();
            reference.Uri = "";

            // add transformation to the reference
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXML.AddReference(reference);

            // add key-info
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXML.KeyInfo = keyInfo;

            signedXML.ComputeSignature();

            // get digital signature in XML form
            XmlElement xmlSignature = signedXML.GetXml();

            // append signature to the XML doc
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlSignature, true));
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
