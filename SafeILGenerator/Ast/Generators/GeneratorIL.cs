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
	public class GeneratorIL : Generator
	{
		protected MethodInfo MethodInfo;
		protected ILGenerator ILGenerator;

		public GeneratorIL(MethodInfo MethodInfo, ILGenerator ILGenerator)
		{
			this.MethodInfo = MethodInfo;
			this.ILGenerator = ILGenerator;
		}

		protected void Generate(AstNodeExprImm Item)
		{
			if (Item.Type == typeof(int))
			{
				int IntValue = (int)Item.Value;
				switch (IntValue)
				{
					case -1: ILGenerator.Emit(OpCodes.Ldc_I4_M1); break;
					case 0: ILGenerator.Emit(OpCodes.Ldc_I4_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldc_I4_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldc_I4_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldc_I4_3); break;
					case 4: ILGenerator.Emit(OpCodes.Ldc_I4_4); break;
					case 5: ILGenerator.Emit(OpCodes.Ldc_I4_5); break;
					case 6: ILGenerator.Emit(OpCodes.Ldc_I4_6); break;
					case 7: ILGenerator.Emit(OpCodes.Ldc_I4_7); break;
					case 8: ILGenerator.Emit(OpCodes.Ldc_I4_8); break;
					default: ILGenerator.Emit(OpCodes.Ldc_I4, IntValue); break;
				}
			}
			else
			{
				throw(new NotImplementedException(String.Format("Can't handle immediate type {0}", Item.Type)));
			}
		}

		protected void Generate(AstNodeStmContainer Container)
		{
			foreach (var Node in Container.Nodes)
			{
				Generate(Node);
			}
		}

		protected void Generate(AstNodeExprLocal Local)
		{
			var LocalBuilder = Local.AstLocal.LocalBuilder;

			switch (LocalBuilder.LocalIndex)
			{
				case 0: ILGenerator.Emit(OpCodes.Ldloc_0); break;
				case 1: ILGenerator.Emit(OpCodes.Ldloc_1); break;
				case 2: ILGenerator.Emit(OpCodes.Ldloc_2); break;
				case 3: ILGenerator.Emit(OpCodes.Ldloc_3); break;
				default: ILGenerator.Emit(OpCodes.Ldloc, LocalBuilder); break;
			}
		}

		protected void Generate(AstNodeStmAssign Assign)
		{
			//Assign.Local.LocalBuilder.LocalIndex
			Generate(Assign.Value);
			if (Assign.Left is AstNodeExprLocal)
			{
				ILGenerator.Emit(OpCodes.Stloc, (Assign.Left as AstNodeExprLocal).AstLocal.LocalBuilder);
			}
			else if (Assign.Left is AstNodeExprArgument)
			{
				ILGenerator.Emit(OpCodes.Starg, (Assign.Left as AstNodeExprArgument).AstArgument.Index);
			}
			else
			{
				throw(new NotImplementedException("Not implemented " + Assign.Left.GetType()));
			}
			//Assign.Local
		}

		protected void Generate(AstNodeStmReturn Return)
		{
			if (Return.Expression.Type != MethodInfo.ReturnType) throw(new Exception("Return type mismatch"));
			Generate(Return.Expression);
			ILGenerator.Emit(OpCodes.Ret);
		}

		protected void Generate(AstNodeExprCallStatic Call)
		{
			foreach (var Parameter in Call.Parameters)
			{
				Generate(Parameter);
			}
			ILGenerator.Emit(OpCodes.Call, Call.MethodInfo);
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
				case "+": ILGenerator.Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Add : OpCodes.Add_Ovf_Un); break;
				case "-": ILGenerator.Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Sub : OpCodes.Sub_Ovf_Un); break;
				case "*": ILGenerator.Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Mul : OpCodes.Mul_Ovf_Un); break;
				case "/": ILGenerator.Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Div : OpCodes.Div_Un); break;
				case "%": ILGenerator.Emit(AstUtils.IsTypeSigned(LeftType) ? OpCodes.Rem : OpCodes.Rem_Un); break;
				default: throw(new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}
	}
}
