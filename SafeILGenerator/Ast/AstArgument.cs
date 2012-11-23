using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast
{
	public class AstArgument
	{
		public readonly int Index;
		public readonly Type Type;

		public AstArgument(int Index, Type Type)
		{
			this.Index = Index;
			this.Type = Type;
		}

		static public AstArgument Create(MethodInfo MethodInfo, ILGenerator ILGenerator, int Index)
		{
			return new AstArgument(Index, MethodInfo.GetParameters()[Index].ParameterType);
		}
	}
}
