using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSLTool
{
	class XSLCompletionList : List<CodeCompletionItem>
	{
		public XSLCompletionList()
		{
			Add(CodeCompletionItem.Make("apply-imports", "apply-imports", "Applies a template rule from an imported style sheet"));
			Add(CodeCompletionItem.Make("apply-templates", "apply-templates", "Applies a template rule to the current element or to the current element's child nodes"));
			Add(CodeCompletionItem.Make("attribute", "attribute", "Adds an attribute"));
			Add(CodeCompletionItem.Make("attribute-set", "attribute-set", "Defines a named set of attributes"));
			Add(CodeCompletionItem.Make("call-template", "call-template", "Calls a named template"));
			Add(CodeCompletionItem.Make("choose", "choose", "Used in conjunction with &lt;when&gt; and &lt;otherwise&gt; to express multiple conditional tests"));
			Add(CodeCompletionItem.Make("comment", "comment", "Creates a comment node in the result tree"));
			Add(CodeCompletionItem.Make("copy", "copy", "Creates a copy of the current node (without child nodes and attributes)"));
			Add(CodeCompletionItem.Make("copy-of", "copy-of", "Creates a copy of the current node (with child nodes and attributes)"));
			Add(CodeCompletionItem.Make("decimal-format", "decimal-format", "Defines the characters and symbols to be used when converting numbers into strings, with the format - number() function"));
			Add(CodeCompletionItem.Make("element", "element", "Creates an element node in the output document"));
			Add(CodeCompletionItem.Make("fallback", "fallback", "Specifies an alternate code to run if the processor does not support an XSLT element"));
			Add(CodeCompletionItem.Make("for-each", "for-each", "Loops through each node in a specified node set"));
			Add(CodeCompletionItem.Make("if", "if", "Contains a template that will be applied only if a specified condition is true"));
			Add(CodeCompletionItem.Make("import", "import", "Imports the contents of one style sheet into another."));
			Add(CodeCompletionItem.Make("include", "include", "Includes the contents of one style sheet into another. "));
			Add(CodeCompletionItem.Make("key", "key", "Declares a named key that can be used in the style sheet with the key() function"));
			Add(CodeCompletionItem.Make("message", "message", "Writes a message to the output (used to report errors)"));
			Add(CodeCompletionItem.Make("namespace-alias", "namespace-alias", "Replaces a namespace in the style sheet to a different namespace in the output"));
			Add(CodeCompletionItem.Make("number", "number", "Determines the integer position of the current node and formats a number"));
			Add(CodeCompletionItem.Make("otherwise", "otherwise", "Specifies a default action for the &lt;choose&gt; element"));
			Add(CodeCompletionItem.Make("output", "output", "Defines the format of the output document"));
			Add(CodeCompletionItem.Make("param", "param", "Declares a local or global parameter"));
			Add(CodeCompletionItem.Make("preserve-space", "preserve-space", "Defines the elements for which white space should be preserved"));
			Add(CodeCompletionItem.Make("processing-instruction", "processing-instruction", "Writes a processing instruction to the output"));
			Add(CodeCompletionItem.Make("sort", "sort", "Sorts the output"));
			Add(CodeCompletionItem.Make("strip-space", "strip-space", "Defines the elements for which white space should be removed"));
			Add(CodeCompletionItem.Make("stylesheet", "stylesheet", "Defines the root element of a style sheet"));
			Add(CodeCompletionItem.Make("template", "template", "Rules to apply when a specified node is matched"));
			Add(CodeCompletionItem.Make("text", "text", "Writes literal text to the output"));
			Add(CodeCompletionItem.Make("transform", "transform", "Defines the root element of a style sheet"));
			Add(CodeCompletionItem.Make("value-of", "value-of", "Extracts the value of a selected node"));
			Add(CodeCompletionItem.Make("variable", "variable", "Declares a local or global variable"));
			Add(CodeCompletionItem.Make("when", "when", "Specifies an action for the &lt;choose&gt; element"));
			Add(CodeCompletionItem.Make("with-param", "with-param", "Defines the value of a parameter to be passed into a template"));

		}
	}
}
