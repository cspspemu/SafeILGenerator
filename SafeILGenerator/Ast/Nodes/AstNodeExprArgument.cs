using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprArgument : AstNodeExprLValue
	{
		public readonly AstArgument AstArgument;

		public AstNodeExprArgument(AstArgument AstArgument)
		{
			this.AstArgument = AstArgument;
		}

		protected override Type UncachedType
		{
			get { return AstArgument.Type; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
		}
	}
}
