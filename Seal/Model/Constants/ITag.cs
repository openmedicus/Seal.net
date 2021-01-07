using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dk.nsi.seal.Model.Constants
{
	public interface ITag
	{
		XNamespace Ns { get; }
		string TagName { get; }
		//string Prefix { get; }

	}
}
