﻿using System;
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
		public static readonly RoutedUICommand SaveXmlFile
			= new RoutedUICommand("Save the current XML Data File",
				nameof(SaveXmlFile),
				typeof(Commands));

		public static readonly RoutedUICommand ChooseXsltFile
			= new RoutedUICommand("Open XSLT Stylesheet File",
				nameof(ChooseXsltFile),
				typeof(Commands));
		public static readonly RoutedUICommand SaveXsltFile
			= new RoutedUICommand("Save the current XSLT Stylesheet File",
				nameof(SaveXsltFile),
				typeof(Commands));

		public static readonly RoutedUICommand Transform
			= new RoutedUICommand("Open XSLT Stylesheet File",
				nameof(Transform),
				typeof(Commands));

		public static readonly RoutedUICommand SaveOutputToFile
			= new RoutedUICommand("Save Generated output to file",
				nameof(SaveOutputToFile),
				typeof(Commands));

		public static readonly RoutedUICommand ExportOutputToPDFFile
			= new RoutedUICommand("Export Generated output to a PDF file",
				nameof(ExportOutputToPDFFile),
				typeof(Commands));

		public static readonly RoutedUICommand ShowAboutBox
			= new RoutedUICommand("Show the about box",
				nameof(ShowAboutBox),
				typeof(Commands));
	}
}
