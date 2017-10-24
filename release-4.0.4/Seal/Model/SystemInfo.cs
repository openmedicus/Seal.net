using System;
using System.Security.Cryptography.Pkcs;
namespace dk.nsi.seal
{
	public class SystemInfo
	{
		public CareProvider CareProvider { get; }
		public string ItSystemName { get; }

	    public SystemInfo(CareProvider careProvider, string itSystemName)
		{
			CareProvider = careProvider;
			ItSystemName = itSystemName;
		}


		public override bool Equals(object obj)
		{
			if (!(obj is SystemInfo)) return false;

			var si = (SystemInfo)obj;
			var result = CareProvider.Id.Equals(si.CareProvider.Id)
			              & CareProvider.OrgName.Equals(si.CareProvider.OrgName)
			              & CareProvider.Type.Equals(si.CareProvider.Type)
			              & ItSystemName.Equals(si.ItSystemName);
			return result;
		}

		public override int GetHashCode()
		{
			return CareProvider.GetHashCode()
					^ ItSystemName.GetHashCode();
		}

	}
}
