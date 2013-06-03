using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public abstract class AstNodeExprCall : AstNodeExpr
	{
		public bool IsTail { get {
			if (Parent == null) return false;
			//return Parent.GetType() == typeof(AstNodeExprCallTail);
			return Parent is AstNodeExprCallTail;
		} }
		public MethodInfo MethodInfo;
		public AstNodeExpr[] Parameters;

		public AstNodeExprCall(MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
		{
			if (MethodInfo == null) throw (new Exception("MethodInfo can't be null"));

			var MethodParameters = MethodInfo.GetParameters().Select(Parameter => Parameter.ParameterType).ToArray();
			var ParametersTypes = Parameters.Select(Parameter => Parameter.Type).ToArray();

			if (!MethodParameters.SequenceEqual(ParametersTypes))
			{
				throw (new Exception(String.Format(
					"Parameters mismatch ({0}) != ({1})",
					String.Join(",", (IEnumerable<Type>)MethodParameters),
					String.Join(",", (IEnumerable<Type>)ParametersTypes)
				)));
			}

			this.MethodInfo = MethodInfo;
			this.Parameters = Parameters;
		}

		protected override Type UncachedType
		{
			get { return MethodInfo.ReturnType; }
		}

		public override void TransformNodes(TransformNodesDelegate Transformer)
		{
			Transformer.Ref(ref Parameters);
		}
	}
}
