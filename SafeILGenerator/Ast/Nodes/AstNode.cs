using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public delegate AstNode TransformNodesDelegate(AstNode AstNode);

	static public class TransformNodesDelegateExtensions
	{
		static public void Ref<T>(this TransformNodesDelegate Transformer, ref T Node) where T : AstNode
		{
			Node = (T)Transformer(Node);
		}

		static public void Ref<T>(this TransformNodesDelegate Transformer, ref T[] Nodes) where T : AstNode
		{
			var NewNodes = new List<T>();
			foreach (var Node in Nodes)
			{
				var NewNode = (T)Transformer(Node);
				if (NewNode != null) NewNodes.Add(NewNode);
			}
			Nodes = NewNodes.ToArray();
			
		}
	}

	public abstract class AstNode
	{
		public abstract void TransformNodes(TransformNodesDelegate Transformer);
	}
}
