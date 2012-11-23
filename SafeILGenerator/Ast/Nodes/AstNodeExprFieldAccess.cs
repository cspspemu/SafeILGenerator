using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprFieldAccess : AstNodeExprLValue
	{
		public AstNodeExpr Object;
		public FieldInfo Field;

		public AstNodeExprFieldAccess(AstNodeExpr Object, string FieldName)
			: this(Object, Object.Type.GetField(FieldName))
		{
		}

		public AstNodeExprFieldAccess(AstNodeExpr Object, FieldInfo Field)
		{
			this.Object = Object;
			this.Field = Field;
		}

		protected override Type UncachedType
		{
			get { return Field.FieldType; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Object);
		}
	}
}
