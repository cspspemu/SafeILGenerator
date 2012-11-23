using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprLocal : AstNodeExprLValue
	{
		public readonly AstLocal AstLocal;

		public AstNodeExprLocal(AstLocal AstLocal)
		{
			this.AstLocal = AstLocal;
		}

		protected override Type UncachedType
		{
			get { return AstLocal.Type; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
		}
	}
}
