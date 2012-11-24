using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeExprCallTail : AstNodeExpr
	{
		public AstNodeExprCall Call;

		public AstNodeExprCallTail(AstNodeExprCall Call)
		{
			this.Call = Call;
			Call.Parent = this;
		}

		protected override Type UncachedType
		{
			get { return Call.Type; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Call);
		}
	}
}
