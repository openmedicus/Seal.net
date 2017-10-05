using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
	public class SosiStsCertificateMatcher
	{
		private const string NewFocesSTSSubjectSerialnumberPrefix = "CVR:33257872-FID:";

		private readonly string _newFocesStsCertPrefix;


		public SosiStsCertificateMatcher(string newFocesSTSCertPrefix)
		{
			_newFocesStsCertPrefix = newFocesSTSCertPrefix;
		}

		public bool Matches(DistinguishedName subjectDN)
		{
			return subjectDN.CommonName != null && subjectDN.CommonName.StartsWith(_newFocesStsCertPrefix)
				&& subjectDN.SubjectSerialNumber != null && subjectDN.SubjectSerialNumber.StartsWith(NewFocesSTSSubjectSerialnumberPrefix);
		}
	}
}
