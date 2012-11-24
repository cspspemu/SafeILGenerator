using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
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
			if (LeftNode.Type != RightNode.Type) throw (new Exception(String.Format("Left.Type({0}) != Right.Type({1}) Operator: {2}", LeftNode.Type, RightNode.Type, Operator)));
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref LeftNode);
			Transformer.Ref(ref RightNode);
		}

		public Type CommonType
		{
			get
			{
				return LeftNode.Type;
			}
		}

		protected override Type UncachedType
		{
			get
			{
				return CommonType;
			}
		}
	}
}
