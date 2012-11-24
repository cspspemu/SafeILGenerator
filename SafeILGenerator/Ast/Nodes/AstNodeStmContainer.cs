using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeStmContainer : AstNodeStm
	{
		public AstNodeStm[] Nodes;

		public AstNodeStmContainer(params AstNodeStm[] Nodes)
		{
			this.Nodes = Nodes;
		}

		public AstNodeStmContainer(IEnumerable<AstNodeStm> Nodes)
		{
			this.Nodes = Nodes.ToArray();
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Nodes);
		}
	}
}
