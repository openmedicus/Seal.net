using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using NUnit.Framework;

namespace SealTest
{
    class Global
    {
        public static X509Certificate2 cert = new X509Certificate2(TestContext.CurrentContext.TestDirectory + "/Resources/VicValidVOCES.p12", "!234Qwer");

        public static X509Certificate2 MOCES_cpr_gyldig = new X509Certificate2(TestContext.CurrentContext.TestDirectory + "/Resources/MOCES_cpr_gyldig.p12", "Test1234");

        public static string[] AuthIds = { "NS101", "NS102", "NS103" };
        public static string[] PatientCprs = { "0411427781", "2911245178", "0510171632", "1403713968", "2908993384", "1703056748" };


        public static XElement SignedTokenXml()
        {
            return XElement.Load(TestContext.CurrentContext.TestDirectory + "/Resources/SignedToken.xml");
        }
    }
}