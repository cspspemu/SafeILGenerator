using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeStmAssignLocal : AstNodeStm
	{
		public AstLocal LeftLocal;
		public AstNodeExpr RightNode;

		public AstNodeStmAssignLocal(AstLocal LeftLocal, AstNodeExpr RightNode)
		{
			this.LeftLocal = LeftLocal;
			this.RightNode = RightNode;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			RightNode = (AstNodeExpr)Transformer(RightNode);
		}
	}
}
