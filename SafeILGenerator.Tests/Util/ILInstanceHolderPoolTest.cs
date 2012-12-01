using System;
using SafeILGenerator.Utils;
using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast;
using Xunit;

namespace SafeILGenerator.Tests.Util
{
	public class ILInstanceHolderPoolTest
	{
		private static AstGenerator ast = AstGenerator.Instance;

		[Fact]
		public void TestAllocAssignGetAndRelease()
		{
			var Pool = new ILInstanceHolderPool(typeof(int), 16);
			var Item = Pool.Alloc();
			Item.Value = 10;
			Assert.Equal(10, Item.Value);
			Assert.Equal(15, Pool.FreeCount);
			Item.Free();
			Assert.Equal(16, Pool.FreeCount);
		}

		[Fact]
		public void TestMethod2()
		{
			var Pool = new ILInstanceHolderPool(typeof(int), 16);
			var Item = Pool.Alloc();
			var AstNode = ast.Statements(
				ast.Assign(ast.StaticFieldAccess(Item.FieldInfo), ast.Argument<int>(0, "Value")),
				ast.Return()
			);
			Console.WriteLine(GeneratorCSharp.GenerateString<GeneratorCSharp>(AstNode));
			var ItemSet = GeneratorIL.GenerateDelegate<GeneratorIL, Action<int>>("ItemSet", AstNode);
			ItemSet(10);
			Assert.Equal(10, Item.Value);
		}

		[Fact]
		public void TestMethod3()
		{
			var Pool1 = new ILInstanceHolderPool(typeof(int), 16);
			var Pool2 = new ILInstanceHolderPool(typeof(int), 16);
			var Item1 = Pool1.Alloc();
			var Item2 = Pool2.Alloc();
			Item1.Value = 11;
			Item2.Value = 22;
			Assert.Equal(11, Item1.Value);
			Assert.Equal(22, Item2.Value);
		}

		[Fact]
		public void TestGlobalAlloc()
		{
			Assert.Equal(0, ILInstanceHolder.CapacityCount);
			Assert.Equal(0, ILInstanceHolder.FreeCount);
			
			var GlobalKey = ILInstanceHolder.TAlloc<int>();

			Assert.Equal(4, ILInstanceHolder.CapacityCount);
			Assert.Equal(3, ILInstanceHolder.FreeCount);

			GlobalKey.Value = 10;
			GlobalKey.Free();

			Assert.Equal(4, ILInstanceHolder.CapacityCount);
			Assert.Equal(4, ILInstanceHolder.FreeCount);
		}

	}
}
