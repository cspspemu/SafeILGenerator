using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public class AstNodeExprCallInstance : AstNodeExprCall
	{
		public AstNodeExpr Object;

		public AstNodeExprCallInstance(AstNodeExpr Object, Delegate Delegate, params AstNodeExpr[] Parameters)
			: this(Object, Delegate.Method, Parameters)
		{
			
		}

		public AstNodeExprCallInstance(AstNodeExpr Object, MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
			: base(MethodInfo, Parameters)
		{
			this.Object = Object;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Object);
			base.TransformNodes(Transformer);
		}
	}
}
