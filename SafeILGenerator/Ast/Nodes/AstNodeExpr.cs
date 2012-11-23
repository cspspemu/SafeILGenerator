using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Nodes
{
	public abstract class AstNodeExpr : AstNode
	{
		public Type Type { get { return UncachedType; } }
		protected abstract Type UncachedType { get; }

		public static AstNodeExprBinop operator +(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "+", Right); }
		public static AstNodeExprBinop operator -(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "-", Right); }
		public static AstNodeExprBinop operator *(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "*", Right); }
		public static AstNodeExprBinop operator /(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "/", Right); }
		public static AstNodeExprBinop operator %(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "%", Right); }

		public static AstNodeExprBinop operator &(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "&", Right); }
		public static AstNodeExprBinop operator |(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "|", Right); }
		public static AstNodeExprBinop operator ^(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "^", Right); }

		//public static AstNodeExprBinop operator <<(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "<<", Right); }
		//public static AstNodeExprBinop operator >>(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, ">>", Right); }

		public static AstNodeExprUnop operator -(AstNodeExpr Right) { return new AstNodeExprUnop("-", Right); }
		public static AstNodeExprUnop operator ~(AstNodeExpr Right) { return new AstNodeExprUnop("~", Right); }

		public static implicit operator AstNodeExpr(int Value) { return new AstNodeExprImm(Value); }

		//public static AstNodeExprBinop operator ==(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "==", Right); }
		//public static AstNodeExprBinop operator !=(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "!=", Right); }
		//
		//public static AstNodeExprBinop operator >(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, ">", Right); }
		//public static AstNodeExprBinop operator <(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "<", Right); }
		//public static AstNodeExprBinop operator >=(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, ">=", Right); }
		//public static AstNodeExprBinop operator <=(AstNodeExpr Left, AstNodeExpr Right) { return new AstNodeExprBinop(Left, "<=", Right); }
	}
}
