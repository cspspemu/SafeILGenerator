using System;
using SafeILGenerator.Ast.Utils;
using Xunit;

namespace SafeILGenerator.Tests.Ast.Utils
{
	public class IndentedStringBuilderTest
	{
		[Fact]
		public void TestIndentation()
		{
			var Output = new IndentedStringBuilder();
			Output.Write("{\n");
			Output.Indent(() =>
			{
				Output.Write("Hello World!\n");
				Output.Write("Goodbye World!\n");
			});
			Output.Write("}\n");
			Assert.Equal(
				@"{\n" +
				@"    Hello World!\n" +
				@"    Goodbye World!\n" +
				@"}\n",
				AstStringUtils.ToLiteralRaw(Output.ToString())
			);
		}
	}
}
