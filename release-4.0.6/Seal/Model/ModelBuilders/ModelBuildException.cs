using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Model.ModelBuilders
{
	public class ModelBuildException : Exception
	{
		public ModelBuildException(string message) : base(message)
		{
		}

		public ModelBuildException(string message, Exception e) : base(message, e)
		{
		}
	}
}
