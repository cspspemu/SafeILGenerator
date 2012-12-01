using System;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;
using Xunit;

namespace SafeILGenerator.Tests.Ast.Generators
{
	public class GeneratorCSharpTest
	{
		[Fact]
		public void TestAstExpression()
		{
			var GeneratorCSharp = new GeneratorCSharp();
			GeneratorCSharp.GenerateRoot(new AstNodeExprBinop(new AstNodeExprImm(3), "+", new AstNodeExprImm(5)));
			Assert.Equal("(3 + 5)", GeneratorCSharp.ToString());
		}

		[Fact]
		public void TestAstIf()
		{
			var GeneratorCSharp = new GeneratorCSharp();
			GeneratorCSharp.GenerateRoot(new AstNodeStmIfElse(new AstNodeExprImm(true), new AstNodeStmReturn(), new AstNodeStmReturn()));
			Assert.Equal("if (true) return; else return;", GeneratorCSharp.ToString());
		}

		[Fact]
		public void TestSimpleCall()
		{
			var Generator = new GeneratorCSharp();
			Generator.GenerateRoot(
				new AstNodeStmReturn(
					new AstNodeExprCallStatic(
						(Func<int, int>)GetTestValue,
						new AstNodeExprImm(10)
					)
				)
			);

			Assert.Equal("return GeneratorCSharpTest.GetTestValue(10);", Generator.ToString());
		}

		static public int GetTestValue(int Value)
		{
			return 333 * Value;
		}
	}
}
