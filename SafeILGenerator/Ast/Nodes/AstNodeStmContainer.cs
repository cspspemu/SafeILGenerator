using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeStmContainer : AstNode
	{
		public List<AstNodeStm> Nodes;

		public AstNodeStmContainer(IEnumerable<AstNodeStm> Nodes)
		{
			this.Nodes = Nodes.ToList();
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			var NewNodes = new List<AstNodeStm>();
			foreach (var Node in this.Nodes)
			{
				var NewNode = Transformer(Node);
				if (NewNode != null) NewNodes.Add((AstNodeStm)NewNode);
			}
			this.Nodes = NewNodes;
		}
	}
}
