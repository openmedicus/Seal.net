using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dk.nsi.seal.Model.Constants
{
	public class WsfAuthTags : ITag
	{

		public static WsfAuthTags ClaimType => new WsfAuthTags("ClaimType");
		public static WsfAuthTags Value => new WsfAuthTags("Value");

		protected WsfAuthTags(string tag)
		{
			TagName = tag;
		}
		public XNamespace Ns => NameSpaces.xwsfAuth;
		public string TagName { get; private set; }
	}
}
