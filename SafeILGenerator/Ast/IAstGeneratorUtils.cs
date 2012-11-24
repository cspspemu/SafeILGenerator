using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	static public class IAstGeneratorUtils
	{
		static public AstNodeExprArgument Argument<T>(this IAstGenerator IAstGenerator, int Index, string Name = null)
		{
			return new AstNodeExprArgument(new AstArgument(Index, typeof(T), Name));
		}

		static public AstNodeExprArgument Argument(this IAstGenerator IAstGenerator, AstArgument AstArgument)
		{
			return new AstNodeExprArgument(AstArgument);
		}

		static public AstNodeExprLocal Local(this IAstGenerator IAstGenerator, AstLocal AstLocal)
		{
			return new AstNodeExprLocal(AstLocal);
		}

		static public AstNodeStmGoto Goto(this IAstGenerator IAstGenerator, AstLabel AstLabel)
		{
			return new AstNodeStmGoto(AstLabel);
		}

		static public AstNodeStmLabel Label(this IAstGenerator IAstGenerator, AstLabel AstLabel)
		{
			return new AstNodeStmLabel(AstLabel);
		}

		static public AstNodeExprFieldAccess FieldAccess(this IAstGenerator IAstGenerator, AstNodeExpr Instance, FieldInfo FieldInfo)
		{
			return new AstNodeExprFieldAccess(Instance, FieldInfo);
		}

		static public AstNodeExprFieldAccess FieldAccess(this IAstGenerator IAstGenerator, AstNodeExpr Instance, string FieldName)
		{
			return new AstNodeExprFieldAccess(Instance, FieldName);
		}

		static public AstNodeExprImm Immediate(this IAstGenerator IAstGenerator, object Value)
		{
			return new AstNodeExprImm(Value);
		}

		static public AstNodeExprCallStatic CallStatic(this IAstGenerator IAstGenerator, Delegate Delegate, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallStatic(Delegate, Parameters);
		}

		static public AstNodeExprCallInstance CallInstance(this IAstGenerator IAstGenerator, AstNodeExpr Instance, Delegate Delegate, params AstNodeExpr[] Parameters)
		{
			return new AstNodeExprCallInstance(Instance, Delegate, Parameters);
		}

		static public AstNodeExprUnop Unary(this IAstGenerator IAstGenerator, string Operator, AstNodeExpr Right)
		{
			return new AstNodeExprUnop(Operator, Right);
		}

		static public AstNodeExprBinop Binary(this IAstGenerator IAstGenerator, AstNodeExpr Left, string Operator, AstNodeExpr Right)
		{
			return new AstNodeExprBinop(Left, Operator, Right);
		}

		static public AstNodeExprTerop Ternary(this IAstGenerator IAstGenerator, AstNodeExpr Condition, AstNodeExpr True, AstNodeExpr False)
		{
			return new AstNodeExprTerop(Condition, True, False);
		}

		static public AstNodeStmIfElse IfElse(this IAstGenerator IAstGenerator, AstNodeExpr Condition, AstNodeStm True, AstNodeStm False = null)
		{
			return new AstNodeStmIfElse(Condition, True, False);
		}

		static public AstNodeStmContainer Statements(this IAstGenerator IAstGenerator, params AstNodeStm[] Statements)
		{
			return new AstNodeStmContainer(Statements);
		}

		static public AstNodeStmExpr Statement(this IAstGenerator IAstGenerator, AstNodeExpr Expr)
		{
			return new AstNodeStmExpr(Expr);
		}

		static public AstNodeStmReturn Return(this IAstGenerator IAstGenerator, AstNodeExpr Expr = null)
		{
			return new AstNodeStmReturn(Expr);
		}

		static public AstNodeStmAssign Assign(this IAstGenerator IAstGenerator, AstNodeExprLValue Left, AstNodeExpr Expr)
		{
			return new AstNodeStmAssign(Left, Expr);
		}

		static public AstNodeExprCast Cast(this IAstGenerator IAstGenerator, Type Type, AstNodeExpr Expr, bool Explicit = true)
		{
			return new AstNodeExprCast(Type, Expr, Explicit);
		}

		static public AstNodeExprCast Cast<T>(this IAstGenerator IAstGenerator, AstNodeExpr Expr, bool Explicit = true)
		{
			return IAstGenerator.Cast(typeof(T), Expr, Explicit);
		}

		static public AstNodeExprGetAddress GetAddress(this IAstGenerator IAstGenerator, AstNodeExprLValue Expr)
		{
			return new AstNodeExprGetAddress(Expr);
		}

		static public AstNodeExprIndirect Indirect(this IAstGenerator IAstGenerator, AstNodeExpr PointerExpr)
		{
			return new AstNodeExprIndirect(PointerExpr);
		}
	}
}
