using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Xsl;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace XSLTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string TAG_CHOOSE_XSLT = "CHOOSE_XSLT";
		private const string TAG_CHOOSE_XML = "CHOOSE_XML";
		private const string TAG_EXIT = "EXIT";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ChooseDataFile()
		{
			ChooseFileFor(Data, "Choose a data file...");

		}

		private void ChooseSheetFile()
		{
			ChooseFileFor(Sheet, "Choose an xslt style sheet...");
		}

		private void SetOutputHighlighting(string language)
		{
			var definition = HighlightingManager.Instance.GetDefinition(language);
			if (definition != null && Output is TextEditor)
				Output.SyntaxHighlighting = definition;
			else if (StatusMessage is Label)
				StatusMessage.Content = $"Unknown highlighting type: {language}";
		}

		private string GetResourceString(string name)
		{
			using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream($"XSLTool.Resources.{name}"))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		void RegisterSyntaxHighlighters()
		{
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("XSLTool.Resources.Highlighting.sql.xshd"))
			{
				using (var reader = new System.Xml.XmlTextReader(stream))
				{
					var highlighting =
						 ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);

					HighlightingManager.Instance.RegisterHighlighting("SQL", null, highlighting);
				}
			}
		}

		private void ActByTag(string tag)
		{
			switch (tag)
			{
				case TAG_CHOOSE_XML: ChooseDataFile(); break;
				case TAG_CHOOSE_XSLT: ChooseSheetFile(); break;
				case TAG_EXIT: Environment.Exit(0); break;
			}
		}

		private void ChooseFileFor(TextEditor editor, string title = "Choose a file...")
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Title = title;

			dialog.FileOk += (sender, arg) => {
				editor.Text = System.IO.File.ReadAllText(dialog.FileName);
			};

			NeedsTransformation = true;
			dialog.ShowDialog(this);
		}


		private void InnerExecuteTransformation()
		{
			XslCompiledTransform xslt;

			using (StringReader srt = new StringReader(Sheet.Text))
			{
				using (StringReader sri = new StringReader(Data.Text))
				{
					using (XmlReader xrt = XmlReader.Create(srt))
					{
						using (XmlReader xri = XmlReader.Create(sri))
						{
							try
							{
								xslt = new XslCompiledTransform();
								xslt.Load(xrt);
							}
							catch (Exception ex)
							{
								throw new TransformationException($"Invalid XSLT: {ex.Message}", ex);
							}

							using (StringWriter sw = new StringWriter())
							{
								using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
								{
									try
									{
										xslt.Transform(xri, xwo);
										Output.Text = sw.ToString();
									}
									catch (Exception ex)
									{
										throw new TransformationException($"Transform failed: {ex.Message}", ex);
									}
								}
							}
						}
					}
				}
			}
			StatusMessage.Content = "";
			NeedsTransformation = false;
		}
		private void ExecuteTransformation()
		{
			try
			{
				InnerExecuteTransformation();
			}
			catch (TransformationException ex)
			{
				StatusMessage.Content = ex.Message;
			}
		}

		const int AutoTransformDelayMsecs = 250;

		private bool NeedsTransformation { get; set; } = false;
		private DateTime LastRecordedKeypress = DateTime.Now - TimeSpan.FromMilliseconds(AutoTransformDelayMsecs);

		void RecordMutatingKeyPress()
		{
			LastRecordedKeypress = DateTime.Now;
			NeedsTransformation = true;
		}

		private DispatcherTimer AutoTransformTimer;

		private void SetupAutoTransformTimer()
		{
			TimeSpan delay = TimeSpan.FromMilliseconds(AutoTransformDelayMsecs);

			AutoTransformTimer = new DispatcherTimer();
			AutoTransformTimer.Tick += (sender, e) => {
				// if the setting to auto transform is enabled and time distance between last recorded keypress and now > auto transform delay...		

				if (AutoTransformEnabled.IsChecked == true)
					if (NeedsTransformation && DateTime.Now > LastRecordedKeypress + delay)
						ExecuteTransformation();
			};

			AutoTransformTimer.Interval = TimeSpan.FromMilliseconds(AutoTransformDelayMsecs);
			AutoTransformTimer.Start();
		}
		#region Event handling
		private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ActByTag((string)((TabItem)sender).Tag);
		}

		private void OutputType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string languageName = ((ComboBoxItem)(e.AddedItems[0])).Tag as string;
			SetOutputHighlighting(languageName);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			RegisterSyntaxHighlighters();
			Sheet.Text = GetResourceString("Initial.xslt");
			Data.Text = "<Root />";
			SetupAutoTransformTimer();

			SetOutputHighlighting("XML");
		}

		private void TransformNow_Click(object sender, RoutedEventArgs e)
		{
			ExecuteTransformation();
		}

		private void Editor_KeyDown(object sender, KeyEventArgs e)
		{
			RecordMutatingKeyPress();
		}

		private void ChooseXmlFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ChooseDataFile();
		}
		private void ChooseXsltFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ChooseSheetFile();
		}
		private void Transform_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ExecuteTransformation();
		}
		#endregion Event handling
	}
}
