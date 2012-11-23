using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeStmIfElse : AstNodeStm
	{
		public AstNodeExpr Condition;
		public AstNodeStm True;
		public AstNodeStm False;

		public AstNodeStmIfElse(AstNodeExpr Condition, AstNodeStm True, AstNodeStm False = null)
		{
			this.Condition = Condition;
			this.True = True;
			this.False  = False;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Condition);
			Transformer.Ref(ref True);
			Transformer.Ref(ref False);
		}
	}
}
