using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Optimizers;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;

namespace SafeILGenerator.Tests.Ast.Optimizers
{
	[TestClass]
	public class AstOptimizerTest : IAstGenerator
	{
		[TestMethod]
		public void TestCalculateImmediates()
		{
			var OptimizedNode = new AstOptimizer().Optimize(
				(this.Immediate(0) + this.Immediate(2)) * this.Immediate(3)
			);
			Assert.AreEqual("6", new GeneratorCSharp().Generate(OptimizedNode).ToString());
			//Console.WriteLine("{0}", Opti);
		}

		[TestMethod]
		public void TestAdd0()
		{
			var OptimizedNode = new AstOptimizer().Optimize(
				(this.Argument<int>(0, "Arg") + this.Immediate(0)) * this.Immediate(3)
			);
			Assert.AreEqual("(Arg * 3)", new GeneratorCSharp().Generate(OptimizedNode).ToString());
			//Console.WriteLine("{0}", Opti);
		}

		[TestMethod]
		public void TestTripleCast()
		{
			var Node = (AstNode)this.Cast<int>(this.Cast<uint>(this.Immediate((int)7)));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("7", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastToImmediate()
		{
			var Node = (AstNode)this.Cast<uint>(this.Immediate((int)7));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("7", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastSignExtend()
		{
			var Node = (AstNode)this.Cast<uint>(this.Cast<sbyte>(this.Argument<int>(0, "Arg")));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("(UInt32)(SByte)Arg", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastSignExtend2()
		{
			var Node = (AstNode)this.Cast<sbyte>(this.Cast<uint>(this.Argument<int>(0, "Arg")));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("(SByte)Arg", new GeneratorCSharp().Generate(Node).ToString());
		}
	}
}
