using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstGenerator
	{
		private AstGenerator()
		{
		}

		static public AstGenerator Instance = new AstGenerator();

		public AstNodeStmComment Comment(string Comment)
		{
			return new AstNodeStmComment(Comment);
		}

		public AstNodeExprArgument Argument(Type Type, int Index, string Name = null)
		{
			return new AstNodeExprArgument(new AstArgument(Index, Type, Name));
		}

		public AstNodeExprArgument Argument<T>(int Index, string Name = null)
		{
			return Argument(typeof(T), Index, Name);
		}

		public AstNodeExprArgument Argument(AstArgument AstArgument)
		{
			return new AstNodeExprArgument(AstArgument);
		}

		public AstNodeExprLocal Local(AstLocal AstLocal)
		{
			return new AstNodeExprLocal(AstLocal);
		}

		public AstNodeStmGoto GotoAlways(AstLabel AstLabel)
		{
			return new AstNodeStmGotoAlways(AstLabel);
		}

		public AstNodeStmGotoIfTrue GotoIfTrue(AstLabel AstLabel, AstNodeExpr Condition)
		{
			return new AstNodeStmGotoIfTrue(AstLabel, Condition);
		}

		public AstNodeStmGotoIfFalse GotoIfFalse(AstLabel AstLabel, AstNodeExpr Condition)
		{
			return new AstNodeStmGotoIfFalse(AstLabel, Condition);
		}

		public AstNodeStmLabel Label(AstLabel AstLabel)
		{
			return new AstNodeStmLabel(AstLabel);
		}

		public AstNodeExprFieldAccess FieldAccess(AstNodeExpr Instance, FieldInfo FieldInfo)
		{
			return new AstNodeExprFieldAccess(Instance, FieldInfo);
		}

		public AstNodeExprFieldAccess FieldAccess(AstNodeExpr Instance, string FieldName)
		{
			return new AstNodeExprFieldAccess(Instance, FieldName);
		}

		public AstNodeExprImm Immediate(object Value)
		{
			return new AstNodeExprImm(Value);
		}

		public AstNodeExprCallTail CallTail(AstNodeExprCall Call)
		{
			return new AstNodeExprCallTail(Call);
		}

		public AstNodeExprCallStatic CallStatic(Delegate Delegate, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallStatic(Delegate, Parameters);
		}

		public AstNodeExprCallStatic CallStatic(MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallStatic(MethodInfo, Parameters);
		}

		public AstNodeExprCallInstance CallInstance(AstNodeExpr Instance, Delegate Delegate, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallInstance(Instance, Delegate, Parameters);
		}

		public AstNodeExprCallInstance CallInstance(AstNodeExpr Instance, MethodInfo MethodInfo, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallInstance(Instance, MethodInfo, Parameters);
		}

		public AstNodeExprUnop Unary(string Operator, AstNodeExpr Right)
		{
			return new AstNodeExprUnop(Operator, Right);
		}

		public AstNodeExprBinop Binary(AstNodeExpr Left, string Operator, AstNodeExpr Right)
		{
			return new AstNodeExprBinop(Left, Operator, Right);
		}

		public AstNodeExprTerop Ternary(AstNodeExpr Condition, AstNodeExpr True, AstNodeExpr False)
		{
			return new AstNodeExprTerop(Condition, True, False);
		}

		public AstNodeStmIfElse IfElse(AstNodeExpr Condition, AstNodeStm True, AstNodeStm False = null)
		{
			return new AstNodeStmIfElse(Condition, True, False);
		}

		public AstNodeStmContainer Statements(params AstNodeStm[] Statements)
		{
			return new AstNodeStmContainer(Statements);
		}

		public AstNodeStmEmpty Statement()
		{
			return new AstNodeStmEmpty();
		}

		public AstNodeStmExpr Statement(AstNodeExpr Expr)
		{
			return new AstNodeStmExpr(Expr);
		}

		public AstNodeStmReturn Return(AstNodeExpr Expr = null)
		{
			return new AstNodeStmReturn(Expr);
		}

		public AstNodeStmAssign Assign(AstNodeExprLValue Left, AstNodeExpr Expr)
		{
			return new AstNodeStmAssign(Left, Expr);
		}

		public AstNodeExprCast Cast(Type Type, AstNodeExpr Expr, bool Explicit = true)
		{
			return new AstNodeExprCast(Type, Expr, Explicit);
		}

		public AstNodeExprCast Cast<T>(AstNodeExpr Expr, bool Explicit = true)
		{
			return Cast(typeof(T), Expr, Explicit);
		}

		public AstNodeExprGetAddress GetAddress(AstNodeExprLValue Expr)
		{
			return new AstNodeExprGetAddress(Expr);
		}

		public AstNodeExprIndirect Indirect(AstNodeExpr PointerExpr)
		{
			return new AstNodeExprIndirect(PointerExpr);
		}

	}
}
