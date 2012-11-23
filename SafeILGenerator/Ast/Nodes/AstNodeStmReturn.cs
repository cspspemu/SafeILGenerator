using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeStmReturn : AstNodeStm
	{
		public AstNodeExpr Expression;

		public AstNodeStmReturn(AstNodeExpr Expression = null)
		{
			this.Expression = Expression;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Expression);
		}
	}
}
