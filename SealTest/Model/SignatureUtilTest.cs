using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Model;
using dk.nsi.seal.Vault;
using System.Configuration;
using NUnit.Framework;

namespace SealTest.Model
{
	/// <summary>
	/// Tests the SignatureUtilClass
	/// Must be run from an Nets whitelisted IP or the CRL check will fail
	/// </summary>
	public class SignatureUtilTest
	{

		[Test]
		public void TestSignAndValidateWithTrustWithRevoked()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_gyldig.p12", "Test1234");
			var result = SignAndValidate(newCert, true, true);
			Assert.IsTrue(result);
		}

		[Test]
		public void TestSignAndValidateWithTrustWithoutRevoked()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_gyldig.p12", "Test1234");
			var result = SignAndValidate(newCert, true, false);
			Assert.IsTrue(result);
		}

		[Test]
		public void TestSignAndValidateWithoutTrustWithoutRevoked()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_gyldig.p12", "Test1234");
			var result = SignAndValidate(newCert, false, false);
			Assert.IsTrue(result);
		}

		[Test]
		public void TestSignAndValidateFailDate()
		{
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckDate"))
			{
				ConfigurationManager.AppSettings["CheckDate"] = "True";
			}

			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_udloebet.p12", "Test1234");
			var result = SignAndValidate(newCert, true, true);
			Assert.IsFalse(result);
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckDate"))
			{
				ConfigurationManager.AppSettings["CheckDate"] = "False";
			}
		}

		[Test]
		public void TestSignAndValidateFailRevoked()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_spaerret.p12", "Test1234");
			try
			{
				SignAndValidate(newCert, true, true);
			}
			catch (Exception e)
			{
				//Assert.IsInstanceOfType(e, typeof(ModelException));
			}
		}


		private bool SignAndValidate(X509Certificate2 cert, bool checkTrust, bool checkRevoked)
		{
			GenericCredentialVault vault = new GenericCredentialVault();

			cert.FriendlyName = vault.ALIAS_SYSTEM;
			vault.AddTrustedCertificate(cert);

			var ass = AssertionMaker.MakeAssertionForSTS(cert);

			var signedAss = SealUtilities.SignAssertion(ass, cert);
			var signedXml = Serialize(signedAss);

			return SignatureUtil.Validate(signedXml.Root, null, vault, checkTrust, checkRevoked);
		}



		[Test]
		public void TestSignAndValidateNotTrusted()
		{
			GenericCredentialVault vault = new GenericCredentialVault();

			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_gyldig.p12", "Test1234");
			var cert2 = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\VOCES_gyldig.p12", "Test1234");
			cert2.FriendlyName = vault.ALIAS_SYSTEM;
			vault.AddTrustedCertificate(cert2);

			var ass = AssertionMaker.MakeAssertionForSTS(newCert);

			var signedAss = SealUtilities.SignAssertion(ass, newCert);
			var signedXml = Serialize(signedAss);

			try
			{
				SignatureUtil.Validate(signedXml.Root, null, vault, true, true);
			}
			catch (Exception e)
			{
				//Assert.IsInstanceOfType(e, typeof(ModelException));
			}
		}

		[Test]
		public void TestSignAndValidateSelfSignedWithTrustWithCrl()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\SelfSigned.pfx", "Test1234");
			try
			{
				SignAndValidate(newCert, true, true);
				Assert.IsTrue(false, "Test did not throw exception");
			}
			catch (Exception e)
			{
				//Assert.IsInstanceOfType(e, typeof(ModelException));
			}
		}

		[Test]
		public void TestSignAndValidateSelfSignedWithTrustWithoutCrl()
		{
			//Add test certificate to vault
			X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\SelfSigned.pfx", "Test1234");
			var result = SignAndValidate(newCert, true, false);
			Assert.IsTrue(result);
		}


		private static XDocument Serialize<T>(T element)
		{
			return XDocument.Load(Serialize2Stream(element), LoadOptions.PreserveWhitespace);
		}

		private static Stream Serialize2Stream<T>(T element)
		{
			var ms = new MemoryStream();
			using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Indent = false }))
			{
				GetSerializer<T>().Serialize(xmlWriter, element);
			}
			ms.Position = 0;
			return ms;
		}

		private static XmlSerializer GetSerializer<T>()
		{
			var t = typeof(T);
			var rootns = t.GetCustomAttributes(false).OfType<XmlTypeAttribute>().FirstOrDefault().Namespace;

			return new XmlSerializer(t , rootns );
		}



	}
}
