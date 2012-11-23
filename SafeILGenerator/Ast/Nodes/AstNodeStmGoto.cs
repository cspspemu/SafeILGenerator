using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeStmGoto : AstNodeStm
	{
		public readonly AstLabel AstLabel;

		public AstNodeStmGoto(AstLabel AstLabel)
		{
			this.AstLabel = AstLabel;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
		}
	}
}
