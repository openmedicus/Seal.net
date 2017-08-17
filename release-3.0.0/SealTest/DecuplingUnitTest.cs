using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using dk.nsi.fmk.decoupling;
using dk.nsi.fmk.service;
using dk.nsi.seal;
using NUnit.Framework;
using SealTest;
using Attribute = dk.nsi.fmk.decoupling.Attribute;
using AttributeStatement = dk.nsi.fmk.decoupling.AttributeStatement;
using AttributeStatementID = dk.nsi.fmk.decoupling.AttributeStatementID;
using Conditions = dk.nsi.fmk.decoupling.Conditions;
using ConfirmationMethod = dk.nsi.fmk.decoupling.ConfirmationMethod;
using FlowStatus = dk.nsi.fmk.decoupling.FlowStatus;
using Header = dk.nsi.fmk.decoupling.Header;
using KeyInfo = dk.nsi.fmk.decoupling.KeyInfo;
using Linking = dk.nsi.fmk.decoupling.Linking;
using MedicineCardRequestStructureType = dk.nsi.fmk.decoupling.MedicineCardRequestStructureType;
using MedicineCardResponseType2 = dk.nsi.fmk.decoupling.MedicineCardResponseType2;
using NameFormat = dk.nsi.fmk.decoupling.NameFormat;
using OnBehalfOfStructureType = dk.nsi.fmk.decoupling.OnBehalfOfStructureType;
using OrgUsingID = dk.nsi.fmk.decoupling.OrgUsingID;
using PrescriptionReplicationStatusStructureType = dk.nsi.fmk.decoupling.PrescriptionReplicationStatusStructureType;
using Priority = dk.nsi.fmk.decoupling.Priority;
using RequireNonRepudiationReceipt = dk.nsi.fmk.decoupling.RequireNonRepudiationReceipt;
using Security = dk.nsi.fmk.decoupling.Security;
using Subject = dk.nsi.fmk.decoupling.Subject;
using SubjectConfirmation = dk.nsi.fmk.decoupling.SubjectConfirmation;
using SubjectConfirmationData = dk.nsi.fmk.decoupling.SubjectConfirmationData;
using TimeOut = dk.nsi.fmk.decoupling.TimeOut;
using Timestamp = dk.nsi.fmk.decoupling.Timestamp;
using TimingStructureType = dk.nsi.fmk.decoupling.TimingStructureType;

namespace SealTest.decoupling
{
    //Bemærk at dk.nsi.fmk.FMK.StartService(); kræver at Visual Studio kører i administrator tilstand
    // Hvis du gerne vil slippe for at køre VS i admin tilstand kan du køre følgende:
    //    netsh http add urlacl url=http://+:1010/FMK/ user=DOMAIN\user
    //    https://msdn.microsoft.com/library/ms733768.aspx
    [TestFixture]
    public class DecuplingUnitTest
    {
        public DecuplingUnitTest()
        {
            FMK.StartService();
        }

        [Test]
        [Ignore("")]
        public void DecuplingTest()
        {
            var client = new DccMedicineCardPortTypeClient
            {
                FMKHeader = MakeHeader(),
                FMKSecurity = MakeSecurity(MakeAssertion()),
                cert = Global.cert
            };

            //client.Endpoint.EndpointBehaviors.Add(new SealEndpointBehavior());

            PrescriptionReplicationStatusStructureType pres;
            MedicineCardResponseType2 cardtypes;
            TimingStructureType[] tst;

            var tt = client.GetMedicineCard_20120101(
                new OnBehalfOfStructureType
                {
                    AuthorisationIdentifier = Global.AuthIds[0]
                },
                "CompugroupMedical Danmark",
                "XMO",
                "7.2",
                "CompuGroup Medical Denmark",
                "CompuGroup Medical Denmark",
                new OrgUsingID
                {
                    Value = "CompuGroup Medical Denmark",
                    NameFormat = NameFormat.medcomcvrnumber
                },
                Global.AuthIds[0],
                new DecouplingHeader(),
                new MedicineCardRequestStructureType
                {
                    PersonCivilRegistrationIdentifier = Global.PatientCprs[1]
                },
                out tst,
                out pres,
                out cardtypes);
        }

        [Test]
        [Ignore("")]
        public void DecuplingTest1()
        {
            //Direkte kald
            var client = new DccMedicineCardPortTypeClient("SosiGW")
            {
                FMKHeader = MakeHeader(),
                FMKSecurity = MakeSecurity(MakeAssertion()),
                cert = Global.cert
            };

            //client.Endpoint.EndpoinBtehaviors.Add(new SealEndpointBehavior());

            PrescriptionReplicationStatusStructureType pres;
            MedicineCardResponseType2 cardtypes;
            TimingStructureType[] tst;

            var tt = client.GetMedicineCard_20120101(
                new OnBehalfOfStructureType
                {
                    AuthorisationIdentifier = Global.AuthIds[0]
                },
                "CompugroupMedical Danmark",
                "XMO",
                "7.2",
                "CompuGroup Medical Denmark",
                "CompuGroup Medical Denmark",
                new OrgUsingID
                {
                    Value = "CompuGroup Medical Denmark",
                    NameFormat = NameFormat.medcomcvrnumber
                },
                Global.AuthIds[0],
                new DecouplingHeader(),
                new MedicineCardRequestStructureType
                {
                    PersonCivilRegistrationIdentifier = Global.PatientCprs[1]
                },
                out tst,
                out pres,
                out cardtypes);
        }

        [Test]
        [Ignore("")]
        public void TestSosiGWExchangeViaApi2()
        {
            var sc = SealCard.Create(MakeAssertion());
            using (var stsClient = new SosiGwCardClient("SosiGW"))
            {
                var d = stsClient.ExchangeAssertion(sc, "http://sundhed.dk/") as GenericXmlSecurityToken;
                var elm = d.TokenXml;
            }
        }

        private static Security MakeSecurity(AssertionType assertion)
        {
            return new Security
            {
                id = Guid.NewGuid().ToString("D"),
                Timestamp = new Timestamp {Created = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5)},
                Assertion = assertion
            };
        }

        private static AssertionType MakeAssertion()
        {
            var vnow = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5);

            return new AssertionType
            {
                IssueInstant = vnow,
                id = "IDCard",
                Version = 2.0m,
                Issuer = "TESTSTS",
                Conditions = new Conditions
                {
                    NotBefore = vnow,
                    NotOnOrAfter = vnow + TimeSpan.FromHours(8)
                },
                Subject = new Subject
                {
                    NameID = new NameIDType
                    {
                        Format = SubjectIdentifierType.medcomcprnumber,
                        Value = "2408631478"
                    },
                    SubjectConfirmation = new SubjectConfirmation
                    {
                        ConfirmationMethod = ConfirmationMethod.urnoasisnamestcSAML20cmholderofkey,
                        SubjectConfirmationData = new SubjectConfirmationData
                        {
                            Item = new KeyInfo
                            {
                                Item = "OCESSignature"
                            }
                        }
                    }
                },
                AttributeStatement = new[]
                {
                    new AttributeStatement
                    {
                        id = AttributeStatementID.IDCardData,
                        Attribute = new[]
                        {
                            new Attribute {Name = AttributeName.sosiIDCardID, AttributeValue = Guid.NewGuid().ToString("D")},
                            new Attribute {Name = AttributeName.sosiIDCardVersion, AttributeValue = "1.0.1"},
                            new Attribute {Name = AttributeName.sosiIDCardType, AttributeValue = "user"},
                            new Attribute {Name = AttributeName.sosiAuthenticationLevel, AttributeValue = "4"},
                            new Attribute {Name = AttributeName.sosiOCESCertHash, AttributeValue = Global.cert.GetCertHashString()}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.UserLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = AttributeName.medcomUserCivilRegistrationNumber, AttributeValue = "2408631478"},
                            new Attribute {Name = AttributeName.medcomUserGivenName, AttributeValue = "Amaja Christiansen"},
                            new Attribute {Name = AttributeName.medcomUserSurName, AttributeValue = "-"},
                            new Attribute {Name = AttributeName.medcomUserEmailAddress, AttributeValue = "jso@trifork.com"},
                            new Attribute {Name = AttributeName.medcomUserRole, AttributeValue = "1234"},
                            new Attribute {Name = AttributeName.medcomUserAuthorizationCode, AttributeValue = "12345"}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.SystemLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = AttributeName.medcomITSystemName, AttributeValue = "Sygdom.dk"},
                            new Attribute {Name = AttributeName.medcomCareProviderID, AttributeValue = "25520041", NameFormat = SubjectIdentifierType.medcomcvrnumber},
                            new Attribute {Name = AttributeName.medcomCareProviderName, AttributeValue = "TRIFORK SERVICES A/S // CVR:25520041"}
                        }
                    }
                }
            };
        }

        private static Header MakeHeader()
        {
            return new Header
            {
                SecurityLevel = 3,
                TimeOut = TimeOut.Item1440,
                TimeOutSpecified = true,
                Linking = new Linking
                {
                    FlowID = Guid.NewGuid().ToString("D"),
                    MessageID = Guid.NewGuid().ToString("D")
                },
                FlowStatus = FlowStatus.flow_running,
                FlowStatusSpecified = true,
                Priority = Priority.RUTINE,
                RequireNonRepudiationReceipt = RequireNonRepudiationReceipt.yes
            };
        }
    }
}

namespace dk.nsi.fmk.decoupling
{
    public partial class DccMedicineCardPortTypeClient
    {
        public Security FMKSecurity;
        public Header FMKHeader;
        public X509Certificate2 cert;

        public void DoLogin(string DigestValue)
        {
            var csp = (RSACryptoServiceProvider) cert.PrivateKey;
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Convert.FromBase64String(DigestValue));
            var rb = new signIdCardRequestBody
            {
                SignatureValue = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")),
                KeyInfo = new KeyInfo
                {
                    Item = new X509Data {Item = cert.Export(X509ContentType.Cert)}
                }
            };
            if (signIdCard(FMKSecurity, FMKHeader, rb) != signIdCardResponse.ok)
            {
                throw new Exception("Gateway logon error");
            }
        }

        public implicitLoginHeader GetMedicineCard_20120101(OnBehalfOfStructureType OnBehalfOfStructure,
            string SystemOwnerName,
            string SystemName,
            string SystemVersion,
            string OrgResponsibleName,
            string OrgUsingName,
            OrgUsingID OrgUsingID,
            string RequestedRole,
            DecouplingHeader DecouplingHeader,
            MedicineCardRequestStructureType MedicineCardRequestStructure,
            out TimingStructureType[] TimingListStructure,
            out PrescriptionReplicationStatusStructureType PrescriptionReplicationStatusStructure,
            out MedicineCardResponseType2 MedicineCardResponseStructure)
        {
            try
            {
                return GetMedicineCard_20120101(FMKSecurity, FMKHeader, OnBehalfOfStructure, SystemOwnerName, SystemName, SystemVersion, OrgResponsibleName, OrgUsingName,
                    OrgUsingID, RequestedRole, DecouplingHeader, MedicineCardRequestStructure, out TimingListStructure, out PrescriptionReplicationStatusStructure,
                    out MedicineCardResponseStructure);
            }
            catch (FaultException<SosiGWLoginError> ex)
            {
                DoLogin(ex.Detail.DigestValue);
                return GetMedicineCard_20120101(FMKSecurity, FMKHeader, OnBehalfOfStructure, SystemOwnerName, SystemName, SystemVersion, OrgResponsibleName, OrgUsingName,
                    OrgUsingID, RequestedRole, DecouplingHeader, MedicineCardRequestStructure, out TimingListStructure, out PrescriptionReplicationStatusStructure,
                    out MedicineCardResponseStructure);
            }
        }
    }
}