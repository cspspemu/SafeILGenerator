//#define DEBUG_GENERATOR_IL
//#define DO_NOT_EMIT

using SafeILGenerator.Ast;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Ast.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SafeILGenerator.Ast.Generators
{
	public class GeneratorIL : Generator<GeneratorIL>, IAstGenerator
	{
		protected MethodInfo MethodInfo;
		protected ILGenerator ILGenerator;

		public GeneratorIL(MethodInfo MethodInfo, ILGenerator ILGenerator)
		{
			this.MethodInfo = MethodInfo;
			this.ILGenerator = ILGenerator;
		}

		private void EmitHook(OpCode OpCode, object Param)
		{
#if DEBUG_GENERATOR_IL || DO_NOT_EMIT
			Console.WriteLine("{0} {1}", OpCode, Param);
#endif
		}

#if DO_NOT_EMIT
		private void Emit(OpCode OpCode) { EmitHook(OpCode, null); }
		private void Emit(OpCode OpCode, object Value) { EmitHook(OpCode, Value); }
#else
		private void Emit(OpCode OpCode) { EmitHook(OpCode, null); ILGenerator.Emit(OpCode); }
		private void Emit(OpCode OpCode, int Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, long Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, float Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, double Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, LocalBuilder Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, MethodInfo Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, FieldInfo Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, Type Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, Label Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
#endif

		protected void _Generate(AstNodeExprImm Item)
		{
			var ItemType = AstUtils.GetSignedType(Item.Type);
			var ItemValue = Item.Value;

			if (
				ItemType == typeof(int)
				|| ItemType == typeof(sbyte)
				|| ItemType == typeof(short)
				|| ItemType == typeof(bool)
			)
			{
				var Value = (int)Convert.ToInt64(ItemValue);
				switch (Value)
				{
					case -1: Emit(OpCodes.Ldc_I4_M1); break;
					case 0: Emit(OpCodes.Ldc_I4_0); break;
					case 1: Emit(OpCodes.Ldc_I4_1); break;
					case 2: Emit(OpCodes.Ldc_I4_2); break;
					case 3: Emit(OpCodes.Ldc_I4_3); break;
					case 4: Emit(OpCodes.Ldc_I4_4); break;
					case 5: Emit(OpCodes.Ldc_I4_5); break;
					case 6: Emit(OpCodes.Ldc_I4_6); break;
					case 7: Emit(OpCodes.Ldc_I4_7); break;
					case 8: Emit(OpCodes.Ldc_I4_8); break;
					default: Emit(OpCodes.Ldc_I4, Value); break;
				}
			}
			else if (ItemType == typeof(IntPtr))
			{
#if false
				Emit(OpCodes.Ldc_I8, ((IntPtr)Item.Value).ToInt64());
				Emit(OpCodes.Conv_I);
#else
				if (Environment.Is64BitProcess)
				{
					Emit(OpCodes.Ldc_I8, ((IntPtr)Item.Value).ToInt64());
					Emit(OpCodes.Conv_I);
				}
				else
				{
					Emit(OpCodes.Ldc_I4, ((IntPtr)Item.Value).ToInt32());
					Emit(OpCodes.Conv_I);
				}
#endif
			}
			else if (ItemType == typeof(float))
			{
				Emit(OpCodes.Ldc_R4, (float)Item.Value);
			}
			else
			{
				throw (new NotImplementedException(String.Format("Can't handle immediate type {0}", ItemType)));
			}
		}

		protected void _Generate(AstNodeStmContainer Container)
		{
			foreach (var Node in Container.Nodes)
			{
				Generate(Node);
			}
		}

		protected void _Generate(AstNodeExprArgument Argument)
		{
			int ArgumentIndex = Argument.AstArgument.Index;
			switch (ArgumentIndex)
			{
				case 0: Emit(OpCodes.Ldarg_0); break;
				case 1: Emit(OpCodes.Ldarg_1); break;
				case 2: Emit(OpCodes.Ldarg_2); break;
				case 3: Emit(OpCodes.Ldarg_3); break;
				default: Emit(OpCodes.Ldarg, ArgumentIndex); break;
			}
			
		}

		protected void _Generate(AstNodeExprLocal Local)
		{
			var LocalBuilder = Local.AstLocal.LocalBuilder;

			switch (LocalBuilder.LocalIndex)
			{
				case 0: Emit(OpCodes.Ldloc_0); break;
				case 1: Emit(OpCodes.Ldloc_1); break;
				case 2: Emit(OpCodes.Ldloc_2); break;
				case 3: Emit(OpCodes.Ldloc_3); break;
				default: Emit(OpCodes.Ldloc, LocalBuilder); break;
			}
		}

		protected void _Generate(AstNodeExprFieldAccess FieldAccess)
		{
			Generate(FieldAccess.Instance);
			Emit(OpCodes.Ldfld, FieldAccess.Field);
		}

		protected void _Generate(AstNodeExprIndirect Indirect)
		{
			Generate(Indirect.PointerExpression);
			var PointerType = Indirect.PointerExpression.Type.GetElementType();

			if (false) { }

			else if (PointerType == typeof(byte)) Emit(OpCodes.Ldind_U1);
			else if (PointerType == typeof(ushort)) Emit(OpCodes.Ldind_U2);
			else if (PointerType == typeof(uint)) Emit(OpCodes.Ldind_U4);
			else if (PointerType == typeof(ulong)) Emit(OpCodes.Ldind_I8);

			else if (PointerType == typeof(sbyte)) Emit(OpCodes.Ldind_I1);
			else if (PointerType == typeof(short)) Emit(OpCodes.Ldind_I2);
			else if (PointerType == typeof(int)) Emit(OpCodes.Ldind_I4);
			else if (PointerType == typeof(long)) Emit(OpCodes.Ldind_I8);

			else if (PointerType == typeof(float)) Emit(OpCodes.Ldind_R4);
			else if (PointerType == typeof(double)) Emit(OpCodes.Ldind_R8);

			else throw (new NotImplementedException("Can't load indirect value"));
		}

		protected void _Generate(AstNodeExprGetAddress GetAddress)
		{
			var AstNodeExprFieldAccess = (GetAddress.Expression as AstNodeExprFieldAccess);

			if (AstNodeExprFieldAccess != null)
			{
				Generate(AstNodeExprFieldAccess.Instance);
				Emit(OpCodes.Ldflda, AstNodeExprFieldAccess.Field);
			}
			else
			{
				throw(new NotImplementedException());
			}
		}

		protected void _Generate(AstNodeStmAssign Assign)
		{
			//Assign.Local.LocalBuilder.LocalIndex
			var AstNodeExprLocal = (Assign.LValue as AstNodeExprLocal);
			var AstNodeExprArgument = (Assign.LValue as AstNodeExprArgument);
			var AstNodeExprFieldAccess = (Assign.LValue as AstNodeExprFieldAccess);
			var AstNodeExprIndirect = (Assign.LValue as AstNodeExprIndirect);

			if (AstNodeExprLocal != null)
			{
				Generate(Assign.Value);
				Emit(OpCodes.Stloc, AstNodeExprLocal.AstLocal.LocalBuilder);
			}
			else if (AstNodeExprArgument != null)
			{
				Generate(Assign.Value);
				Emit(OpCodes.Starg, AstNodeExprArgument.AstArgument.Index);
			}
			else if (AstNodeExprFieldAccess != null)
			{
				Generate(AstNodeExprFieldAccess.Instance);
				Generate(Assign.Value);
				Emit(OpCodes.Stfld, AstNodeExprFieldAccess.Field);
			}
			else if (AstNodeExprIndirect != null)
			{
				var PointerType = AstUtils.GetSignedType(AstNodeExprIndirect.PointerExpression.Type.GetElementType());

				Generate(AstNodeExprIndirect.PointerExpression);
				Generate(Assign.Value);

				if (PointerType == typeof(sbyte)) Emit(OpCodes.Stind_I1);
				else if (PointerType == typeof(short)) Emit(OpCodes.Stind_I2);
				else if (PointerType == typeof(int)) Emit(OpCodes.Stind_I4);
				else if (PointerType == typeof(long)) Emit(OpCodes.Stind_I8);
				else if (PointerType == typeof(float)) Emit(OpCodes.Stind_R4);
				else if (PointerType == typeof(double)) Emit(OpCodes.Stind_R8);
				else throw (new NotImplementedException("Can't store indirect value"));
			}
			else
			{
				throw (new NotImplementedException("Not implemented " + Assign.LValue.GetType()));
			}
			//Assign.Local
		}

		protected void _Generate(AstNodeStmReturn Return)
		{
			var ExpressionType = (Return.Expression != null) ? Return.Expression.Type : typeof(void);

			if (ExpressionType != MethodInfo.ReturnType) throw (new Exception("Return type mismatch"));

			if (Return.Expression != null) Generate(Return.Expression);
			Emit(OpCodes.Ret);
		}

		protected void _Generate(AstNodeExprCallTail Call)
		{
			Generate(Call.Call);
			Emit(OpCodes.Ret);
		}

		protected void _Generate(AstNodeExprCallStatic Call)
		{
			foreach (var Parameter in Call.Parameters) Generate(Parameter);
			if (Call.IsTail) Emit(OpCodes.Tailcall);
			Emit(OpCodes.Call, Call.MethodInfo);
		}

		protected void _Generate(AstNodeExprCallInstance Call)
		{
			Generate(Call.Instance);
			foreach (var Parameter in Call.Parameters) Generate(Parameter);
			if (Call.IsTail) Emit(OpCodes.Tailcall);
			Emit(OpCodes.Call, Call.MethodInfo);
		}

		protected void _Generate(AstNodeExprCast Cast)
		{
			var CastedType = Cast.CastedType;

			Generate(Cast.Expr);

			if (Cast.Explicit)
			{
				if (false) { }

				else if (CastedType == typeof(sbyte)) Emit(OpCodes.Conv_I1);
				else if (CastedType == typeof(short)) Emit(OpCodes.Conv_I2);
				else if (CastedType == typeof(int)) Emit(OpCodes.Conv_I4);
				else if (CastedType == typeof(long)) Emit(OpCodes.Conv_I8);

				else if (CastedType == typeof(byte)) Emit(OpCodes.Conv_U1);
				else if (CastedType == typeof(ushort)) Emit(OpCodes.Conv_U2);
				else if (CastedType == typeof(uint)) Emit(OpCodes.Conv_U4);
				else if (CastedType == typeof(ulong)) Emit(OpCodes.Conv_U8);

				else if (CastedType == typeof(float)) Emit(OpCodes.Conv_R4);
				else if (CastedType == typeof(double)) Emit(OpCodes.Conv_R8);

				else if (CastedType.IsPointer) Emit(OpCodes.Conv_I);

				else
				{
					//Emit(OpCodes.Castclass, CastedType);
					throw(new NotImplementedException("Not implemented cast class"));
				}
			}
		}

		protected void _Generate(AstNodeStmIfElse IfElse)
		{
			var AfterIfLabel = ILGenerator.DefineLabel();

			Generate(IfElse.Condition);
			Emit(OpCodes.Brfalse, AfterIfLabel);
			Generate(IfElse.True);

			if (IfElse.False != null)
			{
				var AfterElseLabel = ILGenerator.DefineLabel();
				Emit(OpCodes.Br, AfterElseLabel);

				ILGenerator.MarkLabel(AfterIfLabel);

				Generate(IfElse.False);

				ILGenerator.MarkLabel(AfterElseLabel);
			}
			else
			{
				ILGenerator.MarkLabel(AfterIfLabel);
			}
		}

		protected void _Generate(AstNodeExprBinop Item)
		{
			var LeftType = Item.LeftNode.Type;
			var RightType = Item.RightNode.Type;

			//if (LeftType != RightType) throw(new Exception(String.Format("BinaryOp Type mismatch ({0}) != ({1})", LeftType, RightType)));

			//Item.GetType().GenericTypeArguments[0]
			this.Generate(Item.LeftNode);
			this.Generate(Item.RightNode);

			switch (Item.Operator)
			{
				case "+": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Add : OpCodes.Add); break;
				case "-": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Sub : OpCodes.Sub); break;
				case "*": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Mul : OpCodes.Mul); break;
				case "/": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Div : OpCodes.Div_Un); break;
				case "%": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Rem : OpCodes.Rem_Un); break;
				case "==": Emit(OpCodes.Ceq); break;
				case "!=": Emit(OpCodes.Ceq); Emit(OpCodes.Ldc_I4_0); Emit(OpCodes.Ceq); break;
				case "<": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Clt : OpCodes.Clt_Un); break;
				case ">": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Cgt : OpCodes.Cgt_Un); break;
				case "<=": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Cgt : OpCodes.Cgt_Un); Emit(OpCodes.Ldc_I4_0); Emit(OpCodes.Ceq); break;
				case ">=": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Clt : OpCodes.Clt_Un); Emit(OpCodes.Ldc_I4_0); Emit(OpCodes.Ceq); break;
				case "&": Emit(OpCodes.And); break;
				case "|": Emit(OpCodes.Or); break;
				case "^": Emit(OpCodes.Xor); break;
				case "<<": Emit(OpCodes.Shl); break;
				case ">>": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Shr : OpCodes.Shr_Un); break;
				default: throw(new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}

		protected void _Generate(AstNodeStmExpr Stat)
		{
			var ExpressionType = Stat.AstNodeExpr.Type;
			Generate(Stat.AstNodeExpr);

			if (ExpressionType != typeof(void))
			{
				Emit(OpCodes.Pop);
			}
		}

		protected void _Generate(AstNodeStmEmpty Empty)
		{
		}

		protected void _Generate(AstNodeExprUnop Item)
		{
			var RightType = Item.RightNode.Type;

			this.Generate(Item.RightNode);

			switch (Item.Operator)
			{
				case "~": Emit(OpCodes.Not); break;
				case "-": Emit(OpCodes.Neg); break;
				case "!": Emit(OpCodes.Ldc_I4_0); Emit(OpCodes.Ceq); break;
				default: throw(new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}
	}
}
