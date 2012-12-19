using System;
using System.Linq;
using NUnit.Framework;

namespace SafeILGenerator.Tests
{
	[TestFixture]
	public class SafeILGeneratorTest
	{
		[Test]
		public void TestGenerate()
		{
			var Adder = CSafeILGenerator.Generate<Func<int, int, int>>("TestGenerate", (Generator) =>
			{
				Generator.LoadArgument<int>(0);
				Generator.LoadArgument<int>(1);
				Generator.BinaryOperation(SafeBinaryOperator.AdditionSigned);
				Generator.Return<int>();
			});
			Assert.AreEqual(3, Adder(1, 2));
		}

		[Test]
		public void TestLog()
		{
			CSafeILGenerator SafeILGenerator = null;

			var AdderPlus16 = CSafeILGenerator.Generate<Func<int, int, int>>("TestGenerate", (Generator) =>
			{
				SafeILGenerator = Generator;
				Generator.LoadArgument<int>(0);
				Generator.LoadArgument<int>(1);
				Generator.BinaryOperation(SafeBinaryOperator.AdditionSigned);
				Generator.Push((int)16);
				Generator.BinaryOperation(SafeBinaryOperator.AdditionSigned);
				Generator.Return<int>();
			}, DoLog: true);

			Assert.AreEqual(
				String.Join("\r\n", new string[] {
					"ldarg.0",
					"ldarg.1",
					"add",
					"ldc.i4.s, 16",
					"add",
					"ret",
				}),
				String.Join("\r\n", SafeILGenerator.GetEmittedInstructions())
			);
		}

		[Test]
		public void TestSwitch()
		{
			var Switcher = CSafeILGenerator.Generate<Func<int, int>>("TestSwitch", (Generator) =>
			{
				var Local = Generator.DeclareLocal<int>("Value", false);
				Generator.Push((int)-33);
				Generator.StoreLocal(Local);

				Generator.LoadArgument<int>(0);
				Generator.Switch(
					// List
					new int[] { 0, 2, 3 },
					// Integer Selector
					(Value) => Value,
					// Case
					(Value) =>
					{
						Generator.Push(Value);
						Generator.StoreLocal(Local);
					},
					// Default
					() =>
					{
						Generator.Push(-99);
						Generator.StoreLocal(Local);
					}
				);
				Generator.LoadLocal(Local);
				Generator.Return<int>();
			});

			var ExpectedItems = new int[] { -99, 0, -99, 2, 3, -99 };
			var GeneratedItems = new int[] { -1, 0, 1, 2, 3, 4 }.Select(Item => Switcher(Item));

			Assert.AreEqual(ExpectedItems.ToArray(), GeneratedItems.ToArray());
		}
	}
}
