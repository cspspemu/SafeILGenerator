using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codegen.Ast.Generators;
using Codegen.Ast.Nodes;

namespace Codegen.Tests.Ast.Generators
{
	[TestClass]
	public class GeneratorCSharpTest
	{
		[TestMethod]
		public void TestAstExpression()
		{
			var GeneratorCSharp = new GeneratorCSharp();
			GeneratorCSharp.Generate(new AstNodeExprBinop(new AstNodeExprImm(3), "+", new AstNodeExprImm(5)));
			Assert.AreEqual("(3 + 5)", GeneratorCSharp.ToString());
		}
	}
}
