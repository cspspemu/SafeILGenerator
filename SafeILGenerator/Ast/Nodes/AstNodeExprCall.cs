using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public abstract class AstNodeExprCall : AstNodeExpr
	{
		public MethodInfo MethodInfo;
		public AstNodeExpr[] Parameters;

		public AstNodeExprCall(MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
		{
			var MethodParameters = MethodInfo.GetParameters().Select(Parameter => Parameter.ParameterType).ToArray();
			var ParametersTypes = Parameters.Select(Parameter => Parameter.Type).ToArray();

			if (!MethodParameters.SequenceEqual(ParametersTypes)) throw(new Exception("Parameters mismatch"));

			this.MethodInfo = MethodInfo;
			this.Parameters = Parameters;
		}

		protected override Type UncachedType
		{
			get { return MethodInfo.ReturnType; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			for (int n = 0; n < Parameters.Length; n++)
			{
				Transformer.Ref(ref Parameters[n]);
			}
		}
	}
}
