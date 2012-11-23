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
#if DEBUG_GENERATOR_IL
			Console.WriteLine("{0} {1}", OpCode, Param);
#endif
		}

		private void Emit(OpCode OpCode) { EmitHook(OpCode, null); ILGenerator.Emit(OpCode); }
		private void Emit(OpCode OpCode, int Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, LocalBuilder Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, MethodInfo Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, FieldInfo Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }
		private void Emit(OpCode OpCode, Type Value) { EmitHook(OpCode, Value); ILGenerator.Emit(OpCode, Value); }

		protected void Generate(AstNodeExprImm Item)
		{
			var ItemType = Item.Type;
			var ItemValue = Item.Value;

			if (ItemType == typeof(int) || ItemType == typeof(uint))
			{
				var Value = (ItemType == typeof(int)) ? (int)ItemValue : (int)(uint)ItemValue;
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
			else
			{
				throw (new NotImplementedException(String.Format("Can't handle immediate type {0}", ItemType)));
			}
		}

		protected void Generate(AstNodeStmContainer Container)
		{
			foreach (var Node in Container.Nodes)
			{
				Generate(Node);
			}
		}

		protected void Generate(AstNodeExprArgument Argument)
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

		protected void Generate(AstNodeExprLocal Local)
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

		protected void Generate(AstNodeExprFieldAccess FieldAccess)
		{
			Generate(FieldAccess.Instance);
			Emit(OpCodes.Ldfld, FieldAccess.Field);
		}

		protected void Generate(AstNodeStmAssign Assign)
		{
			//Assign.Local.LocalBuilder.LocalIndex
			var AstNodeExprLocal = (Assign.LValue as AstNodeExprLocal);
			var AstNodeExprArgument = (Assign.LValue as AstNodeExprArgument);
			var AstNodeExprFieldAccess = (Assign.LValue as AstNodeExprFieldAccess);

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
			else
			{
				throw (new NotImplementedException("Not implemented " + Assign.LValue.GetType()));
			}
			//Assign.Local
		}

		protected void Generate(AstNodeStmReturn Return)
		{
			if (Return.Expression.Type != MethodInfo.ReturnType) throw(new Exception("Return type mismatch"));
			Generate(Return.Expression);
			Emit(OpCodes.Ret);
		}

		protected void Generate(AstNodeExprCallStatic Call)
		{
			foreach (var Parameter in Call.Parameters)
			{
				Generate(Parameter);
			}
			Emit(OpCodes.Call, Call.MethodInfo);
		}

		protected void Generate(AstNodeExprCast Cast)
		{
			var CastedType = Cast.CastedType;

			Generate(Cast.Expr);

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

			else
			{
				Emit(OpCodes.Castclass, CastedType);
			}
		}

		protected void Generate(AstNodeExprBinop Item)
		{
			var LeftType = Item.LeftNode.Type;
			var RightType = Item.RightNode.Type;

			if (LeftType != RightType) throw(new Exception("Type mismatch"));

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
				case "&": Emit(OpCodes.And); break;
				case "|": Emit(OpCodes.Or); break;
				case "^": Emit(OpCodes.Xor); break;
				case "<<": Emit(OpCodes.Shl); break;
				case ">>": Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Shr : OpCodes.Shr_Un); break;
				default: throw(new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}

		protected void Generate(AstNodeStmEmpty Empty)
		{
		}

		protected void Generate(AstNodeExprUnop Item)
		{
			var RightType = Item.RightNode.Type;

			this.Generate(Item.RightNode);

			switch (Item.Operator)
			{
				case "~": Emit(OpCodes.Not); break;
				case "-": Emit(OpCodes.Neg); break;
				default: throw (new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}
	}
}
