using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast
{
	public class AstLabel
	{
		public readonly Label Label;
		public readonly string Name;

		protected AstLabel(Label Label, string Name)
		{
			this.Label = Label;
			this.Name = Name;
		}

		static public AstLabel Create(ILGenerator ILGenerator, string Name = "<Unknown>")
		{
			return new AstLabel(ILGenerator.DefineLabel(), Name);
		}
	}
}
