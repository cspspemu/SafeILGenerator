using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codegen.Tests
{
	[TestClass]
	public class SafeILGeneratorTest
	{
		[TestMethod]
		public void TestGenerate()
		{
			var Adder = SafeILGenerator.Generate<Func<int, int, int>>((Generator) =>
			{
				Generator.LoadArgument<int>(0);
				Generator.LoadArgument<int>(1);
				Generator.BinaryOperation(SafeBinaryOperator.AdditionSigned);
				Generator.Return();
			});
			Assert.AreEqual(3, Adder(1, 2));
		}

		[TestMethod]
		public void TestSwitch()
		{
			var Switcher = SafeILGenerator.Generate<Func<int, int>>((Generator) =>
			{
				var Local = Generator.DeclareLocal<int>("Value");
				Generator.Push((int)-2);
				Generator.StoreLocal(Local);

				Generator.LoadArgument<int>(0);
				Generator.Switch(
					// List
					new int[] { 0, 3, 5 },
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
						Generator.Push(-1);
						Generator.StoreLocal(Local);
					}
				);
				Generator.LoadLocal(Local);
				Generator.Return();
			});
			Assert.AreEqual(3, Switcher(3));
		}
	}
}
