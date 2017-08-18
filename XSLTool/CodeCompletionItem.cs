using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace XSLTool
{
	class CodeCompletionItem : ICompletionData
	{
		public CodeCompletionItem(string text)
		{
			Text = text;
			Content = text;
			Description = $"{text}";
		}
		public ImageSource Image { get { return null; } }

		public string Text { get; private set; }

		public object Content { get; private set; }

		public object Description { get; private set; }

		public double Priority { get; set; } = 1;

		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, this.Text);
		}

		static public CodeCompletionItem Make(string text, string content, string description)
		{
			CodeCompletionItem item = new CodeCompletionItem(text);
			item.Content = content;
			item.Description = description;
			return item;
		}
	}
}
