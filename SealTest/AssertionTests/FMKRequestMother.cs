using System;
using System.Collections.Generic;
using System.Xml;
using dk.nsi.seal;
using SealTest.MedicineCardService;

namespace SealTest.AssertionTests
{
    public class FMKRequestMother
    {
        public static GetMedicineCardRequest_2015_06_01 GetMedicineCardRequest20150601(string userCpr, SealCard ass)
        {
            return new GetMedicineCardRequest_2015_06_01(
                Security: MakeSecurity(ass, Guid.NewGuid()),
                Header: MakeHeader(),
                OnBehalfOf: GetOnBehalfOf(userCpr),
                WhitelistingHeader: GetWhitelistingHeader(),
                ConsentHeader: null,
                GetMedicineCardRequest: GetGetMedicineCardRequest());
        }

        public static GetMedicineCardRequestType GetGetMedicineCardRequest()
        {
            return new GetMedicineCardRequestType()
            {
                PersonIdentifier = new PersonIdentifierType { source = "CPR", Value = "2603558084" }
            };
        }

        public static OnBehalfOfType GetOnBehalfOf(string userCpr)
        {
            return new OnBehalfOfType()
            {
                Item = new PersonIdentifierType()
                { source = "CPR", Value = userCpr }
            };
        }

        public static WhitelistingHeader GetWhitelistingHeader()
        {
            return new WhitelistingHeader
            {
                SystemOwnerName = "Trifork",
                SystemName = "Trifork146Only",
                SystemVersion = "1.5",
                RequestedRole = "Læge",
                Items = new List<object>
                {
                    "LægepraksisleverandørXYZ",
                    "ROS Infektionsmedicinsk Amb.",
                    new OrgUsingID
                    {
                        Value = "3800W7D",
                        NameFormat = NameFormat.medcomskscode
                    }
                }.ToArray(),
                ItemsElementName = new List<ItemsChoiceType9>
                {
                    ItemsChoiceType9.OrgResponsibleName,
                    ItemsChoiceType9.OrgUsingName,
                    ItemsChoiceType9.OrgUsingID
                }.ToArray()
            };
        }

        public static SecurityHeaderType MakeSecurity(SealCard card, Guid id)
        {
            var assertionDoc = new XmlDocument();
            var assertionElement = assertionDoc.ReadNode(card.Xassertion.CreateReader()) as XmlElement;

            var timestampDoc = new XmlDocument();

            var createdElement = timestampDoc.CreateElement("Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            // vær opmærkesom på at det er forskelligt fra dgws 1.0.1 til 1.1 om timestamps skal være i lokal tid eller i UTC !
            createdElement.InnerText = (DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5)).ToLocalTime().ToString("yyyy-MM-ddThh:mm:sszzz");

            var timestampElement = timestampDoc.CreateElement("Timestamp", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            timestampElement.AppendChild(createdElement);

            var idAttribute = new XmlDocument().CreateAttribute("id");
            idAttribute.Value = id.ToString("D");

            return new SecurityHeaderType
            {
                Any = new[] {
                    timestampElement,
                    assertionElement
                },
                AnyAttr = new[] {
                    idAttribute
                }
            };
        }

        public static Header MakeHeader()
        {
            return new Header
            {
                SecurityLevel = 3,
                SecurityLevelSpecified = true,
                //TimeOut = TimeOut.Item1440,
                //TimeOutSpecified = true,
                Linking = new Linking
                {
                    //FlowID = Guid.NewGuid().ToString("D"),
                    MessageID = Guid.NewGuid().ToString("D")
                },
                //FlowStatus = FlowStatus.flow_running,
                //FlowStatusSpecified = true,
                Priority = Priority.RUTINE,
                RequireNonRepudiationReceipt = RequireNonRepudiationReceipt.yes,
                RequireNonRepudiationReceiptSpecified = true
            };
        }
    }
}