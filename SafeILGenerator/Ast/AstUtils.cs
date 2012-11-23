using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstUtils
	{
		static public Type GetSignedType(Type Type)
		{
			if (Type == typeof(byte)) return typeof(sbyte);
			if (Type == typeof(ushort)) return typeof(short);
			if (Type == typeof(uint)) return typeof(int);
			if (Type == typeof(ulong)) return typeof(long);
			return Type;
		}

		static public Type GetUnsignedType(Type Type)
		{
			if (Type == typeof(sbyte)) return typeof(byte);
			if (Type == typeof(short)) return typeof(ushort);
			if (Type == typeof(int)) return typeof(uint);
			if (Type == typeof(long)) return typeof(ulong);
			return Type;
		}

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

		static public object CastType(object Value, Type CastType)
		{
			if (CastType == typeof(sbyte)) return (sbyte)Convert.ToInt64(Value);
			if (CastType == typeof(short)) return (short)Convert.ToInt64(Value);
			if (CastType == typeof(int)) return (int)Convert.ToInt64(Value);
			if (CastType == typeof(long)) return (long)Convert.ToInt64(Value);

			if (CastType == typeof(byte)) return (byte)Convert.ToInt64(Value);
			if (CastType == typeof(ushort)) return (ushort)Convert.ToInt64(Value);
			if (CastType == typeof(uint)) return (uint)Convert.ToInt64(Value);
			if (CastType == typeof(ulong)) return (ulong)Convert.ToInt64(Value);

			if (CastType == typeof(float)) return Convert.ToSingle(Value);
			if (CastType == typeof(double)) return Convert.ToDouble(Value);

			throw(new NotImplementedException());
		}
	}
}
