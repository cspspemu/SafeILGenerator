using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeCase : AstNode
	{
		public object[] CaseValues;
		public AstNodeStm Code;

		public AstNodeCase(object[] Values, AstNodeStm Code)
		{
			this.CaseValues = Values;
			this.Code = Code;
		}

		public AstNodeCase(object Value, AstNodeStm Code)
			: this(new[] { Value}, Code)
		{
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Code);
		}
	}
}
