using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
	public class SosiFederation : Federation
	{
		private const string NewFocesSTSSubjectNamePrefix = "SOSI Federation";

	    private readonly SosiStsCertificateMatcher matcher;

	    public SosiFederation(ICertificateStatusChecker certificateStatusChecker) : base(new OcesCertificationAuthority(certificateStatusChecker))
	    {
            matcher = new SosiStsCertificateMatcher(NewFocesSTSSubjectNamePrefix);
	    }

	    protected override bool SubjectDistinguishedNameMatches(DistinguishedName subjectDistinguishedName)
	    {
	        return matcher.Matches(subjectDistinguishedName);
	    }
	}
}
