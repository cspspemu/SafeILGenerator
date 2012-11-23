using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeExprFieldAccess : AstNodeExprLValue
	{
		public AstNodeExpr Instance;
		public FieldInfo Field;

		public AstNodeExprFieldAccess(AstNodeExpr Instance, string FieldName)
			: this(Instance, Instance.Type.GetField(FieldName))
		{
		}

		public AstNodeExprFieldAccess(AstNodeExpr Instance, FieldInfo Field)
		{
			this.Instance = Instance;
			this.Field = Field;
		}

		protected override Type UncachedType
		{
			get { return Field.FieldType; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Instance);
		}
	}
}
