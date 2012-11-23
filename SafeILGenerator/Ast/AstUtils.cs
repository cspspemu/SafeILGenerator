using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstUtils
	{
		static public bool IsTypeSigned(Type Type)
		{
			if (!Type.IsPrimitive) return false;
			return (
				Type == typeof(sbyte) ||
				Type == typeof(short) ||
				Type == typeof(int) ||
				Type == typeof(long) ||
				Type == typeof(float) ||
				Type == typeof(double) ||
				Type == typeof(decimal)
			);
		}
	}
}
