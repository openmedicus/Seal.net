﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dk.nsi.seal.Model.Constants
{
	public class LibertyTags : ITag
	{
		public static LibertyTags Framework => new LibertyTags("Framework");

		protected LibertyTags(string tag)
		{
			TagName = tag;
		}
		public XNamespace Ns => NameSpaces.xLibertySbfSchema;
		public string TagName { get; private set; }
	}
}
