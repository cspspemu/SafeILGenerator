using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeStmSwitch : AstNodeStm
	{
		public AstNodeCase[] Cases;

		public AstNodeStmSwitch(IEnumerable<AstNodeCase> Cases)
		{
			this.Cases = Cases.ToArray();
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Cases);
		}
	}
}
