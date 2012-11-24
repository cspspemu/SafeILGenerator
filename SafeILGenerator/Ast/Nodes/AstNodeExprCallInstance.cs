using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public class AstNodeExprCallInstance : AstNodeExprCall
	{
		public AstNodeExpr Instance;

		public AstNodeExprCallInstance(AstNodeExpr Object, Delegate Delegate, params AstNodeExpr[] Parameters)
			: this(Object, Delegate.Method, Parameters)
		{
			
		}

		public AstNodeExprCallInstance(AstNodeExpr Object, MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
			: base(MethodInfo, Parameters)
		{
			this.Instance = Object;
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Instance);
			base.TransformNodes(Transformer);
		}
	}
}
