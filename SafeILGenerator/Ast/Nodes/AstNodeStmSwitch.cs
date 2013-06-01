using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeStmSwitch : AstNodeStm
	{
		public AstNodeExpr SwitchValue;
		public AstNodeCase[] Cases;

		public AstNodeStmSwitch(AstNodeExpr SwitchValue, IEnumerable<AstNodeCase> Cases)
		{
			this.SwitchValue = SwitchValue;
			this.Cases = Cases.ToArray();
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref SwitchValue);
			Transformer.Ref(ref Cases);
		}
	}
}
