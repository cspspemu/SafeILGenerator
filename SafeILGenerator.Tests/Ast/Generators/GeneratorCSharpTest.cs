using System;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Ast;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SafeILGenerator.Tests.Ast.Generators
{
	[TestClass]
	public class GeneratorCSharpTest
	{
		GeneratorCSharp GeneratorCSharp;

		[TestInitialize]
		public void SetUp()
		{
			GeneratorCSharp = new GeneratorCSharp();
		}

		[TestMethod]
		public void TestAstExpression()
		{
			GeneratorCSharp.GenerateRoot(new AstNodeExprBinop(new AstNodeExprImm(3), "+", new AstNodeExprImm(5)));
			Assert.AreEqual("(3 + 5)", GeneratorCSharp.ToString());
		}

		[TestMethod]
		public void TestAstIf()
		{
			GeneratorCSharp.GenerateRoot(new AstNodeStmIfElse(new AstNodeExprImm(true), new AstNodeStmReturn(), new AstNodeStmReturn()));
			Assert.AreEqual("if (true) return; else return;", GeneratorCSharp.ToString());
		}

		[TestMethod]
		public void TestSimpleCall()
		{
			GeneratorCSharp.GenerateRoot(
				new AstNodeStmReturn(
					new AstNodeExprCallStatic(
						(Func<int, int>)GetTestValue,
						new AstNodeExprImm(10)
					)
				)
			);

			Assert.AreEqual("return GeneratorCSharpTest.GetTestValue(10);", GeneratorCSharp.ToString());
		}

		static private AstGenerator ast = AstGenerator.Instance;

		[TestMethod]
		public void TestAstSwitch()
		{
			var Local = AstLocal.Create<int>("Local");
			var Ast = ast.Statements(
				ast.Switch(
					ast.Local(Local),
					ast.Default(ast.Return("Nor One, nor Three")),
					ast.Case(1, ast.Return("One")),
					ast.Case(3, ast.Return("Three"))
				),
				ast.Return("Invalid!")
			);

			var Actual = GeneratorCSharp.GenerateString<GeneratorCSharp>(Ast);
			var Expected = @"
				{
					switch (Local) {
						case 1:
							return ""One"";
						break;
						case 3:
							return ""Three"";
						break;
						default:
							return ""Nor One, nor Three"";
						break;
					}
					return ""Invalid!"";
				}
			";
			Actual = new Regex(@"\s+").Replace(Actual, " ").Trim();
			Expected = new Regex(@"\s+").Replace(Expected, " ").Trim();

			Assert.AreEqual(Expected, Actual);
		}

		static public int GetTestValue(int Value)
		{
			return 333 * Value;
		}
	}
}
