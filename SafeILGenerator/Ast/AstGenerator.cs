using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstGenerator
	{
		protected AstGenerator()
		{
		}

		static public readonly AstGenerator Instance = new AstGenerator();

		public AstNodeStmComment Comment(string comment)
		{
			return new AstNodeStmComment(comment);
		}

		public AstNodeExprArgument Argument(Type type, int index, string name = null)
		{
			return new AstNodeExprArgument(new AstArgument(index, type, name));
		}

		public AstNodeExprArgument Argument<T>(int index, string name = null)
		{
			return Argument(typeof(T), index, name);
		}

		public AstNodeExprArgument Argument(AstArgument astArgument)
		{
			return new AstNodeExprArgument(astArgument);
		}

		public AstNodeExprLocal Local(AstLocal astLocal)
		{
			return new AstNodeExprLocal(astLocal);
		}

		public AstNodeStmGoto GotoAlways(AstLabel astLabel)
		{
			return new AstNodeStmGotoAlways(astLabel);
		}

		public AstNodeStmGotoIfTrue GotoIfTrue(AstLabel astLabel, AstNodeExpr condition)
		{
			return new AstNodeStmGotoIfTrue(astLabel, condition);
		}

		public AstNodeStmGotoIfFalse GotoIfFalse(AstLabel astLabel, AstNodeExpr condition)
		{
			return new AstNodeStmGotoIfFalse(astLabel, condition);
		}

		public AstNodeStmLabel Label(AstLabel astLabel)
		{
			return new AstNodeStmLabel(astLabel);
		}

		public AstNodeExprFieldAccess FieldAccess(AstNodeExpr instance, FieldInfo fieldInfo, string fieldName = "")
		{
			return new AstNodeExprFieldAccess(instance, fieldInfo, fieldName);
		}

		public AstNodeExprFieldAccess FieldAccess(AstNodeExpr instance, string fieldName)
		{
			return new AstNodeExprFieldAccess(instance, fieldName);
		}

		public AstNodeExprLValue FieldPropertyAccess(AstNodeExpr instance, string fieldPropertyName)
		{
			if (instance.Type.GetField(fieldPropertyName) != null) return FieldAccess(instance, fieldPropertyName);
			if (instance.Type.GetProperty(fieldPropertyName) != null) return PropertyAccess(instance, fieldPropertyName);
			throw (new InvalidOperationException(String.Format("Can't find Field/Property '{0}' for type '{1}'", fieldPropertyName, instance.Type)));
		}

		public AstNodeExprPropertyAccess PropertyAccess(AstNodeExpr instance, PropertyInfo propertyInfo)
		{
			return new AstNodeExprPropertyAccess(instance, propertyInfo);
		}

		public AstNodeExprPropertyAccess PropertyAccess(AstNodeExpr instance, string propertyName)
		{
			return new AstNodeExprPropertyAccess(instance, propertyName);
		}

		public AstNodeExprArrayAccess ArrayAccess(AstNodeExpr instance, AstNodeExpr index)
		{
			return new AstNodeExprArrayAccess(instance, index);
		}

		public AstNodeExprImm Immediate<TType>(TType value)
		{
			return new AstNodeExprImm(value, typeof(TType));
		}

		public AstNodeExprImm Immediate(object value)
		{
			return new AstNodeExprImm(value);
		}

		public AstNodeExprNull Null<TType>()
		{
			return new AstNodeExprNull(typeof(TType));
		}

		//public AstNodeExprStaticFieldAccess ImmediateObject<TType>(ILInstanceHolderPool<TType> Pool, TType Value)
		//{
		//	return new AstNodeExprImm(Value);
		//}

		public AstNodeExprCallTailCall TailCall(AstNodeExprCall call)
		{
			return new AstNodeExprCallTailCall(call);
		}

		public AstNodeExprCallStatic CallStatic(Delegate Delegate, params AstNodeExpr[] parameters)
		{
			return new AstNodeExprCallStatic(Delegate, parameters);
		}

		public AstNodeExprCallStatic CallStatic(MethodInfo methodInfo, params AstNodeExpr[] parameters)
		{
			return new AstNodeExprCallStatic(methodInfo, parameters);
		}

		public AstNodeExprCallInstance CallInstance(AstNodeExpr instance, Delegate Delegate, params AstNodeExpr[] parameters)
		{
			return new AstNodeExprCallInstance(instance, Delegate, parameters);
		}

		public AstNodeExprCallDelegate CallDelegate(AstNodeExpr instance, params AstNodeExpr[] parameters)
		{
			return new AstNodeExprCallDelegate(instance, parameters);
		}

		public AstNodeExprCallInstance CallInstance(AstNodeExpr instance, MethodInfo methodInfo, params AstNodeExpr[] parameters)
		{
			return new AstNodeExprCallInstance(instance, methodInfo, parameters);
		}

		public AstNodeExprUnop Unary(string Operator, AstNodeExpr right)
		{
			return new AstNodeExprUnop(Operator, right);
		}

		public AstNodeExprBinop Binary(AstNodeExpr left, string op, AstNodeExpr right)
		{
			return new AstNodeExprBinop(left, op, right);
		}

		public AstNodeExprTerop Ternary(AstNodeExpr condition, AstNodeExpr True, AstNodeExpr False)
		{
			return new AstNodeExprTerop(condition, True, False);
		}

		public AstNodeStmIfElse If(AstNodeExpr condition, AstNodeStm True)
		{
			return new AstNodeStmIfElse(condition, True);
		}

		public AstNodeStmIfElse IfElse(AstNodeExpr condition, AstNodeStm True, AstNodeStm False)
		{
			return new AstNodeStmIfElse(condition, True, False);
		}

		public AstNodeStmContainer Statements(IEnumerable<AstNodeStm> statements)
		{
			return new AstNodeStmContainer(statements.ToArray());
		}

		public AstNodeStmContainer Statements(params AstNodeStm[] statements)
		{
			return new AstNodeStmContainer(statements);
		}

		public AstNodeStmContainer StatementsInline(IEnumerable<AstNodeStm> statements)
		{
			return new AstNodeStmContainer(true, statements.ToArray());
		}

		public AstNodeStmContainer StatementsInline(params AstNodeStm[] statements)
		{
			return new AstNodeStmContainer(true, statements);
		}

		public AstNodeStmEmpty Statement()
		{
			return new AstNodeStmEmpty();
		}

		public AstNodeStmExpr Statement(AstNodeExpr expr)
		{
			return new AstNodeStmExpr(expr);
		}

		public AstNodeStmReturn Return(AstNodeExpr expr = null)
		{
			return new AstNodeStmReturn(expr);
		}

		public AstNodeStmAssign Assign(AstNodeExprLValue left, AstNodeExpr expr)
		{
			return new AstNodeStmAssign(left, expr);
		}

		public AstNodeExprCast Cast(Type type, AstNodeExpr expr, bool Explicit = true)
		{
			return new AstNodeExprCast(type, expr, Explicit);
		}

		public AstNodeExprCast Cast<T>(AstNodeExpr expr, bool Explicit = true)
		{
			return Cast(typeof(T), expr, Explicit);
		}

		public AstNodeExprGetAddress GetAddress(AstNodeExprLValue expr)
		{
			return new AstNodeExprGetAddress(expr);
		}

		public AstNodeExprLValue Reinterpret<TType>(AstNodeExprLValue value)
		{
			return Reinterpret(typeof(TType), value);
		}

		public AstNodeExprLValue Reinterpret(Type type, AstNodeExprLValue value)
		{
			return Indirect(Cast(type.MakePointerType(), GetAddress(value), Explicit: false));
		}

		public AstNodeExprIndirect Indirect(AstNodeExpr pointerExpr)
		{
			return new AstNodeExprIndirect(pointerExpr);
		}

		public AstNodeStm DebugWrite(string text)
		{
			return Statement(CallStatic((Action<string>)Console.WriteLine, text));
		}

		public AstNodeExprLValue StaticFieldAccess(FieldInfo fieldInfo)
		{
			return new AstNodeExprStaticFieldAccess(fieldInfo);
		}

		public AstNodeExprLValue StaticFieldAccess<T>(Expression<Func<T>> expression)
		{
			return StaticFieldAccess(_fieldof(expression));
		}

		private static FieldInfo _fieldof<T>(Expression<Func<T>> expression)
		{
			MemberExpression body = (MemberExpression)expression.Body;
			return (FieldInfo)body.Member;
		}

		public AstNodeCase Case(object value, AstNodeStm code)
		{
			return new AstNodeCase(value, code);
		}

		public AstNodeCaseDefault Default(AstNodeStm code)
		{
			return new AstNodeCaseDefault(code);
		}

		public AstNodeStmSwitch Switch(AstNodeExpr valueToCheck, params AstNodeCase[] cases)
		{
			return new AstNodeStmSwitch(valueToCheck, cases);
		}

		public AstNodeStmSwitch Switch(AstNodeExpr valueToCheck, AstNodeCaseDefault Default, params AstNodeCase[] cases)
		{
			return new AstNodeStmSwitch(valueToCheck, cases, Default);
		}

		public AstNodeExprNewArray NewArray(Type type, params AstNodeExpr[] elements)
		{
			return new AstNodeExprNewArray(type, elements);
		}

		public AstNodeExprNewArray NewArray<TType>(params AstNodeExpr[] elements)
		{
			return NewArray(typeof(TType), elements);
		}

		public AstNodeExprNew New(Type type, params AstNodeExpr[] Params)
		{
			return new AstNodeExprNew(type, Params);
		}

		public AstNodeExprNew New<TType>(params AstNodeExpr[] Params)
		{
			return New(typeof(TType), Params);
		}

		public AstNodeExprSetGetLValuePlaceholder SetGetLValuePlaceholder<TType>()
		{
			return SetGetLValuePlaceholder(typeof(TType));
		}

		public AstNodeExprSetGetLValuePlaceholder SetGetLValuePlaceholder(Type type)
		{
			return new AstNodeExprSetGetLValuePlaceholder(type);
		}

		public AstNodeExprSetGetLValue SetGetLValue(AstNodeExpr setExpression, AstNodeExpr getExpression)
		{
			return new AstNodeExprSetGetLValue(setExpression, getExpression);
		}

		public AstNodeStm Throw(AstNodeExpr expression)
		{
			return new AstNodeStmThrow(expression);
		}
	}
}
