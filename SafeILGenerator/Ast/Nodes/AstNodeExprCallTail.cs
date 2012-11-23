using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprCallTail : AstNodeExpr
	{
		AstNodeExprCall Call;

		public AstNodeExprCallTail(AstNodeExprCall Call)
		{
			this.Call = Call;
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
