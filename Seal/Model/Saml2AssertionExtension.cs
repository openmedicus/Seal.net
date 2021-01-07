using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal.Model
{
	public static class Saml2AssertionExtension
	{
		public static string GetAttributeValue(this Saml2Assertion ass, string attributeName)
		{
			foreach (var contextTokenStatement in ass.Statements)
			{
				var attributeStatement = contextTokenStatement as Saml2AttributeStatement;
				if (attributeStatement != null)
				{
					foreach (var attributeStatementAttribute in attributeStatement.Attributes)
					{
						if (attributeStatementAttribute.Name == attributeName)
						{
							return attributeStatementAttribute.Values.FirstOrDefault();
						}
					}
				}
			}
			return "";
		}
	}
}
