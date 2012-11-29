using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Utils
{
	public class AstStringUtils
	{
		public static string ToLiteralRaw(string input)
		{
			var Output = ToLiteral(input);
			return Output.Substring(1, Output.Length - 2);
		}

		public static string ToLiteral(string input)
		{
			using (var writer = new StringWriter())
			{
				using (var provider = CodeDomProvider.CreateProvider("CSharp"))
				{
					provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
					return writer.ToString();
				}
			}
		}

		public static String CaptureOutput(Action Action, bool Capture = true)
		{
			if (Capture)
			{
				var OldOut = Console.Out;
				var StringWriter = new StringWriter();
				try
				{
					Console.SetOut(StringWriter);
					Action();
				}
				finally
				{
					Console.SetOut(OldOut);
				}
				try
				{
					return StringWriter.ToString();
				}
				catch
				{
					return "";
				}
			}
			else
			{
				Action();
				return "";
			}
		}
	}
}
