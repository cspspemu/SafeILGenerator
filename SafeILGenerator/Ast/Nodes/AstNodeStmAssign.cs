using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeStmAssign : AstNodeStm
	{
		public AstNodeExprLValue Left;
		public AstNodeExpr Value;

		public AstNodeStmAssign(AstNodeExprLValue Left, AstNodeExpr Value)
		{
			this.Left = Left;
			this.Value = Value;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Left);
			Transformer.Ref(ref Value);
		}
	}
}
