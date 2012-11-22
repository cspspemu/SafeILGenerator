using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public delegate AstNode TransformNodesDelegate(AstNode AstNode);

	public abstract class AstNode
	{
		public abstract void TransformNodes(TransformNodesDelegate Transformer);
	}
}
