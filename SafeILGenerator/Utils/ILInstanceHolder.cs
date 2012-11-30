using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Utils
{
	public class ILInstanceHolder
	{
		private static Dictionary<Type, List<ILInstanceHolderPool>> Pools = new Dictionary<Type, List<ILInstanceHolderPool>>();

		public static ILInstanceHolderPoolItem Alloc(Type Type, object Value = null)
		{
			lock (Pools)
			{
				if (!Pools.ContainsKey(Type))
				{
					Pools[Type] = new List<ILInstanceHolderPool>();
				}
				var PoolsType = Pools[Type];
				var FreePool = PoolsType.Where(Pool => Pool.HasAvailable).FirstOrDefault();
				if (FreePool == null)
				{
					PoolsType.Add(FreePool = new ILInstanceHolderPool(Type, 1 << (PoolsType.Count + 2)));
				}
				return FreePool.Alloc();
			}
		}

		public static ILInstanceHolderPoolItem<TType> Alloc<TType>(TType Value = default(TType))
		{
			return new ILInstanceHolderPoolItem<TType>(Alloc(typeof(TType), Value));
		}
	}
}
