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
		public Label Label;
		public string Name;

		protected AstLabel(Label Label, string Name)
		{
			this.Label = Label;
			this.Name = Name;
		}

		static public AstLabel CreateDelayedWithName(string Name = "<Unknown>")
		{
			return new AstLabel(default(Label), Name);
		}

		static public AstLabel CreateFromLabel(Label Label, string Name = "<Unknown>")
		{
			return new AstLabel(Label, Name);
		}

		//static public AstLabel CreateNewLabelFromILGenerator(ILGenerator ILGenerator, string Name = "<Unknown>")
		//{
		//	return new AstLabel((ILGenerator != null) ? ILGenerator.DefineLabel() : default(Label), Name);
		//}

		public override string ToString()
		{
			return String.Format("AstLabel({0})", Name);
		}
	}
}
