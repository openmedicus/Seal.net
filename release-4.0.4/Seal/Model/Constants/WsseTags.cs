using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dk.nsi.seal.Model.Constants
{
	public class WsseTags : ITag
	{

		public static WsseTags Security => new WsseTags("Security");
		public static WsseTags SecurityTokenReference => new WsseTags("SecurityTokenReference");

		protected WsseTags(string tag)
		{
			TagName = tag;
		}
		public XNamespace Ns => NameSpaces.xwsse;
		public string TagName { get; private set; }

	}
}
