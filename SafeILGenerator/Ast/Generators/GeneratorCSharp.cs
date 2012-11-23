using Codegen.Ast.Generators;
using Codegen.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Generators
{
	public class GeneratorCSharp : Generator
	{
		protected StringBuilder Output = new StringBuilder();

		protected void Generate(AstNodeExprImm Item)
		{
			string StringValue = Item.Value.ToString();

			if (Item.Value is bool)
			{
				StringValue = StringValue.ToLower();
			}
			Output.Append(StringValue);
		}

		protected void Generate(AstNodeExprBinop Item)
		{
			Output.Append("(");
			this.Generate(Item.LeftNode);
			Output.Append(" " + Item.Operator + " ");
			this.Generate(Item.RightNode);
			Output.Append(")");
		}

		protected void Generate(AstNodeStmIfElse Item)
		{
			Output.Append("if (");
			this.Generate(Item.Condition);
			Output.Append(") ");
			this.Generate(Item.True);
			if (Item.False != null)
			{
				Output.Append(" else ");
				this.Generate(Item.False);
			}
		}

		protected void Generate(AstNodeStmReturn Item)
		{
			Output.Append("return");
			if (Item.Expression != null)
			{
				Output.Append(" ");
				this.Generate(Item.Expression);
			}
			Output.Append(";");
		}

		protected void Generate(AstNodeExprCallStatic Item)
		{
			Output.Append(Item.MethodInfo.DeclaringType.Name + "." + Item.MethodInfo.Name);
			Output.Append("(");
			for (int n = 0; n < Item.Parameters.Length; n++)
			{
				if (n != 0) Output.Append(", ");
				Generate(Item.Parameters[n]);
			}
			Output.Append(")");
		}

		public override string ToString()
		{
			return Output.ToString();
		}
	}
}
