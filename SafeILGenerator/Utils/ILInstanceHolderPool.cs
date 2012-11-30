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
	public class ILInstanceHolderPoolItem<TType>
	{
		private ILInstanceHolderPoolItem Item;
		public int Index { get { return Item.Index; } }
		public FieldInfo FieldInfo { get { return Item.FieldInfo; } }

		public ILInstanceHolderPoolItem(ILInstanceHolderPoolItem Item)
		{
			this.Item = Item;
		}

		public TType Value
		{
			set
			{
				Item.Value = value;
			}
			get
			{
				return (TType)Item.Value;
			}
		}

		public void Free()
		{
			Item.Free();
		}
	}

	public class ILInstanceHolderPoolItem
	{
		private readonly ILInstanceHolderPool Parent;
		public readonly int Index;
		public readonly FieldInfo FieldInfo;

		public ILInstanceHolderPoolItem(ILInstanceHolderPool Parent, int Index, FieldInfo FieldInfo)
		{
			this.Parent = Parent;
			this.Index = Index;
			this.FieldInfo = FieldInfo;
		}

		public object Value
		{
			set
			{
				FieldInfo.SetValue(null, value);
			}
			get
			{
				return FieldInfo.GetValue(null);
			}
		}

		public void Free()
		{
			Parent.Free(this);
		}
	}

	public class ILInstanceHolderPool
	{
		private static AstGenerator ast = AstGenerator.Instance;

		public readonly Type ItemType;
		private ILInstanceHolderPoolItem[] FieldInfos;
		private LinkedList<int> FreeItems = new LinkedList<int>();
		private Type HolderType;
		private static int Autoincrement = 0;

		public int FreeCount
		{
			get
			{
				return FreeItems.Count;
			}
		}

		public bool HasAvailable
		{
			get
			{
				return FreeCount > 0;
			}
		}

		public ILInstanceHolderPoolItem Alloc()
		{
			var Item = FieldInfos[FreeItems.First.Value];
			FreeItems.RemoveFirst();
			Item.Value = null;
			return Item;
		}

		internal void Free(ILInstanceHolderPoolItem Item)
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

		public ILInstanceHolderPool(Type ItemType, int Count, string TypeName = null)
		{
			this.ItemType = ItemType;
			if (TypeName == null) TypeName = "DynamicType" + Autoincrement++;
			var TypeBuilder = ModuleBuilder.DefineType(TypeName, TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Class);
			FieldInfos = new ILInstanceHolderPoolItem[Count];
			for (int n = 0; n < Count; n++)
			{
				TypeBuilder.DefineField("V" + n, ItemType, FieldAttributes.Public | FieldAttributes.Static);
			}

			HolderType = TypeBuilder.CreateType();

			for (int n = 0; n < Count; n++)
			{
				FieldInfos[n] = new ILInstanceHolderPoolItem(this, n, HolderType.GetField("V" + n));
				FreeItems.AddLast(n);
			}
		}
	}
}
