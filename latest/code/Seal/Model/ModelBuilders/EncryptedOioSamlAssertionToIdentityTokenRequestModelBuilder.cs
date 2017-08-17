using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Model.Requests;

namespace dk.nsi.seal.Model.ModelBuilders
{
	public class EncryptedOioSamlAssertionToIdentityTokenRequestModelBuilder
	{

		public EncryptedOioSamlAssertionToIdentityTokenRequest Build(XDocument doc)
		{
			return new EncryptedOioSamlAssertionToIdentityTokenRequest(doc);
		}
	}
}
