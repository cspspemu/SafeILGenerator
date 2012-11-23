using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstLocal
	{
		public readonly LocalBuilder LocalBuilder;
		public readonly string Name;

		public Type Type { get { return LocalBuilder.LocalType; } }

		protected AstLocal(LocalBuilder LocalBuilder, string Name)
		{
			this.LocalBuilder = LocalBuilder;
			this.Name = Name;
		}

		static public AstLocal Create(ILGenerator ILGenerator, Type Type, string Name = "<Unknown>")
		{
			return new AstLocal(ILGenerator.DeclareLocal(Type), Name);
		}
	}
}
