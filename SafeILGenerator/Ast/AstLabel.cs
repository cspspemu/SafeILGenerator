using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
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

		static public AstLabel CreateDummyWithName(string Name = "<Unknown>")
		{
			return new AstLabel(default(Label), Name);
		}

		static public AstLabel Create(ILGenerator ILGenerator, string Name = "<Unknown>")
		{
			return CreateFromLabel(ILGenerator.DefineLabel(), Name);
		}

		static public AstLabel CreateFromLabel(SafeLabel SafeLabel)
		{
			return CreateFromLabel(SafeLabel.ReflectionLabel, SafeLabel.Name);
		}

		static public AstLabel CreateFromLabel(Label Label, string Name = "<Unknown>")
		{
			return new AstLabel(Label, Name);
		}

		public override string ToString()
		{
			return String.Format("AstLabel({0})", Name);
		}
	}
}
