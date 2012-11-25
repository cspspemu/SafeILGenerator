using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Ast.Generators;
using System.Reflection.Emit;
using System.Reflection;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Ast;

namespace SafeILGenerator.Tests.Ast.Generators
{
	[TestClass]
	public unsafe class GeneratorILTest : IAstGenerator
	{
		static public TDelegate GenerateDynamicMethod<TDelegate>(string MethodName, Action<DynamicMethod, ILGenerator> Generator, bool CheckTypes = true, bool DoDebug = false, bool DoLog = false)
		{
			var MethodInfo = typeof(TDelegate).GetMethod("Invoke");
			var DynamicMethod = new DynamicMethod(
				MethodName,
				MethodInfo.ReturnType,
				MethodInfo.GetParameters().Select(Parameter => Parameter.ParameterType).ToArray(),
				Assembly.GetExecutingAssembly().ManifestModule
			);
			var ILGenerator = DynamicMethod.GetILGenerator();
			{
				Generator(DynamicMethod, ILGenerator);
			}
			return (TDelegate)(object)DynamicMethod.CreateDelegate(typeof(TDelegate));
		}

		[TestMethod]
		public void TestSimpleReturn()
		{
			var Func = GenerateDynamicMethod<Func<int>>("Test", (DynamicMethod, ILGenerator) =>
			{
				var Generator = new GeneratorIL(DynamicMethod, ILGenerator);
				Generator.Generate(new AstNodeStmReturn(new AstNodeExprImm(777)));
			});

			Assert.AreEqual(777, Func());
		}
		
		[TestMethod]
		public void TestSimpleCall()
		{
			var Func = GenerateDynamicMethod<Func<int>>("Test", (DynamicMethod, ILGenerator) =>
			{
				var Generator = new GeneratorIL(DynamicMethod, ILGenerator);
				Generator.Generate(
					new AstNodeStmReturn(
						new AstNodeExprCallStatic(
							(Func<int, int>)GetTestValue,
							new AstNodeExprImm(10)
						)
					)
				);
			});

			Assert.AreEqual(3330, Func());
		}
		
		[TestMethod]
		public void TestSimpleLocal()
		{
			var Func = GenerateDynamicMethod<Func<int>>("Test", (DynamicMethod, ILGenerator) =>
			{
				var TestLocal = AstLocal.Create(ILGenerator, typeof(int), "TestLocal");

				var Generator = new GeneratorIL(DynamicMethod, ILGenerator);
				Generator.Generate(
					new AstNodeStmContainer(
						new AstNodeStmAssign(
							new AstNodeExprLocal(TestLocal),
							new AstNodeExprImm(123)
						),
						new AstNodeStmReturn(
							new AstNodeExprLocal(TestLocal)
						)
					)
				);
			});

			Assert.AreEqual(123, Func());
		}

		[TestMethod]
		public void TestFieldAccess()
		{
			var Func = GenerateDynamicMethod<Func<TestClass, int>>("Test", (DynamicMethod, ILGenerator) =>
			{
				var TestLocal = AstLocal.Create(ILGenerator, typeof(int), "TestLocal");

				var TestArgument = this.Argument<TestClass>(0, "Test");

				var AstNode = this.Statements(
					this.Assign(
						this.FieldAccess(TestArgument, "Test"),
						this.Immediate(456)
					),
					this.Return(
						this.FieldAccess(TestArgument, "Test")
					)
				);
	
				Console.WriteLine(new GeneratorCSharp().Generate((AstNode)AstNode).ToString());

				new GeneratorIL(DynamicMethod, ILGenerator).Generate(AstNode);
			});

			Assert.AreEqual(456, Func(new TestClass()));
		}

		delegate void ActionPointer(void* Pointer);

		[TestMethod]
		public void TestPointerWrite()
		{
			var Func = GenerateDynamicMethod<ActionPointer>("Test", (DynamicMethod, ILGenerator) =>
			{
				var AstNode = this.Statements(
					this.Assign(
					this.Indirect(this.Cast(typeof(int*), this.Argument(typeof(int*), 0, "Ptr"))),
						this.Immediate(456)
					),
					this.Return()
				);

				Console.WriteLine(new GeneratorCSharp().Generate((AstNode)AstNode).ToString());

				new GeneratorIL(DynamicMethod, ILGenerator).Generate(AstNode);
			});

			var Data = new int[1];
			fixed (int* DataPtr = Data)
			{
				Func(DataPtr);
			}

			Assert.AreEqual(456, Data[0]);
		}

		public class TestClass
		{
			public int Test;
		}

		static public int GetTestValue(int Value)
		{
			return 333 * Value;
		}
	}
}
