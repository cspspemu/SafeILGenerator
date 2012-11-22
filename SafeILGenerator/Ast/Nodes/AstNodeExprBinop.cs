using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprBinop : AstNodeExpr
	{
		public AstNodeExpr LeftNode;
		public string Operator;
		public AstNodeExpr RightNode;

		public AstNodeExprBinop(AstNodeExpr LeftNode, string Operator, AstNodeExpr RightNode)
		{
			this.LeftNode = LeftNode;
			this.Operator = Operator;
			this.RightNode = RightNode;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			LeftNode = (AstNodeExpr)Transformer(LeftNode);
			RightNode = (AstNodeExpr)Transformer(RightNode);
		}

		protected override Type UncachedType
		{
			get
			{
				if (LeftNode.Type != RightNode.Type) throw(new Exception("Left.Type != Right.Type"));
				return LeftNode.Type;
			}
		}
	}
}
