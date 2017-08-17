using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal
{
	public class OcesUtil
	{
		static Uri RetrieveIntermediateCertificateURI(X509Certificate2 certificate)
		{
			return new Uri("");
			//var b1_3_6_1_5_5_7_1_1 = certificate.Extensions["1.3.6.1.5.5.7.1.1"];
			//if (b1_3_6_1_5_5_7_1_1 == null)
			//{
			//	throw new Exception("Invalid certificate - Authority Information Access (1.3.6.1.5.5.7.1.1) not found.");
			//}
			//try
			//{
			//	byte[] b1_3_6_1_5_5_7_1_1 = certificate.getExtensionValue("1.3.6.1.5.5.7.1.1");
			//	if (b1_3_6_1_5_5_7_1_1 == null)
			//	{
			//		throw new Exception("Invalid certificate - Authority Information Access (1.3.6.1.5.5.7.1.1) not found.");
			//		//throw new PKIException("Invalid certificate - Authority Information Access (1.3.6.1.5.5.7.1.1) not found.");
			//	}
			//	ASN1InputStream is1_3_6_1_5_5_7_1_1 = new ASN1InputStream(b1_3_6_1_5_5_7_1_1);

			//	DEROctetString osAuthorityInformationAccess = (DEROctetString) is1_3_6_1_5_5_7_1_1.readObject();
			//	ASN1InputStream osAuthorityInformationAccessValue = new ASN1InputStream(
			//		osAuthorityInformationAccess.getOctets());

			//	ASN1Sequence seqAuthorityInformationAccessValue = (ASN1Sequence) osAuthorityInformationAccessValue.readObject();

			//	if (seqAuthorityInformationAccessValue.size() < 2)
			//	{
			//		throw new PKIException(
			//			"Invalid certificate - CA Issuers (1.3.6.1.5.5.7.48.2) not found under Authority Information Access.");
			//	}

			//	ASN1Sequence seq1_3_6_1_5_5_7_48_2 = (ASN1Sequence) seqAuthorityInformationAccessValue.getObjectAt(1);
			//	ASN1Encodable seq1_3_6_1_5_5_7_48_2Value = seq1_3_6_1_5_5_7_48_2.getObjectAt(1);

			//	DEROctetString osAlternativeName =
			//		(DEROctetString) ASN1TaggedObject.getInstance(seq1_3_6_1_5_5_7_48_2Value).getObject();

			//	return new URI(new String(osAlternativeName.getOctets()));
			//}
			//catch (IOException ex)
			//{
			//	throw new PKIException(ex);
			//}
			//catch (URISyntaxException ex)
			//{
			//	throw new PKIException(ex);
			//}
		}

		static bool IsProbableOCES1Certificate(X509Certificate2 certificate)
		{
			return certificate.IssuerName.Name != null &&
			       certificate.IssuerName.Name.IndexOf("TDC OCES", StringComparison.Ordinal) != -1;
		}

		static bool IsProbableOCES2Certificate(X509Certificate2 certificate)
		{
			return certificate.IssuerName.Name != null &&
			       certificate.IssuerName.Name.IndexOf("TRUST2408", StringComparison.Ordinal) != -1;
		}

		static bool IsProbableIntermediateOrRootCertificate(X509Certificate2 certificate)
		{
			return IsProbableOCES2Certificate(certificate) && certificate.IssuerName.Name.IndexOf("Primary") != -1;
		}

		static bool IsIssuerOf(X509Certificate certificate, X509Certificate verifyAgainst)
		{
			return true;
			//	try
			//	{
			//		certificate.verify(verifyAgainst.getPublicKey());
			//		return true; // NOPMD
			//	}
			//	catch (InvalidKeyException e)
			//	{
			//		return false; // NOPMD
			//	}
			//	catch (CertificateException e)
			//	{
			//		throw new PKIException("Failed to establish issuer of");
			//	}
			//	catch (NoSuchAlgorithmException e)
			//	{
			//		throw new PKIException("Failed to establish issuer of");
			//	}
			//	catch (NoSuchProviderException e)
			//	{
			//		throw new PKIException("Failed to establish issuer of");
			//	}
			//	catch (SignatureException e)
			//	{
			//		return false;
			//	}
			//}
		}
	}
}
