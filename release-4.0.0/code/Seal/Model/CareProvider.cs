using System;
using dk.nsi.seal.dgwstypes;

namespace dk.nsi.seal
{
	public class CareProvider
	{

		public string Id { get; }
		public string OrgName { get; }
		public SubjectIdentifierType Type { get; }

		public CareProvider(SubjectIdentifierType type, string id, string orgName)
		{
			Id = id;
			OrgName = orgName;
			Type = type;

		}

		public override bool Equals(object obj)
		{
			if (!(obj is CareProvider)) return false;

			var cp = (CareProvider)obj;
			return Id == cp.Id
				& OrgName == cp.OrgName
				& Type == cp.Type;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode()
					^ OrgName.GetHashCode()
					^ Type.GetHashCode();
		}

	}
}
