using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Ast.Generators;
using System.Reflection.Emit;
using System.Reflection;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Utils;

namespace SafeILGenerator.Tests.Ast.Generators
{
	[TestClass]
	public unsafe class GeneratorILTest
	{
		static private AstGenerator ast = AstGenerator.Instance;

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
				Generator.GenerateRoot(new AstNodeStmReturn(new AstNodeExprImm(777)));
			});

			Assert.AreEqual(777, Func());
		}
		
		[TestMethod]
		public void TestSimpleCall()
		{
			var Func = GenerateDynamicMethod<Func<int>>("Test", (DynamicMethod, ILGenerator) =>
			{
				var Generator = new GeneratorIL(DynamicMethod, ILGenerator);
				Generator.GenerateRoot(
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
				Generator.GenerateRoot(
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

				var TestArgument = ast.Argument<TestClass>(0, "Test");

				var AstNode = ast.Statements(
					ast.Assign(
						ast.FieldAccess(TestArgument, "Test"),
						ast.Immediate(456)
					),
					ast.Return(
						ast.FieldAccess(TestArgument, "Test")
					)
				);
	
				Console.WriteLine(new GeneratorCSharp().GenerateRoot((AstNode)AstNode).ToString());

				new GeneratorIL(DynamicMethod, ILGenerator).GenerateRoot(AstNode);
			});

			Assert.AreEqual(456, Func(new TestClass()));
		}

		delegate void ActionPointer(void* Pointer);

		[TestMethod]
		public void TestPointerWrite()
		{
			var Func = GenerateDynamicMethod<ActionPointer>("Test", (DynamicMethod, ILGenerator) =>
			{
				var AstNode = ast.Statements(
					ast.Assign(
					ast.Indirect(ast.Cast(typeof(int*), ast.Argument(typeof(int*), 0, "Ptr"))),
						ast.Immediate(456)
					),
					ast.Return()
				);

				Console.WriteLine(new GeneratorCSharp().GenerateRoot((AstNode)AstNode).ToString());

				new GeneratorIL(DynamicMethod, ILGenerator).GenerateRoot(AstNode);
			});

			var Data = new int[1];
			fixed (int* DataPtr = Data)
			{
				Func(DataPtr);
			}

			Assert.AreEqual(456, Data[0]);
		}

        [TestMethod]
        public void TestPointerWrite_bool()
        {
            var Func = GenerateDynamicMethod<ActionPointer>("Test", (DynamicMethod, ILGenerator) =>
            {
                var AstNode = ast.Statements(
                    ast.Assign(
                    ast.Indirect(ast.Cast(typeof(bool*), ast.Argument(typeof(bool*), 0, "Ptr"))),
                        ast.Immediate(true)
                    ),
                    ast.Return()
                );

                Console.WriteLine(new GeneratorCSharp().GenerateRoot((AstNode)AstNode).ToString());

                new GeneratorIL(DynamicMethod, ILGenerator).GenerateRoot(AstNode);
            });

			foreach (var FillValue in new bool[] { false, true })
			{
				var Data = new bool[8];
				for (int n = 0; n < Data.Length; n++) Data[n] = FillValue;
				Data[0] = false;

				fixed (bool* DataPtr = Data)
				{
					Func(DataPtr);
				}

				Assert.AreEqual(true, Data[0]);
				for (int n = 1; n < 8; n++) Assert.AreEqual(FillValue, Data[n]);
			}
        }

		[TestMethod]
		public void TestWriteLineLoadString()
		{
			var Ast = ast.Statements(
				ast.Statement(ast.CallStatic((Action<string>)Console.WriteLine, ast.Argument<string>(0))),
				ast.Statement(ast.CallStatic((Action<string>)Console.WriteLine, "Goodbye World!")),
				ast.Return()
			);

			var Method = GeneratorIL.GenerateDelegate<GeneratorIL, Action<string>>("TestWriteLine", Ast);

			Console.WriteLine(new GeneratorCSharp().GenerateRoot(Ast).ToString());
			Console.WriteLine("{0}", GeneratorIL.GenerateToString<GeneratorIL>(Method.Method, Ast));

			var Output = AstStringUtils.CaptureOutput(() =>
			{
				Method("Hello World!");
			});

			Assert.AreEqual("Hello World!\r\nGoodbye World!\r\n", Output);
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
