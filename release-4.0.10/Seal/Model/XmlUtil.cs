using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Model.Constants;

namespace dk.nsi.seal.Model
{
	public static class XmlUtil
	{
		public static XElement CreateElement(ITag tag)
		{
			return new XElement(tag.Ns + tag.TagName);
			//localDoc .createElementNS(tag.getNS(), tag.getPrefix() + ":" + tag.getTagName());
		}
	}
}
