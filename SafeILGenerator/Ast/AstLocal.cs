using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	sealed public class AstLocal
	{
		private LocalBuilder LocalBuilder;
		public LocalBuilder GetLocalBuilderForILGenerator(ILGenerator ILGenerator)
		{
			if (this.LocalBuilder == null)
			{
				this.LocalBuilder = ILGenerator.DeclareLocal(Type);
			}
			return this.LocalBuilder;
		}
		public readonly string Name;
		public readonly Type Type;

		//public Type Type { get { return LocalBuilder.LocalType; } }

		protected AstLocal(Type Type, string Name)
		{
			this.Name = Name;
			this.Type = Type;
		}

		static public AstLocal Create(Type Type, string Name = "<Unknown>")
		{
			return new AstLocal(Type, Name);
		}
	}
}
