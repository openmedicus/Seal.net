using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dk.nsi.seal.Model;
using dk.nsi.seal.Model.DomBuilders;
using dk.nsi.seal.Model.ModelBuilders;

namespace dk.nsi.seal.Factories
{
    public class OIOSAMLFactory
    {
		/*
		// later
		// OIO Bootstrap token to IDWS Identity Token
		public OIOBootstrapToIdentityTokenRequestDOMBuilder createOIOBootstrapToIdentityTokenRequestDOMBuilder()
		{
			return new OIOBootstrapToIdentityTokenRequestDOMBuilder();
		}

		public OIOBootstrapToIdentityTokenRequestModelBuilder createOIOBootstrapToIdentityTokenRequestModelBuilder()
		{
			return new OIOBootstrapToIdentityTokenRequestModelBuilder();
		}

		public OIOBootstrapToIdentityTokenResponseDOMBuilder createOIOBootstrapToIdentityTokenResponseDOMBuilder()
		{
			return new OIOBootstrapToIdentityTokenResponseDOMBuilder();
		}

		public OIOBootstrapToIdentityTokenResponseModelBuilder createOIOBootstrapToIdentityTokenResponseModelBuilder()
		{
			return new OIOBootstrapToIdentityTokenResponseModelBuilder();
		}
		// end
		*/
		
		/**
     * Creates a new <code>OIOSAMLAssertionToIDCardRequestDOMBuilder</code>
     *
     * @return  The newly created <code>OIOSAMLAssertionToIDCardRequestDOMBuilder</code>
     */
	 
		public OioSamlAssertionToIdCardRequestDomBuilder CreateOiosamlAssertionToIdCardRequestDomBuilder()
		{
			return new OioSamlAssertionToIdCardRequestDomBuilder();
		}

		/**
		 * Creates a new <code>OIOSAMLAssertionToIDCardRequestModelBuilder</code>
		 *
		 * @return  The newly created <code>OIOSAMLAssertionToIDCardRequestModelBuilder</code>
		 */
		public OioSamlAssertionToIdCardRequestModelBuilder CreateOioSamlAssertionToIdCardRequestModelBuilder()
		{
			return new OioSamlAssertionToIdCardRequestModelBuilder();
		}


		/**
   * Creates a new <code>IDCardToOIOSAMLAssertionRequestDOMBuilder</code>
   *
   * @return  The newly created <code>IDCardToOIOSAMLAssertionRequestDOMBuilder</code>
   */
		public IdCardToOioSamlAssertionRequestDomBuilder CreateIdCardToOioSamlAssertionRequestDomBuilder()
		{
			return new IdCardToOioSamlAssertionRequestDomBuilder();
		}

		/**
		 * Creates a new <code>IDCardToOIOSAMLAssertionRequestModelBuilder</code>
		 *
		 * @return  The newly created <code>IDCardToOIOSAMLAssertionRequestModelBuilder</code>
		 */
		public IdCardToOioSamlAssertionRequestModelBuilder CreateIdCardToOioSamlAssertionRequestModelBuilder()
		{
			return new IdCardToOioSamlAssertionRequestModelBuilder();
		}

		/**
  * Creates a new <code>OIOSAMLAssertionBuilder</code>
  *
  * @return  The newly created <code>OIOSAMLAssertionBuilder</code>
  */

		public OioSamlAssertionBuilder CreateOioSamlAssertionBuilder()
		{
			return new OioSamlAssertionBuilder();
		}

		//public CitizenIdentityTokenBuilder CreateCitizenIdentityTokenBuilder()
		//{
		//	return new CitizenIdentityTokenBuilder();
		//}

	}
}
