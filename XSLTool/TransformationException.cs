using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSLTool
{
	public class TransformationException : Exception
	{
		public TransformationException(string message, Exception innerException=null) : base(message, innerException)
		{
		}
	}
}
