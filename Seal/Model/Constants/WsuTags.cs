using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dk.nsi.seal.Model.Constants
{
	public class WsuTags : ITag
	{

		public static WsuTags Timestamp => new WsuTags("Timestamp");
		public static WsuTags Created => new WsuTags("Created");
		public static WsuTags Expires => new WsuTags("Expires");

		protected WsuTags(string tag)
		{
			TagName = tag;
		}
		public XNamespace Ns => NameSpaces.xwsu;
		public string TagName { get; private set; }

	}
}
