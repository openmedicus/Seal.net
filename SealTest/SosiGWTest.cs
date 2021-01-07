using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using dk.nsi.fmk;
using dk.nsi.seal;
using NUnit.Framework;
using SealTest.AssertionTests;
//using SealTest.FMKService;
using Assertion = dk.nsi.fmk.Assertion;
using FlowStatus = dk.nsi.fmk.FlowStatus;
using GW = SealTest.SosiGWReference;
using Header = dk.nsi.fmk.Header;
using Linking = dk.nsi.fmk.Linking;
using Priority = dk.nsi.fmk.Priority;
using RequireNonRepudiationReceipt = dk.nsi.fmk.RequireNonRepudiationReceipt;
using Security = dk.nsi.fmk.Security;
using TimeOut = dk.nsi.fmk.TimeOut;
using Timestamp = dk.nsi.fmk.Timestamp;

namespace SealTest
{
    [TestFixture]
    public class SosiGwTest
    {
        private Assertion _assertion;
        private Header _header;

        [Test]
        [Ignore("Needs to update FMK to 1.4.6")]
        public void SosiGatewayTest()
        {
            DoLogin(Global.MocesCprGyldig);

            var client = new MedicineCardPortTypeClient("SosiGWFMK");
            client.GetMedicineCard_20120101(MakeSecurity(_assertion), _header);
        }


        /*[Test]
        public void GatewaySecureBrowserLoginTest()
        {
            DoLogin(Global.MocesCprGyldig);

            using (var stsClient = new Seal2SamlStsClient("GWFetchCard"))
            using (var scope = new OperationContextScope((IContextChannel) stsClient.Channel.Channel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SealCardMessageHeader(SealCard.Create(_assertion)));
                var d = stsClient.ExchangeAssertionViaGW("http://sundhed.dk/") as GenericXmlSecurityToken;
                var elm = d.TokenXml;
            }
        }*/


        public void DoLogin(X509Certificate2 cert)
        {
            var gwClient = new GW.SosiGWFacadeClient();
            var sec = MakeSecurity(MakeAssertionForSTS());
            var dig = gwClient.requestIdCardDigestForSigning(sec, "whatever");

            var csp = (RSACryptoServiceProvider) cert.PrivateKey;
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(dig.DigestValue);
            var rb = new GW.signIdCardRequestBody
            {
                SignatureValue = csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")),
                KeyInfo = new GW.KeyInfo
                {
                    Item = new GW.X509Data {Item = cert.Export(X509ContentType.Cert)}
                }
            };

            var res = gwClient.signIdCard(sec, rb);
            if (res != GW.signIdCardResponse.ok)
            {
                throw new Exception("Gateway logon error");
            }
            _header = MakeHeader();
            _assertion = SealCard.Create(sec.Assertion).GetAssertion<Assertion>(typeof(GW.AssertionType).Name);
        }

        private static GW.Security MakeSecurity(GW.AssertionType assertion)
        {
            return new GW.Security
            {
                id = Guid.NewGuid().ToString("D"),
                Timestamp = new GW.Timestamp {Created = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5)},
                Assertion = assertion
            };
        }

        private static Security MakeSecurity(Assertion assertion)
        {
            return new Security
            {
                id = Guid.NewGuid().ToString("D"),
                Timestamp = new Timestamp {Created = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5)},
                Assertion = assertion
            };
        }

        private static GW.AssertionType MakeAssertionForSTS()
        {
            var vnow = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5);
            var later = (vnow + TimeSpan.FromHours(8)).ToString("u").Replace(' ', 'T');

            var ass = new GW.AssertionType
            {
                IssueInstant = vnow,
                id = "IDCard",
                Version = 2.0m,
                Issuer = "WinPLC",
                Conditions = new GW.Conditions
                {
                    NotBefore = vnow,
                    NotOnOrAfter = vnow + TimeSpan.FromHours(8)
                },
                Subject = new GW.Subject
                {
                    NameID = new GW.NameIDType
                    {
                        Format = GW.SubjectIdentifierType.medcomcprnumber,
                        Value = "2203333571"
                    },
                    SubjectConfirmation = new GW.SubjectConfirmation
                    {
                        ConfirmationMethod = GW.ConfirmationMethod.urnoasisnamestcSAML20cmholderofkey,
                        SubjectConfirmationData = new GW.SubjectConfirmationData
                        {
                            Item = new GW.KeyInfo
                            {
                                Item = "OCESSignature"
                            }
                        }
                    }
                },
                AttributeStatement = new[]
                {
                    new GW.AttributeStatement
                    {
                        id = GW.AttributeStatementID.IDCardData,
                        Attribute = new[]
                        {
                            new GW.Attribute {Name = GW.AttributeName.sosiIDCardID, AttributeValue = Guid.NewGuid().ToString("D")},
                            new GW.Attribute {Name = GW.AttributeName.sosiIDCardVersion, AttributeValue = "1.0.1"},
                            new GW.Attribute {Name = GW.AttributeName.sosiIDCardType, AttributeValue = "user"},
                            new GW.Attribute {Name = GW.AttributeName.sosiAuthenticationLevel, AttributeValue = "4"}
                        }
                    },
                    new GW.AttributeStatement
                    {
                        id = GW.AttributeStatementID.UserLog,
                        Attribute = new[]
                        {
                            new GW.Attribute {Name = GW.AttributeName.medcomUserCivilRegistrationNumber, AttributeValue = "1802602810"},
                            new GW.Attribute {Name = GW.AttributeName.medcomUserGivenName, AttributeValue = "Stine"},
                            new GW.Attribute {Name = GW.AttributeName.medcomUserSurName, AttributeValue = "Svendsen"},
                            new GW.Attribute {Name = GW.AttributeName.medcomUserEmailAddress, AttributeValue = "stineSvendsen@example.com"},
                            new GW.Attribute {Name = GW.AttributeName.medcomUserRole, AttributeValue = "læge"},
                            new GW.Attribute {Name = GW.AttributeName.medcomUserAuthorizationCode, AttributeValue = "ZXCVB"}
                        }
                    },
                    new GW.AttributeStatement
                    {
                        id = GW.AttributeStatementID.SystemLog,
                        Attribute = new[]
                        {
                            new GW.Attribute {Name = GW.AttributeName.medcomITSystemName, AttributeValue = "Sygdom.dk"},
                            new GW.Attribute
                            {
                                Name = GW.AttributeName.medcomCareProviderID,
                                AttributeValue = "30808460",
                                NameFormat = GW.SubjectIdentifierType.medcomcvrnumber,
                                NameFormatSpecified = true
                            },
                            new GW.Attribute {Name = GW.AttributeName.medcomCareProviderName, AttributeValue = "Statens Serum Institut"}
                        }
                    }
                }
            };

            return ass;
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