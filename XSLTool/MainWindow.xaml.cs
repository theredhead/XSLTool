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

			dialog.FileOk += (sender, arg) =>
			{
				editor.Text = File.ReadAllText(dialog.FileName);
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
								throw new TransformationException($"Invalid XSLT: {ex.Message} {ex.InnerException?.Message}", ex);
							}

							using (StringWriter sw = new StringWriter())
							{
								using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
								{
									try
									{
										xslt.Transform(xri, xwo);
										Output.Text = sw.ToString();
										OutputChanged(Output.Text);
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

		// Gets called whenever the output is changed by a transformation.
		private void OutputChanged(string text)
		{
			if (Browser.IsVisible)
				Browser.NavigateToString(text);
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
			AutoTransformTimer.Tick += (sender, e) =>
			{
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
			// ActByTag((string)((TabItem)sender).Tag);
		}

		private void OutputType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string languageName = ((ComboBoxItem)(e.AddedItems[0])).Tag as string;
			SetOutputHighlighting(languageName);

			VisualDisplayToggle.IsEnabled = (languageName == "HTML");
			SetVisualizationEnabled(VisualDisplayToggle.IsEnabled && (VisualDisplayToggle.IsChecked ?? false));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			RegisterSyntaxHighlighters();
			Sheet.Text = GetResourceString("Initial.xslt");
			Data.Text = GetResourceString("Initial.xml");
			SetupAutoTransformTimer();

			SetOutputHighlighting("XML");
			ExecuteTransformation();
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

		private void VisualDisplayToggle_Toggled(object sender, RoutedEventArgs e)
		{
			CheckBox check = (CheckBox)sender;
			bool wantVisual = check.IsEnabled && (check.IsChecked ?? false);
			SetVisualizationEnabled(wantVisual);
		}

		private void SetVisualizationEnabled(bool wantVisual)
		{
			if (Browser != null && Output != null)
			{
				Browser.Visibility = wantVisual ? Visibility.Visible : Visibility.Hidden;
				Output.Visibility = wantVisual ? Visibility.Hidden : Visibility.Visible;

				if (wantVisual)
					OutputChanged(Output.Text);
			}
		}

		AboutBox aboutbox = new AboutBox();
		private void ShowAboutBox_Executed(object sender, RoutedEventArgs e)
		{
			aboutbox.Show();
		}
		private void SaveOutputToFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save generated output to file...";
			dialog.CheckFileExists = true;
			string ext = Output.SyntaxHighlighting.Name.ToLower();
			dialog.Filter = $"{ext}|*.{ext}|Any|*.*";

			dialog.FileOk += (o, arg) => {
				string path = dialog.FileName;
				File.WriteAllText(path, Output.Text);
			};

			dialog.ShowDialog(this);
		}
	}
}
