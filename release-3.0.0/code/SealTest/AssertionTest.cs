using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using dk.nsi.fmk.service;
using dk.nsi.seal;
using NUnit.Framework;
using SealTest.Properties;
using Assert = NUnit.Framework.Assert;

namespace SealTest
{
    using proxy = dk.nsi.fmk;

    //Bemærk at dk.nsi.fmk.FMK.StartService(); kræver at Visual Studio kører i administrator tilstand
    // Hvis du gerne vil slippe for at køre VS i admin tilstand kan du køre følgende:
    //    netsh http add urlacl url=http://+:1010/FMK/ user=DOMAIN\user
    //    https://msdn.microsoft.com/library/ms733768.aspx
    [TestFixture]
    public class AssertionTest
    {
        public AssertionTest()
        {
            FMK.StartService();
        }

        [Test]
        [Ignore("The request was invalid or malformed: The certificate that signed the security token is not trusted")]
        public void TestNemId2SealAssertion()
        {
            //Opretter et NemID som Saml2Assertion
            //Veksler til Sosi kort
            //Kalder lokal service med Sosikort
            var nemidAssertion = AssertionMaker.MakeNemIdAssertion(Global.cert);
            SealCard sc;
            using (var stsClient = new Saml2SosiStsClient("NemId"))
            {
                stsClient.ChannelFactory.Credentials.ClientCertificate.Certificate = Global.MOCES_cpr_gyldig;
                sc = stsClient.ExchangeAssertion(nemidAssertion,
                    SealUtilities.MakeHealthContextAssertion("Test Sundhed.dk", Global.cert.SubjectName.Name, "Sygdom.dk", "5GXFR"),
                    "http://sosi.dk");
            }
            var client = new proxy.MedicineCardPortTypeClient("localFMK");
            client.GetMedicineCard_20120101(MakeSecurity(sc.GetAssertion<proxy.Assertion>()), MakeHeader());
        }

        [Test]
        public void TestSBO()
        {
            //Opretter lokalt SOSIkort
            //Veksler til SOSI kort underskrevet af STS
            //Veksler til OIO-Saml XML
            var rsc = SealCard.Create(AssertionMaker.MakeAssertionForSTS(Global.MOCES_cpr_gyldig));
            var sc = SealUtilities.SignIn(rsc, "http://www.ribeamt.dk/EPJ", Settings.Default.SecurityTokenService);
            using (var stsClient = new Seal2SamlStsClient("Seal2EncSaml"))
            {
                stsClient.ChannelFactory.Credentials.ClientCertificate.Certificate = Global.MOCES_cpr_gyldig;
                var d = stsClient.ExchangeAssertion(sc, "http://sundhed.dk/") as GenericXmlSecurityToken;
                var elm = d.TokenXml;
            }
        }


        [Test]
        public void TestDirectCall()
        {
            //Test mod lokal FMK service med lokal genereret SOSI kort
            var client = new proxy.MedicineCardPortTypeClient("localFMK");
            var ass = SealUtilities.SignAssertion(AssertionMaker.MakeAssertion(), Global.MOCES_cpr_gyldig);
            client.GetMedicineCard_20120101(MakeSecurity(ass), MakeHeader());
        }

        [Test]
        public void TestSTSogFMKAssertionAsType()
        {
            //Seal kort oprettes 
            //FMK kaldes
            //Assertion overføres typestærkt
            var rsc = SealCard.Create(AssertionMaker.MakeAssertionForSTS(Global.MOCES_cpr_gyldig));
            var sc = SealUtilities.SignIn(rsc, "http://www.ribeamt.dk/EPJ", Settings.Default.SecurityTokenService);

            var client = new proxy.MedicineCardPortTypeClient("localFMK");
            client.GetMedicineCard_20120101(MakeSecurity(sc.GetAssertion<proxy.Assertion>()), MakeHeader());
        }

        [Test]
        public void TestSTSogFMKAssertionAsXml()
        {
            //Seal kort oprettes 
            //FMK kaldes
            //Assertion overføres via SealCard som XML
            var rsc = SealCard.Create(AssertionMaker.MakeAssertionForSTS(Global.MOCES_cpr_gyldig));
            var sc = SealUtilities.SignIn(rsc, "http://www.ribeamt.dk/EPJ", Settings.Default.SecurityTokenService);

            var client = new proxy.MedicineCardPortTypeClient("localFMK");
            using (var scope = new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SealCardMessageHeader(sc));
                client.GetMedicineCard_20120101(null, MakeHeader());
            }
        }

        [Test]
        public void TestAssertionSign()
        {
            var ass = SealUtilities.SignAssertion(AssertionMaker.MakeAssertion(), Global.MOCES_cpr_gyldig);
            Assert.IsTrue(SealUtilities.CheckAssertionSignature(ass));

            var sec = MakeSecurity(AssertionMaker.MakeAssertion());
            sec = SealUtilities.SignAssertion(sec, Global.MOCES_cpr_gyldig);
            Assert.IsTrue(SealUtilities.CheckAssertionSignature(sec));
        }

        private static proxy.Security MakeSecurity(proxy.Assertion assertion)
        {
            return new proxy.Security
            {
                id = Guid.NewGuid().ToString("D"),
                Timestamp = new proxy.Timestamp {Created = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5)},
                Assertion = assertion
            };
        }

        private static proxy.Header MakeHeader()
        {
            return new proxy.Header
            {
                SecurityLevel = 3,
                TimeOut = proxy.TimeOut.Item1440,
                TimeOutSpecified = true,
                Linking = new proxy.Linking
                {
                    FlowID = Guid.NewGuid().ToString("D"),
                    MessageID = Guid.NewGuid().ToString("D")
                },
                FlowStatus = proxy.FlowStatus.flow_running,
                FlowStatusSpecified = true,
                Priority = proxy.Priority.RUTINE,
                RequireNonRepudiationReceipt = proxy.RequireNonRepudiationReceipt.yes
            };
        }
    }

    internal static class ClientExt
    {
        public static void GetMedicineCard_20120101(this proxy.MedicineCardPortTypeClient client, proxy.Security sec, proxy.Header hd)
        {
            proxy.PrescriptionReplicationStatusStructureType pres;
            proxy.MedicineCardResponseType2 cardtypes;
            var tt = client.GetMedicineCard_20120101(
                sec,
                hd,
                new proxy.OnBehalfOfStructureType
                {
                    AuthorisationIdentifier = Global.AuthIds[0]
                },
                SystemOwnerName: "Trifork",
                SystemName: "Trifork146",
                SystemVersion: "7.2",
                OrgResponsibleName: "CompuGroup Medical Denmark",
                OrgUsingName: "CompuGroup Medical Denmark",
                OrgUsingID: new proxy.OrgUsingID
                {
                    Value = "CompuGroup Medical Denmark",
                    NameFormat = proxy.NameFormat.medcomcvrnumber
                },
                RequestedRole: Global.AuthIds[0],
                MedicineCardRequestStructure: new proxy.MedicineCardRequestStructureType
                {
                    PersonCivilRegistrationIdentifier = Global.PatientCprs[1]
                },
                PrescriptionReplicationStatusStructure: out pres,
                MedicineCardResponseStructure: out cardtypes);
        }
    }
}