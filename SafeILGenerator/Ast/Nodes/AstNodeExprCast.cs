using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeExprCast : AstNodeExpr
	{
		public Type CastedType;
		public AstNodeExpr Expr;

		public AstNodeExprCast(Type CastedType, AstNodeExpr Expr)
		{
			this.CastedType = CastedType;
			this.Expr = Expr;
		}

		protected override Type UncachedType
		{
			get { return CastedType; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Expr);
		}
	}
}
