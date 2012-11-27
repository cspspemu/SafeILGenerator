using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;

namespace SafeILGenerator.Tests.Ast.Generators
{
	[TestClass]
	public class GeneratorCSharpTest
	{
		[TestMethod]
		public void TestAstExpression()
		{
			var GeneratorCSharp = new GeneratorCSharp();
			GeneratorCSharp.GenerateRoot(new AstNodeExprBinop(new AstNodeExprImm(3), "+", new AstNodeExprImm(5)));
			Assert.AreEqual("(3 + 5)", GeneratorCSharp.ToString());
		}

		[TestMethod]
		public void TestAstIf()
		{
			var GeneratorCSharp = new GeneratorCSharp();
			GeneratorCSharp.GenerateRoot(new AstNodeStmIfElse(new AstNodeExprImm(true), new AstNodeStmReturn(), new AstNodeStmReturn()));
			Assert.AreEqual("if (true) return; else return;", GeneratorCSharp.ToString());
		}

		[TestMethod]
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

			Assert.AreEqual("return GeneratorCSharpTest.GetTestValue(10);", Generator.ToString());
		}

		static public int GetTestValue(int Value)
		{
			return 333 * Value;
		}
	}
}
