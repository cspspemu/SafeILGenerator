using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public delegate AstNode TransformNodesDelegate(AstNode AstNode);

	static public class TransformNodesDelegateExtensions
	{
		static public void Ref<T>(this TransformNodesDelegate Transformer, ref T Node) where T : AstNode
		{
			Node = (T)Transformer(Node);
		}
	}

	public abstract class AstNode
	{
		public abstract void TransformNodes(TransformNodesDelegate Transformer);
	}
}
