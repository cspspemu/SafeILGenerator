using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstArgument
	{
		public readonly int Index;
		public readonly Type Type;
		public readonly string Name;

		public AstArgument(int Index, Type Type, string Name = null)
		{
			this.Index = Index;
			this.Type = Type;
			this.Name = (Name == null) ? ("@ARG(" + Index + ")") : Name;
		}

		static public AstArgument Create(MethodInfo MethodInfo, ILGenerator ILGenerator, int Index, string Name = null)
		{
			return new AstArgument(Index, MethodInfo.GetParameters()[Index].ParameterType, Name);
		}
	}
}
