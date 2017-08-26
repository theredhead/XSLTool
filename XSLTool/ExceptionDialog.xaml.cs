using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XSLTool
{
	/// <summary>
	/// Interaction logic for ExceptionDialog.xaml
	/// </summary>
	public partial class ExceptionDialog : Window
	{
		public string ProductName
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return Assembly.GetExecutingAssembly().FullName;
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}


		public Exception Exception { get; private set; }
		public ExceptionDialog()
		{
			InitializeComponent();
		}
		public void Bind(Exception ex)
		{
			Exception = ex;
			Message.Text = ex.Message;
			Details.Text = ex.StackTrace;
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(1);
		}

		private void IgnoreButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void SaveReportButton_Click(object sender, RoutedEventArgs e)
		{
			string path = System.IO.Path.GetTempFileName();
			string fileName = System.IO.Path.GetFileName(path);
			string tempFilePath = System.IO.Path.ChangeExtension(path, "txt");
			File.Move(path, tempFilePath);

			using (Stream stream = File.OpenWrite(tempFilePath))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine($"{ProductName} crashed because {Exception.Message}.");
					writer.WriteLine($"@ {DateTime.Now.ToString("yyyy-MM-dd H:mm:ss zzz")}");
					writer.WriteLine();
					WriteExceptionReportToStreamWriter(Exception, writer);
					writer.Flush();
				}

				stream.Close();
			}

			System.Diagnostics.Process.Start(tempFilePath);
		}

		private void WriteExceptionReportToStreamWriter(Exception exception, StreamWriter writer, int indent=0)
		{
			string prefix = String.Join("", Enumerable.Repeat("\t", indent));
			writer.WriteLine($"{prefix}{exception.GetType().Name}:");
			writer.WriteLine($"{prefix}{exception.Message}");
			writer.WriteLine($"{prefix}{exception.StackTrace}");

			if (exception.InnerException != null)
			{
				WriteExceptionReportToStreamWriter(exception.InnerException, writer, indent ++);
			}
		}

		internal static void Present(Exception ex)
		{
			ExceptionDialog dialog = new ExceptionDialog();
			dialog.Bind(ex);
			dialog.ShowDialog();
		}
	}
}
