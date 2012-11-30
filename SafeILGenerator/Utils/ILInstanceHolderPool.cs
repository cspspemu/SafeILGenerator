using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Utils
{
	public class ILInstanceHolderPool<TType>
	{
		private static AstGenerator ast = AstGenerator.Instance;

		public class Item
		{
			public int Index;
			public FieldInfo FieldInfo;

			public void Set(TType Value)
			{
				FieldInfo.SetValue(null, Value);
#if false
				var Delegate = GeneratorIL.GenerateDelegate<GeneratorIL, Action<TType>>(
					"",
					ast.Statements(
						ast.Assign(ast.StaticFieldAccess(FieldInfo), ast.Argument<TType>(0, "Value")),
						ast.Return()
					)
				);
				Delegate(Value);
#endif
			}

			public TType Get()
			{
				return (TType)FieldInfo.GetValue(null);
			}
		}

		private Item[] FieldInfos;
		private LinkedList<int> FreeItems = new LinkedList<int>();
		private Type HolderType;
		private static int Autoincrement = 0;

		public Item Alloc()
		{
			var Item = FieldInfos[FreeItems.First.Value];
			FreeItems.RemoveFirst();
			Item.Set(default(TType));
			return Item;
		}

		public void Free(Item Item)
		{
			FreeItems.AddLast(Item.Index);
		}

		private static string DllName = "Temp.dll";
		private static AssemblyBuilder AssemblyBuilder;
		private static ModuleBuilder ModuleBuilder;

		static ILInstanceHolderPool()
		{
			AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName("DynamicAssembly" + Autoincrement++),
				AssemblyBuilderAccess.RunAndCollect,
				DllName
			);
			ModuleBuilder = AssemblyBuilder.DefineDynamicModule(AssemblyBuilder.GetName().Name, DllName, true);
		}

		public ILInstanceHolderPool(int Count, string TypeName = null)
		{
			if (TypeName == null) TypeName = "DynamicType" + Autoincrement++;
			var TypeBuilder = ModuleBuilder.DefineType(TypeName, TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Class);
			FieldInfos = new Item[Count];
			for (int n = 0; n < Count; n++)
			{
				TypeBuilder.DefineField("V" + n, typeof(TType), FieldAttributes.Public | FieldAttributes.Static);
			}

			HolderType = TypeBuilder.CreateType();

			for (int n = 0; n < Count; n++)
			{
				FieldInfos[n] = new Item()
				{
					Index = n,
					FieldInfo = HolderType.GetField("V" + n)
				};
				FreeItems.AddLast(n);
			}
		}
	}
}
