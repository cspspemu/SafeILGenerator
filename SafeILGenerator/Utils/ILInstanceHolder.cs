using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Utils
{
	public class ILInstanceHolder
	{
		private static Dictionary<Type, List<ILInstanceHolderPool>> TypePools = new Dictionary<Type, List<ILInstanceHolderPool>>();

		public static ILInstanceHolderPoolItem Alloc(Type Type, object Value = null)
		{
			lock (TypePools)
			{
				if (!TypePools.ContainsKey(Type))
				{
					TypePools[Type] = new List<ILInstanceHolderPool>();
				}
				var PoolsType = TypePools[Type];
				var FreePool = PoolsType.Where(Pool => Pool.HasAvailable).FirstOrDefault();
				if (FreePool == null)
				{
					PoolsType.Add(FreePool = new ILInstanceHolderPool(Type, 1 << (PoolsType.Count + 2)));
				}
				var Item = FreePool.Alloc();
				Item.Value = Value;
				return Item;
			}
		}

		public static ILInstanceHolderPoolItem<TType> TAlloc<TType>(TType Value = default(TType))
		{
			return new ILInstanceHolderPoolItem<TType>(Alloc(typeof(TType), Value));
		}

		public static int FreeCount
		{
			get
			{
				return TypePools.Values.Sum(Pools => Pools.Sum(Pool => Pool.FreeCount));
			}
		}

		public static int CapacityCount
		{
			get
			{
				return TypePools.Values.Sum(Pools => Pools.Sum(Pool => Pool.CapacityCount));
			}
		}
	}
}
