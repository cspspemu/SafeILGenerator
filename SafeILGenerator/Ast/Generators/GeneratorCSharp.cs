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
			Output.Append(Item.Value.ToString());
		}

		protected void Generate(AstNodeExprBinop Item)
		{
			Output.Append("(");
			this.Generate(Item.LeftNode);
			Output.Append(" " + Item.Operator + " ");
			this.Generate(Item.RightNode);
			Output.Append(")");
		}

		public override string ToString()
		{
			return Output.ToString();
		}
	}
}
