using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XSLTool
{
	public class Commands
	{
		public static readonly RoutedUICommand ChooseXmlFile
			= new RoutedUICommand("Open XML Data File",
				nameof(ChooseXmlFile),
				typeof(Commands));

		public static readonly RoutedUICommand ChooseXsltFile 
			= new RoutedUICommand("Open XSLT Stylesheet File",
				nameof(ChooseXsltFile),
				typeof(Commands));

		public static readonly RoutedUICommand Transform
			= new RoutedUICommand("Open XSLT Stylesheet File",
				nameof(Transform),
				typeof(Commands));

	}
}
