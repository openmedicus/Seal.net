using System;

namespace dk.nsi.seal
{
	public class UserInfo
	{
		public string AuthorizationCode { get; }
		public string Cpr { get; }
		public string Email { get; }
		public string GivenName { get; }
		public string Occupation { get; }
		public string Role { get; }
		public string SurName { get; }

		public UserInfo(string cpr, string givenName, string surName, string email, string occupation, string role, string authorizationCode)
		{
			Cpr = cpr;
			GivenName = givenName;
			SurName = surName;
			Email = email;
			Occupation = occupation;
			Role = role;
			AuthorizationCode = authorizationCode;
		}
		public UserInfo(UserInfo original, string cpr)
		{
			Cpr = cpr;
			GivenName = original.GivenName;
			SurName = original.SurName;
			Email = original.Email;
			Occupation = original.Occupation;
			Role = original.Role;
			AuthorizationCode = original.AuthorizationCode;

		}


		public override bool Equals(object obj)
		{
			if (!(obj is UserInfo)) return false;

			var ui = (UserInfo)obj;
			var result = Cpr == ui.Cpr
			              & GivenName == ui.GivenName
			              & SurName == ui.SurName
			              & Email == ui.Email
			              & Occupation == ui.Occupation
			              & Role == ui.Role
			              & AuthorizationCode == ui.AuthorizationCode;
			return result;
		}

		public override int GetHashCode()
		{
			return Cpr.GetHashCode()
					^ GivenName.GetHashCode()
					^ SurName.GetHashCode()
					^ Email.GetHashCode()
					^ Occupation.GetHashCode()
					^ Role.GetHashCode()
					^ AuthorizationCode.GetHashCode();
		}


	}
}
