using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Optimizers;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using SafeILGenerator.Ast.Serializers;

namespace SafeILGenerator.Tests.Ast.Optimizers
{
	[TestClass]
	public class AstOptimizerTest
	{
		static private AstGenerator ast = AstGenerator.Instance;

		[TestMethod]
		public void TestCalculateImmediates()
		{
			var OptimizedNode = new AstOptimizer().Optimize(
				(ast.Immediate(0) + ast.Immediate(2)) * ast.Immediate(3)
			);
			Assert.AreEqual("6", new GeneratorCSharp().Generate(OptimizedNode).ToString());
			//Console.WriteLine("{0}", Opti);
		}

		[TestMethod]
		public void TestAdd0()
		{
			var OptimizedNode = new AstOptimizer().Optimize(
				(ast.Argument<int>(0, "Arg") + ast.Immediate(0)) * ast.Immediate(3)
			);
			Assert.AreEqual("(Arg * 3)", new GeneratorCSharp().Generate(OptimizedNode).ToString());
			//Console.WriteLine("{0}", Opti);
		}

		[TestMethod]
		public void TestTripleCast()
		{
			var Node = (AstNode)ast.Cast<int>(ast.Cast<uint>(ast.Immediate((int)7)));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("7", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastToImmediate()
		{
			var Node = (AstNode)ast.Cast<uint>(ast.Immediate((int)7));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("7", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastSignExtend()
		{
			var Node = (AstNode)ast.Cast<uint>(ast.Cast<sbyte>(ast.Argument<int>(0, "Arg")));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("(UInt32)(SByte)Arg", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestCastSignExtend2()
		{
			var Node = (AstNode)ast.Cast<sbyte>(ast.Cast<uint>(ast.Argument<int>(0, "Arg")));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("(SByte)Arg", new GeneratorCSharp().Generate(Node).ToString());
		}

		[TestMethod]
		public void TestZeroMinusNumber()
		{
			var Node = (AstNode)ast.Binary(ast.Immediate(0), "-", ast.Argument<int>(0, "Arg"));
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual("(-Arg)", new GeneratorCSharp().Generate(Node).ToString());
		}

		static XmlSerializer AstNodeSerializer = new XmlSerializer(typeof(AstNode));

		[TestMethod]
		public void TestCompactStmContainer()
		{
			var Node = (AstNode)ast.Statements(
				ast.Return(),
				ast.Statements(
					ast.Statements(
						ast.Return()
					),
					ast.Return(),
					ast.Statements(ast.Statements(ast.Statements())),
					ast.Return(),
					ast.Statements(
						ast.Return()
					)
				),
				ast.Return()
			);
			Node = new AstOptimizer().Optimize(Node);
			Assert.AreEqual(
				"AstNodeStmContainer(AstNodeStmReturn(), AstNodeStmReturn(), AstNodeStmReturn(), AstNodeStmReturn(), AstNodeStmReturn(), AstNodeStmReturn())",
				AstSerializer.Serialize(Node)
			);
		}
	}
}
