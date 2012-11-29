using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Utils
{
	public sealed class IndentedStringBuilder
	{
		private StringBuilder StringBuilder = new StringBuilder();
		private bool StartingLine = true;
		private int IndentLevel = 0;
		private int IndentLevelSpaceCount = 4;

		public void Indent(Action Action)
		{
			IndentLevel++;
			try
			{
				Action();
			}
			finally
			{
				IndentLevel--;
			}
		}

		private void WriteNormalText(string InlineText)
		{
			if (InlineText.Length > 0)
			{
				if (StartingLine)
				{
					StartingLine = false;
					StringBuilder.Append(new String(' ', IndentLevel * IndentLevelSpaceCount));
				}
				StringBuilder.Append(InlineText);
			}
		}

		private void WriteNewLine()
		{
			StartingLine = true;
			StringBuilder.Append("\n");
		}

		public void Write(string Text)
		{
			var Lines = Text.Split('\n');
			for (int n = 0; n < Lines.Length; n++)
			{
				if (n > 0)
				{
					WriteNewLine();
				}
				WriteNormalText(Lines[n]);
			}
		}

		public override string ToString()
		{
			return StringBuilder.ToString();
		}
	}
}
