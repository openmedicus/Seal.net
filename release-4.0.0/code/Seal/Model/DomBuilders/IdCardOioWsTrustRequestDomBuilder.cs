using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Model.Constants;

namespace dk.nsi.seal.Model.DomBuilders
{
	public abstract class IdCardOioWsTrustRequestDomBuilder : OioWsTrustRequestDomBuilder
	{
		public UserIdCard IdCard { get; set; }
		public bool PlaceIdCardInSoapHeader { get; set; }


		protected override void ValidateBeforeBuild()
		{
			base.ValidateBeforeBuild();

			Validate("idcard", IdCard);
		}

		protected override void AddExtraHeaders(XElement header)
		{
			base.AddExtraHeaders(header);
			if (PlaceIdCardInSoapHeader)
			{
				var security = XmlUtil.CreateElement(WsseTags.Security);
				header.Add(security);
				AddIdCard(security);
			}
		}

		protected override void AddActAsTokens(XElement actAs)
		{
			if (PlaceIdCardInSoapHeader)
			{
				AddSecurityTokenReferenceToIdCard(actAs);
			}
			else
			{
				AddIdCard(actAs);
			}
		}

		private void AddIdCard(XElement parent)
		{
			parent.Add(IdCard.Xassertion);
		}

	}
}
