using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Optimizers
{
	public class AstOptimizer
	{
		private Dictionary<Type, MethodInfo> GenerateMappings = new Dictionary<Type, MethodInfo>();

		public AstOptimizer()
		{
			foreach (
				var Method
				in
				this.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(Method => Method.ReturnType == typeof(AstNode))
					.Where(Method => Method.GetParameters().Count() == 1)
			)
			{
				GenerateMappings[Method.GetParameters().First().ParameterType] = Method;
			}
		}

		public AstNode Optimize(AstNode AstNode)
		{
			//if (AstNode != null)
			{
				//Console.WriteLine("Optimize.AstNode: {0}", AstNode);
				AstNode.TransformNodes(Optimize);

				var AstNodeType = AstNode.GetType();

				if (GenerateMappings.ContainsKey(AstNodeType))
				{
					AstNode = (AstNode)GenerateMappings[AstNodeType].Invoke(this, new[] { AstNode });
				}
				else
				{
					//throw(new NotImplementedException(String.Format("Don't know how to optimize {0}", AstNodeType)));
				}
			}
			
			return AstNode;
		}

		protected AstNode _Optimize(AstNodeExprCast Cast)
		{
			//Console.WriteLine("Optimize.AstNodeExprCast: {0} : {1}", Cast.CastedType, Cast.Expr);

			// Dummy cast
			if (Cast.CastedType == Cast.Expr.Type)
			{
				//Console.WriteLine("Dummy Cast");
				return Cast.Expr;
			}
			// Double Cast
			else if (Cast.Expr is AstNodeExprCast)
			{
				//Console.WriteLine("Double Cast");
				var FirstCastType = (Cast.Expr as AstNodeExprCast).CastedType;
				var SecondCastType = Cast.CastedType;
				if (FirstCastType.IsPrimitive && SecondCastType.IsPrimitive)
				{
					if (AstUtils.GetTypeSize(FirstCastType) >= AstUtils.GetTypeSize(SecondCastType))
					{
						return Optimize(new AstNodeExprCast(Cast.CastedType, (Cast.Expr as AstNodeExprCast).Expr));
					}
				}
			}
			// Cast to immediate
			else if (Cast.Expr is AstNodeExprImm)
			{
				//Console.WriteLine("Cast to immediate");
				return new AstNodeExprImm(AstUtils.CastType((Cast.Expr as AstNodeExprImm).Value, Cast.CastedType));
			}

			return Cast;
		}

		protected AstNode _Optimize(AstNodeExprImm Immediate)
		{
			return Immediate;
		}

		protected AstNode _Optimize(AstNodeExprBinop Binary)
		{
			//Console.WriteLine("Optimize.AstNodeExprBinop: {0} {1} {2}", Binary.LeftNode, Binary.Operator, Binary.RightNode);
			var LeftImm = (Binary.LeftNode as AstNodeExprImm);
			var RightImm = (Binary.RightNode as AstNodeExprImm);
			var LeftType = Binary.LeftNode.Type;
			var RightType = Binary.RightNode.Type;
			var Operator = Binary.Operator;

			if ((LeftType == RightType) && !AstUtils.IsTypeFloat(LeftType))
			{
				var Type = LeftType;
				// Can optimize just literal values.
				if ((LeftImm != null) && (RightImm != null))
				{
					if (AstUtils.IsTypeSigned(LeftType))
					{
						var LeftValue = Convert.ToInt64(LeftImm.Value);
						var RightValue = Convert.ToInt64(RightImm.Value);

						// Optimize adding 0
						switch (Operator)
						{
							case "+": return new AstNodeExprImm(AstUtils.CastType(LeftValue + RightValue, Type));
							case "-": return new AstNodeExprImm(AstUtils.CastType(LeftValue - RightValue, Type));
							case "*": return new AstNodeExprImm(AstUtils.CastType(LeftValue * RightValue, Type));
							case "/": return new AstNodeExprImm(AstUtils.CastType(LeftValue / RightValue, Type));
							case "<<": return new AstNodeExprImm(AstUtils.CastType(LeftValue << (int)RightValue, Type));
							case ">>": return new AstNodeExprImm(AstUtils.CastType(LeftValue >> (int)RightValue, Type));
						}
					}
					else
					{
						var LeftValue = Convert.ToUInt64(LeftImm.Value);
						var RightValue = Convert.ToUInt64(RightImm.Value);

						// Optimize adding 0
						switch (Operator)
						{
							case "+": return new AstNodeExprImm(AstUtils.CastType(LeftValue + RightValue, Type));
							case "-": return new AstNodeExprImm(AstUtils.CastType(LeftValue - RightValue, Type));
							case "*": return new AstNodeExprImm(AstUtils.CastType(LeftValue * RightValue, Type));
							case "/": return new AstNodeExprImm(AstUtils.CastType(LeftValue / RightValue, Type));
							case "<<": return new AstNodeExprImm(AstUtils.CastType(LeftValue << (int)RightValue, Type));
							case ">>": return new AstNodeExprImm(AstUtils.CastType(LeftValue >> (int)RightValue, Type));
						}
					}
				}
				else if (LeftImm != null)
				{
					var LeftValue = Convert.ToInt64(LeftImm.Value);
					switch (Operator)
					{
						case "+": if (LeftValue == 0) return Binary.RightNode; break;
						case "-": if (LeftValue == 0) return new AstNodeExprUnop("-", Binary.RightNode); break;
						case "*":
							//if (LeftValue == 0) return new AstNodeExprImm(AstUtils.CastType(0, Type));
							if (LeftValue == 1) return Binary.RightNode;
							break;
						case "/":
							//if (LeftValue == 0) return new AstNodeExprImm(AstUtils.CastType(0, Type));
							break;
					}
				}
				else if (RightImm != null)
				{
					var RightValue = Convert.ToInt64(RightImm.Value);
					switch (Operator)
					{
						case "+": if (RightValue == 0) return Binary.LeftNode; break;
						case "-": if (RightValue == 0) return Binary.LeftNode; break;
						case "*":
							if (RightValue == 1) return Binary.LeftNode;
							break;
						case "/":
							//if (RightValue == 0) throw(new Exception("Can't divide by 0"));
							if (RightValue == 1) return Binary.LeftNode;
							break;
					}
				}
			}

			return Binary;
		}
	}
}
