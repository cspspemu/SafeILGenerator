using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Utils;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast;

namespace SafeILGenerator.Tests.Util
{
	[TestClass]
	public class ILInstanceHolderPoolTest
	{
		private static AstGenerator ast = AstGenerator.Instance;

		[TestMethod]
		public void TestMethod1()
		{
			var Pool = new ILInstanceHolderPool<int>(16);
			var Item = Pool.Alloc();
			Item.Set(10);
			Assert.AreEqual(10, Item.Get());
		}

		[TestMethod]
		public void TestMethod2()
		{
			var Pool = new ILInstanceHolderPool<int>(16);
			var Item = Pool.Alloc();
			var AstNode = ast.Statements(
				ast.Assign(ast.StaticFieldAccess(Item.FieldInfo), ast.Argument<int>(0, "Value")),
				ast.Return()
			);
			Console.WriteLine(GeneratorCSharp.GenerateString<GeneratorCSharp>(AstNode));
			var ItemSet = GeneratorIL.GenerateDelegate<GeneratorIL, Action<int>>("ItemSet", AstNode);
			ItemSet(10);
			Assert.AreEqual(10, Item.Get());
		}

		[TestMethod]
		public void TestMethod3()
		{
			var Pool1 = new ILInstanceHolderPool<int>(16);
			var Pool2 = new ILInstanceHolderPool<int>(16);
			var Item1 = Pool1.Alloc();
			var Item2 = Pool2.Alloc();
			Item1.Set(11);
			Item2.Set(22);
			Assert.AreEqual(11, Item1.Get());
			Assert.AreEqual(22, Item2.Get());
		}
	}
}
