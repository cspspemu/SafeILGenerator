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

		public void Write(string Text)
		{
			var Lines = Text.Split('\n');
			for (int n = 0; n < Lines.Length; n++)
			{
				var Line = Lines[n];
				if (n > 0)
				{
					StringBuilder.Append("\n");
					StartingLine = true;
				}
				if (StartingLine)
				{
					StartingLine = false;
					StringBuilder.Append(new String(' ', IndentLevel * IndentLevelSpaceCount));
				}
				StringBuilder.Append(Line);
			}
		}

		public override string ToString()
		{
			return StringBuilder.ToString();
		}
	}
}
