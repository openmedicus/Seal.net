using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Federation;
using dk.nsi.seal.Model;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.Model.DomBuilders;
using dk.nsi.seal.Model.ModelBuilders;
using dk.nsi.seal.Model.Requests;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace SealTest.Model
{
	public class OioSamlFactoryTest : AbstractTest
	{
		private ICredentialVault vocesVault;
		private ICredentialVault mocesVault;
		private OIOSAMLFactory factory;

		[SetUp]
		public void Init()
		{
			vocesVault = CredentialVaultTestUtil.GetVocesCredentialVault();
			mocesVault = CredentialVaultTestUtil.GetCredentialVault();
			factory = new OIOSAMLFactory();
		}

		[TearDown]
		public void TearDown()
		{
			vocesVault = null;
			mocesVault = null;
		}

		private OioSamlAssertion ParseOioSamlAssertion()
		{
			return new OioSamlAssertion(XElement.Load(NUnit.Framework.TestContext.CurrentContext.TestDirectory +
								 "/Resources/oiosaml-examples/test-new-nemlogin-authentication-assertion-2.xml"));
			//return new OioSamlAssertion(XElement.Load(NUnit.Framework.TestContext.CurrentContext.TestDirectory +
			//					 "/Resources/SignedToken.xml"));
		}

		[Test, Parallelizable(ParallelScope.None)]
		public void TestOioSamlToIdCardRequest()
		{
			var domBuilder = factory.CreateOiosamlAssertionToIdCardRequestDomBuilder();
			domBuilder.SigningVault = (vocesVault);
			domBuilder.OioSamlAssertion = (ParseOioSamlAssertion());
			domBuilder.ItSystemName = ("EMS");
			domBuilder.UserAuthorizationCode = ("2345C");
			domBuilder.UserEducationCode = ("7170");
			domBuilder.UserGivenName = ("Fritz");
			domBuilder.UserSurName = ("Müller");
			var requestDoc = domBuilder.Build();

			var assertionToIdCardRequest = factory.CreateOioSamlAssertionToIdCardRequestModelBuilder().Build(requestDoc);
			Assert.AreEqual("EMS", assertionToIdCardRequest.ItSystemName);
			Assert.AreEqual("2345C", assertionToIdCardRequest.UserAuthorizationCode);
			Assert.AreEqual("7170", assertionToIdCardRequest.UserEducationCode);
			Assert.AreEqual("Fritz", assertionToIdCardRequest.UserGivenName);
			Assert.AreEqual("Müller", assertionToIdCardRequest.UserSurName);
			Assert.AreEqual("http://sosi.dk", assertionToIdCardRequest.AppliesTo);
			Assert.AreEqual("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", assertionToIdCardRequest.Action);
			assertionToIdCardRequest.ValidateSignature();
			assertionToIdCardRequest.ValidateSignatureAndTrust(vocesVault);
			try
			{
				assertionToIdCardRequest.ValidateSignatureAndTrust(CredentialVaultTestUtil.GetOCES2CredentialVault());
			}
			catch (ModelException e)
			{
				Assert.AreEqual("The certificate that signed the security token is not trusted!", e.Message);
			}
			Assert.AreEqual(vocesVault.GetSystemCredentials(), assertionToIdCardRequest.GetSigningCertificate());

			var assertion = assertionToIdCardRequest.OioSamlAssertion;
			Assert.AreEqual("25520041", assertion.CvrNumberIdentifier);
			Assert.AreEqual("_5a49e560-5312-4237-8f32-2ed2b58cfcf7", assertion.Id);
			//assertion.ValidateSignatureAndTrust(SOSITestUtils.getOldIdPTrustVault());
		}


		//[Test]
		//public void IllustrateIDCardIssuing()
		//{

		//	var domBuilder = factory.CreateOiosamlAssertionToIdCardRequestDomBuilder();
		//	domBuilder.SigningVault = (vocesVault);
		//	domBuilder.OioSamlAssertion = (ParseOioSamlAssertion());
		//	domBuilder.ItSystemName = ("Harmoni/EMS");
		//	var requestDoc = domBuilder.Build();

		//	var assertionToIDCardRequest = new OioSamlAssertionToIdCardRequest(requestDoc);

		//	// And STS should:
		//	// Check signature on request and that the signing certificate is issued by the correct OCES CA
		//	// Check signature and trust on assertion, that is check signing certificate is the IdPs
		//	// Check validity in time
		//	// Check assurance level
		//	// etc.

		//	UserInfo userInfo = BuildUserInfo(assertionToIDCardRequest);
		//	var assertion = assertionToIDCardRequest.OioSamlAssertion;

		//	SubjectIdentifierType careProviderIdEnum;
		//	Enum.TryParse(SubjectIdentifierTypeValues.CvrNumber, true, out careProviderIdEnum);
		//	var careProvider = new CareProvider(careProviderIdEnum, assertion.GetAttributeValue("dk:gov:saml:attribute:CvrNumberIdentifier"), assertion.GetAttributeValue("urn:oid:2.5.4.10"));
		//	//CareProvider careProvider = new CareProvider(SubjectIdentifierTypeValues.CVR_NUMBER, assertion.getCvrNumberIdentifier(), assertion.getOrganizationName());
		//	var systemInfo = new SystemInfo(careProvider, assertionToIDCardRequest.ItSystemName);

		//	var certHash = SignatureUtil.getDigestOfCertificate(assertion.getUserCertificate());

		//	var idCard = new UserIdCard(DgwsConstants.VERSION_1_0_1, AuthenticationLevel.MocesTrustedUser, "SOSI-STS", systemInfo, userInfo, certHash, null, null, null);

		//	SOSIFactory sosiFactory = new SOSIFactory(new CredentialVaultSignatureProvider(vocesVault));

		//	//The validity gets set to 24 hours - maybe the idcard should have a shorter lifetime?
		//	IdCard signedIdCard = sosiFactory.copyToVOCESSignedIDCard(idCard);
		//	Element idCardDocElement = signedIdCard.serialize2DOMDocument(sosiFactory, XmlUtil.createEmptyDocument());
		//	//System.out.println(XmlUtil.node2String(idCardDocElement, true, true));
		//}

		[Test, Parallelizable(ParallelScope.None)]
		public void ValidateNemLoginAssertion()
		{
			//InputSource inputSource = new InputSource(this.getClass().getResourceAsStream("/oiosaml-examples/NemLog-in_assertion_valid_signature.xml"));
			//Document document = XmlUtil.readXml(System.getProperties(), inputSource, false);
			var assertionXElement = XElement.Load(NUnit.Framework.TestContext.CurrentContext.TestDirectory +
			                                      "/Resources/oiosaml-examples/NemLog-in_assertion_valid_signature.xml");
			var assertion = new OioSamlAssertion(assertionXElement);
			assertion.ValidateSignatureAndTrust(CredentialVaultTestUtil.GetNewIdpTestCredentialVault());
			Assert.AreEqual("3", assertion.AssuranceLevel);
			Assert.AreEqual("25450442", assertion.CvrNumberIdentifier);
			Assert.AreEqual("27304742", assertion.RidNumberIdentifier);
		}

	//	private Federation getMockFederation()
	//	{
	//		return new SOSITestFederation(System.getProperties()) {
	//			@Override

	//			public boolean isValidSTSCertificate(X509Certificate certificate)
	//		{
	//			return vocesVault.getSystemCredentialPair().getCertificate().equals(certificate);
	//		}
	//	};
	//}

	//private UserInfo BuildUserInfo(OioSamlAssertionToIdCardRequest request)
	//	{
	//		var assertion = request.OioSamlAssertion;
	//		string cpr = "XXXXXXXX"; // Perform lookup based on assertion.getCvrNumberIdentifier() and assertion.getRidNumberIdentifier()
	//		string givenName;
	//		string surName;
	//		if (request.UserGivenName != null && request.UserSurName != null)
	//		{
	//			givenName = request.UserGivenName;
	//			surName = request.UserSurName;
	//		}
	//		else
	//		{
	//			// The IdP cannot split CommonName and neither should we (assertion.getSurName() returns null)
	//			givenName =
	//				assertion.CommonName;
	//			surName = "-";
	//		}
	//		//var email = assertion.GetAttributeValue("urn:oid:0.9.2342.19200300.100.1.3");
	//		var email = assertion.Email;
	//		string occupation = null;
	//		var role = "YYYYY"; // Lookup based on CPR, use request.getUserEducationCode() to pick the right one (or validate)
	//		var authorizationCode = "ZZZZZ";// Lookup based on CPR, use request.getUserAuthorizationCode() to pick the right one (or validate)
	//		return new UserInfo(cpr, givenName, surName, email, occupation, role, authorizationCode);
	//	}

		private UserIdCard CreateIdCard()
		{
			SOSIFactory sosiFactory = new SOSIFactory(null, new CredentialVaultSignatureProvider(mocesVault));
			CareProvider careProvider = new CareProvider(SubjectIdentifierType.medcomcvrnumber, "30808460", "Lægehuset på bakken");
			UserInfo userInfo = new UserInfo("1111111118", "Hans", "Dampf", "", "", "7170", "341KY");
			String alternativeIdentifier = new CertificateInfo(mocesVault.GetSystemCredentials()).ToString();
			var userIdCard = sosiFactory.CreateNewUserIdCard("IT-System", userInfo, careProvider, AuthenticationLevel.MocesTrustedUser, null, null, null, alternativeIdentifier);
			userIdCard.Sign<Assertion>(sosiFactory.SignatureProvider);
			return userIdCard;
		}


		[Test, Parallelizable(ParallelScope.None)]
		public void TestIdCardToOioSamlRequest()
		{
			var domBuilder = factory.CreateIdCardToOioSamlAssertionRequestDomBuilder();
			domBuilder.SigningVault = (vocesVault);
			domBuilder.Audience = ("Sundhed.dk");
			var idCard = CreateIdCard();
			domBuilder.IdCard = (idCard);
			var requestDoc = domBuilder.Build();

			var assertionRequest = factory.CreateIdCardToOioSamlAssertionRequestModelBuilder().Build(requestDoc);
			Assert.AreEqual("Sundhed.dk", assertionRequest.AppliesTo);
			Assert.AreEqual("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", assertionRequest.Action);
			assertionRequest.ValidateSignature();
			assertionRequest.ValidateSignatureAndTrust(vocesVault);
			try
			{
				assertionRequest.ValidateSignatureAndTrust(CredentialVaultTestUtil.GetOCES2CredentialVault());
			}
			catch (ModelException e)
			{
				Assert.AreEqual("The certificate that signed the security token is not trusted!", e.Message);
			}
			Assert.AreEqual(vocesVault.GetSystemCredentials(), assertionRequest.GetSigningCertificate());

			Assert.IsTrue(idCard.Equals(assertionRequest.UserIdCard));
			assertionRequest.UserIdCard.ValidateSignature();
			assertionRequest.UserIdCard.ValidateSignatureAndTrust(mocesVault);
			try
			{
				assertionRequest.UserIdCard.ValidateSignatureAndTrust(new SosiFederation(new CrlCertificateStatusChecker()));
			}
			catch (ModelException e)
			{
				Assert.AreEqual("The certificate that signed the security token is not trusted!", e.Message);
			}

		}

		[Test, Parallelizable(ParallelScope.None)]
		public void TestIdCardToOioSamlRequestIdCardInHeader()
		{
			var domBuilder = factory.CreateIdCardToOioSamlAssertionRequestDomBuilder();
			domBuilder.PlaceIdCardInSoapHeader = true;
			domBuilder.Audience = ("Sundhed.dk");
			var idCard = CreateIdCard();
			domBuilder.IdCard = (idCard);
			var requestDoc = domBuilder.Build();

			//System.out.println(XmlUtil.node2String(requestDoc, true, true));

			var assertionRequest = factory.CreateIdCardToOioSamlAssertionRequestModelBuilder().Build(requestDoc);
			Assert.AreEqual("Sundhed.dk", assertionRequest.AppliesTo);
			Assert.AreEqual("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", assertionRequest.Action);
			try
			{
				assertionRequest.ValidateSignature();
			}
			catch (ModelBuildException e)
			{
				Assert.AreEqual("Could not find Liberty signature element", e.Message);
			}
			Assert.Null(assertionRequest.GetSigningCertificate());

			Assert.AreEqual(idCard, assertionRequest.UserIdCard);
			assertionRequest.UserIdCard.ValidateSignature();
			assertionRequest.UserIdCard.ValidateSignatureAndTrust(mocesVault);
			//try
			//{
			//	assertionRequest.UserIdCard.ValidateSignatureAndTrust(new SOSIFederation(System.getProperties()));
			//}
			//catch (ModelException e)
			//{
			//	Assert.AreEqual("The certificate that signed the security token is not trusted!", e.getMessage());
			//}
		}

		[Test, Parallelizable(ParallelScope.None)]
		public void TestIdCardToOioSamlRequestMissingIdCard()
		{
			var domBuilder = factory.CreateIdCardToOioSamlAssertionRequestDomBuilder();
			domBuilder.PlaceIdCardInSoapHeader = true;
			domBuilder.Audience = ("Sundhed.dk");
			var idCard = CreateIdCard();
			domBuilder.IdCard = (idCard);
			var requestDoc = domBuilder.Build();

			var idcardAssertion = requestDoc.Descendants(SamlTags.Assertion.Ns + SamlTags.Assertion.TagName).FirstOrDefault();//.getElementsByTagNameNS(NameSpaces.SAML2ASSERTION_SCHEMA, SAMLTags.ASSERTION).item(0);
			idcardAssertion.Remove();//.removeChild(idcardAssertion);

			var assertionRequest = factory.CreateIdCardToOioSamlAssertionRequestModelBuilder().Build(requestDoc);
			Assert.AreEqual("Sundhed.dk", assertionRequest.AppliesTo);
			Assert.AreEqual("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", assertionRequest.Action);
			try
			{
				assertionRequest.ValidateSignature();
			}
			catch (ModelBuildException e)
			{
				Assert.AreEqual("Could not find Liberty signature element", e.Message);
			}
			Assert.Null(assertionRequest.GetSigningCertificate());

			try
			{
				Assert.AreEqual(idCard, assertionRequest.UserIdCard);
			}
			catch (ModelException e)
			{
				Assert.AreEqual("Malformed request: IDCard could not be found!", e.Message);
			}
		}

		[Test, Parallelizable(ParallelScope.None)]
		public void TestOioSamlAssertionBuilder()
		{

			var idCard = CreateIdCard();
			var assertion = CreateOioSamlAssertion(idCard);

			//System.out.println(XmlUtil.node2String(assertion.getDOM(), true, false));

			AssertOioSamlAssertion(assertion, idCard);
		}

		private void AssertOioSamlAssertion(OioSamlAssertion assertion, UserIdCard idCard)
		{
			Assert.AreEqual("42634739", assertion.RidNumberIdentifier);
			Assert.AreEqual("CN=TRUST2408 Systemtest XIX CA, O=TRUST2408, C=DK", assertion.CertificateIssuer);
			Assert.IsFalse(assertion.IsYouthCertificate);
			Assert.AreEqual("5818C1A6", assertion.CertificateSerial);
			Assert.AreEqual("CVR:30808460-RID:42634739", assertion.Uid);
			Assert.IsNotNull(assertion.NotOnOrAfter);
			Assert.AreEqual("http://sundhed.dk/saml/SAMLAssertionConsumer", assertion.Recipient);
			Assert.AreEqual(idCard, assertion.UserIdCard);
			assertion.ValidateSignatureAndTrust(vocesVault);
		}

		private OioSamlAssertion CreateOioSamlAssertion(UserIdCard idCard)
		{
			var builder = factory.CreateOioSamlAssertionBuilder();
			builder.SigningVault = (vocesVault);
			builder.Issuer = ("Test STS");
			builder.UserIdCard = (idCard);
			var now = DateTimeEx.UtcNowRound;
			builder.NotBefore = (now);
			builder.NotOnOrAfter = now.AddHours(1);
			builder.AudienceRestriction = ("http://sundhed.dk");
			builder.RecipientUrl = ("http://sundhed.dk/saml/SAMLAssertionConsumer");
			builder.DeliveryNotOnOrAfter = now.AddMinutes(5);
			builder.IncludeIdCardAsBootstrapToken = true;
			return builder.Build();
		}


		//[Test]
		//public void testOIOBootstrapToIdentityTokenRequest()
		//{
		//	//OIOBootstrapToIdentityTokenRequestDOMBuilder domBuilder = factory.createOIOBootstrapToIdentityTokenRequestDOMBuilder();
		//	var domBuilder = factory.createOIOBootstrapToIdentityTokenRequestDOMBuilder();
		//	domBuilder.setSigningVault(vocesVault);
		//	domBuilder.setCPRNumberClaim("2512484916");
		//	domBuilder.setAudience("https://fmk");

		//	InputSource inputSource = new InputSource(this.getClass().getResourceAsStream("/oiosaml-examples/OIOBootstrapToIdentityToken/NemLog-In_bootstrap.xml"));
		//	Document document = XmlUtil.readXml(System.getProperties(), inputSource, false);
		//	OIOBootstrapToken assertion = new OIOBootstrapToken(document.getDocumentElement());

		//	try
		//	{
		//		assertion.validateSignatureAndTrust(SOSITestUtils.getNewIdPTrustVault());
		//	}
		//	catch (ModelException e)
		//	{
		//		Assert.AreEqual("Signature on OIOSAMLAssertion is invalid", e.Message);
		//	}

		//	inputSource = new InputSource(this.getClass().getResourceAsStream("/oiosaml-examples/OIOBootstrapToIdentityToken/NemLog-In_bootstrap_valid_signature.xml"));
		//	document = XmlUtil.readXml(System.getProperties(), inputSource, false);
		//	assertion = new OIOBootstrapToken(document.getDocumentElement());

		//	try
		//	{
		//		assertion.validateSignatureAndTrust(CredentialVaultTestUtil.GetOldIdpTestCredentialVault());
		//	}
		//	catch (ModelException e)
		//	{
		//		Assert.AreEqual("The certificate that signed the security token is not trusted!", e.getMessage());
		//	}

		//	assertion.validateSignatureAndTrust(SOSITestUtils.getNewIdPTrustVault());
		//	domBuilder.setOIOBootstrapToken(assertion);

		//	var requestDoc = domBuilder.build();

		//	//System.out.println(XmlUtil.node2String(requestDoc, true, true));

		//	// write to string and parse to XML
		//	requestDoc = XmlUtil.readXml(System.getProperties(), XmlUtil.node2String(requestDoc), false);

		//	OIOBootstrapToIdentityTokenRequest assertionRequest = factory.createOIOBootstrapToIdentityTokenRequestModelBuilder().build(requestDoc);
		//	Assert.AreEqual("https://fmk", assertionRequest.getAppliesTo());
		//	Assert.AreEqual("http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", assertionRequest.getAction());
		//	Assert.AreEqual("2512484916", assertionRequest.getCPRNumberClaim());
		//	OIOBootstrapToken bootstrapToken = assertionRequest.getOIOBootstrapToken();
		//	Assert.AreEqual(assertion.getAudienceRestriction(), bootstrapToken.getAudienceRestriction());
		//	Assert.AreEqual(assertion.getSubjectNameID(), bootstrapToken.getSubjectNameID());

		//	assertionRequest.validateSignature();
		//	assertionRequest.validateSignatureAndTrust(vocesVault);
		//	try
		//	{
		//		assertionRequest.validateSignatureAndTrust(CredentialVaultTestUtil.GetOCES2CredentialVault());
		//	}
		//	catch (ModelException e)
		//	{
		//		Assert.AreEqual("The certificate that signed the security token is not trusted!", e.getMessage());
		//	}
		//	Assert.AreEqual(vocesVault.GetSystemCredentials(), assertionRequest.getSigningCertificate());
		//}

		//[Test]
		//public void testOIOBootstrapToUnencryptedIdentityTokenResponse()
		//{
		//	assertOIOBootstrapToIdentityTokenResponse(false);
		//}

		//[Test]
		//public void testOIOBootstrapToEncryptedIdentityTokenResponse()
		//{
		//	assertOIOBootstrapToIdentityTokenResponse(true);
		//}

		

		//[Test]
		//public void testCitizenIdentityTokenBuilder()
		//{
		//	IdentityToken identityToken = createIdentityToken();
		//	assertEquals("https://fmk", identityToken.getAudienceRestriction());
		//	assertEquals(null, identityToken.getITSystemName());
		//	assertEquals("2512484916", identityToken.getCpr());

		//	//System.out.println(XmlUtil.node2String(identityToken.getDOM(), true, true));
		//}

		//private IdentityToken createIdentityToken()
		//{
		//	CitizenIdentityTokenBuilder tokenBuilder = factory.createCitizenIdentityTokenBuilder();
		//	tokenBuilder.setIssuer("http://sosi");
		//	tokenBuilder.setAudienceRestriction("https://fmk");
		//	Date now = new Date();
		//	tokenBuilder.setNotBefore(new Date(now.getTime() - 1000));
		//	tokenBuilder.setNotOnOrAfter(new Date(now.getTime() + 5 * 60 * 1000));
		//	tokenBuilder.setCprNumberAttribute("2512484916");
		//	tokenBuilder.setSubjectNameID("C=DK,O=Ingen organisatorisk tilknytning,CN=Lars Larsen,Serial=PID:9208-2002-2-514358910503");
		//	tokenBuilder.setSubjectNameIDFormat(SAMLValues.NAMEID_FORMAT_X509_SUBJECT_NAME);
		//	tokenBuilder.setDeliveryNotOnOrAfter(new Date(now.getTime() + 10 * 1000));
		//	tokenBuilder.setRecipientURL("https://fmk");
		//	tokenBuilder.setSigningVault(vocesVault);
		//	// Just to have another cert
		//	tokenBuilder.setHolderOfKeyCertificate(CredentialVaultTestUtil.getOCES2CredentialVault().getSystemCredentialPair().getCertificate());

		//	return tokenBuilder.build();
		//}

		//[Test]
		//public void testEncryptedOIOSAMLAssertionToIdentityTokenRequest()
		//{
		//	EncryptedOIOSAMLAssertionToIdentityTokenRequestDOMBuilder domBuilder = factory.createEncryptedOIOSAMLAssertionToIdentityTokenRequestDOMBuilder();
		//	domBuilder.setAudience("https://sosi");
		//	OIOSAMLAssertion assertion = createOIOSAMLAssertion(createIDCard());
		//	Element encryptedAssertionElm = createEncryptedAssertion(assertion);
		//	domBuilder.setEncryptedOIOSAMLAssertionElement(encryptedAssertionElm);
		//	domBuilder.setSigningVault(vocesVault);
		//	domBuilder.setCPRNumberClaim("1111111118");

		//	Document requestDoc = domBuilder.build();

		//	//System.out.println(XmlUtil.node2String(requestDoc, true, true));

		//	EncryptedOIOSAMLAssertionToIdentityTokenRequest tokenRequest = factory.createEncryptedOIOSAMLAssertionToIdentityTokenRequestModelBuilder().build(requestDoc);
		//	assertEquals("1111111118", tokenRequest.getCPRNumberClaim());
		//	assertEquals("https://sosi", tokenRequest.getAppliesTo());
		//	assertEquals(vocesVault.getSystemCredentialPair().getCertificate(), tokenRequest.getSigningCertificate());
		//	Element decryptedAssertionElement = EncryptionUtil.decryptAndDetach(tokenRequest.getEncryptedOIOSAMLAssertionElement(), vocesVault.getSystemCredentialPair().getPrivateKey());
		//	OIOSAMLAssertion decryptedAssertion = new OIOSAMLAssertion(decryptedAssertionElement);
		//	assertEquals(assertion.getSubjectNameID(), decryptedAssertion.getSubjectNameID());

		//	tokenRequest.validateSignature();
		//	tokenRequest.validateSignatureAndTrust(vocesVault);
		//	try
		//	{
		//		tokenRequest.validateSignatureAndTrust(mocesVault);
		//		fail();
		//	}
		//	catch (ModelException e)
		//	{
		//		assertEquals("The certificate that signed the security token is not trusted!", e.getMessage());
		//	}

		//}

		//[Test]
		//public void testEncryptedOIOSAMLAssertionToUnencryptedIdentityTokenResponse()
		//{
		//	assertEncryptedOIOSAMLAssertionToIdentityTokenResponse(false);
		//}

		//[Test]
		//public void testEncryptedOIOSAMLAssertionToEncryptedIdentityTokenResponse()
		//{
		//	assertEncryptedOIOSAMLAssertionToIdentityTokenResponse(true);
		//}

		//private void assertEncryptedOIOSAMLAssertionToIdentityTokenResponse(boolean encrypt)
		//{
		//	EncryptedOIOSAMLAssertionToIdentityTokenResponseDOMBuilder domBuilder = factory.createEncryptedOIOSAMLAssertionToIdentityTokenResponseDOMBuilder();
		//	domBuilder.setSigningVault(vocesVault);
		//	domBuilder.setRelatesTo("1234");
		//	domBuilder.setContext("2345");
		//	domBuilder.setIdentityToken(createIdentityToken());
		//	if (encrypt)
		//	{
		//		domBuilder.setEncryptionKey(vocesVault.getSystemCredentialPair().getCertificate().getPublicKey());
		//	}
		//	Document responseDoc = domBuilder.build();

		//	//System.out.println(XmlUtil.node2String(responseDoc, true, true));

		//	EncryptedOIOSAMLAssertionToIdentityTokenResponse tokenResponse = factory.createEncryptedOIOSAMLAssertionToIdentityTokenResponseModelBuilder().build(responseDoc);
		//	assertEquals("https://fmk", tokenResponse.getAppliesTo());
		//	assertEquals(vocesVault.getSystemCredentialPair().getCertificate(), tokenResponse.getSigningCertificate());

		//	IdentityToken identityToken = extractIdentityToken(encrypt, tokenResponse);
		//	assertEquals("2512484916", identityToken.getCpr());
		//	assertEquals(null, identityToken.getUserEducationCode());
		//	assertEquals("3", identityToken.getAssuranceLevel());
		//	assertEquals("DK-SAML-2.0", identityToken.getSpecVersion());


		//	tokenResponse.validateSignature();
		//	tokenResponse.validateSignatureAndTrust(getMockFederation());
		//	try
		//	{
		//		tokenResponse.validateSignatureAndTrust(new SOSITestFederation(System.getProperties()));
		//		fail();
		//	}
		//	catch (ModelException e)
		//	{
		//		assertEquals("The certificate that signed the security token is not trusted!", e.getMessage());
		//	}
		//}

		//private IdentityToken extractIdentityToken(boolean encrypted, AbstractIdentityTokenResponse tokenResponse)
		//{
		//	if (encrypted)
		//	{
		//		assertNull(tokenResponse.getIdentityToken());
		//		Element encryptedIdentityTokenElement = tokenResponse.getEncryptedIdentityTokenElement();
		//		return new IdentityToken(EncryptionUtil.decryptAndDetach(encryptedIdentityTokenElement, vocesVault.getSystemCredentialPair().getPrivateKey()));
		//	}
		//	else
		//	{
		//		assertNull(tokenResponse.getEncryptedIdentityTokenElement());
		//		return tokenResponse.getIdentityToken();
		//	}
		//}


		//private Element createEncryptedAssertion(OIOSAMLAssertion assertion)
		//{
		//	String simpleXml = "<dummy-root/>";
		//	Document tempDoc = XmlUtil.readXml(System.getProperties(), simpleXml, false);
		//	Element dummyRoot = tempDoc.getDocumentElement();

		//	Element encryptedAssertionElm = tempDoc.createElementNS(NameSpaces.SAML2ASSERTION_SCHEMA, SAMLTags.ENCRYPTED_ASSERTION_PREFIXED);
		//	dummyRoot.appendChild(encryptedAssertionElm);
		//	Element assertionElm = (Element)tempDoc.importNode(assertion.getDOM().getDocumentElement(), true);
		//	encryptedAssertionElm.appendChild(assertionElm);

		//	EncryptionUtil.encrypt(assertionElm, vocesVault.getSystemCredentialPair().getCertificate().getPublicKey());

		//	//System.out.println(XmlUtil.node2String(tempDoc, true, true));

		//	return (Element)dummyRoot.getFirstChild();
		//}
	}
}
