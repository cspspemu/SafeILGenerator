using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeExprImm : AstNodeExpr
	{
		public object Value;

		public AstNodeExprImm(object Value)
		{
			this.Value = Value;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
		}

		protected override Type UncachedType
		{
			get { return this.Value.GetType(); }
		}
	}
}
