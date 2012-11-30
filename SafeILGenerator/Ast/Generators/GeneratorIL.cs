﻿using SafeILGenerator.Ast;
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
	public class GeneratorIL : Generator<GeneratorIL>
	{
		protected MethodInfo MethodInfo;
		protected ILGenerator ILGenerator;
		protected bool GenerateLines;
		protected List<string> Lines = new List<string>();

		public GeneratorIL()
		{
		}

		public override GeneratorIL GenerateRoot(AstNode AstNode)
		{
			if (ILGenerator != null)
			{
				var UsedSet = new HashSet<AstLabel>();
				foreach (var AstNodeStmLabel in AstNode.Descendant.Where(Node => Node is AstNodeStmLabel).Cast<AstNodeStmLabel>())
				{
					if (UsedSet.Contains(AstNodeStmLabel.AstLabel))
					{
						throw(new Exception("Label declared twice"));
					}
					UsedSet.Add(AstNodeStmLabel.AstLabel);
				}

				foreach (var AstNodeStmLabel in UsedSet)
				{
					//Console.WriteLine("CreateAllUsedLabels: {0}", AstNodeStmLabel);
					AstNodeStmLabel.Label = ILGenerator.DefineLabel();
				}
			}
			return base.GenerateRoot(AstNode);
		}

		public GeneratorIL(MethodInfo MethodInfo, ILGenerator ILGenerator, bool GenerateLines = false)
		{
			Init(MethodInfo, ILGenerator, GenerateLines);
		}

		public GeneratorIL Init(MethodInfo MethodInfo, ILGenerator ILGenerator, bool GenerateLines = false)
		{
			this.MethodInfo = MethodInfo;
			this.ILGenerator = ILGenerator;
			this.GenerateLines = GenerateLines;
			return this;
		}

		static public string GenerateToString<TGenerator>(MethodInfo MethodInfo, AstNode AstNode) where TGenerator : GeneratorIL, new()
		{
			return String.Join("\n", GenerateToStringList<TGenerator>(MethodInfo, AstNode));
		}

		static public string[] GenerateToStringList<TGenerator>(MethodInfo MethodInfo, AstNode AstNode) where TGenerator : GeneratorIL, new()
		{
			var Generator = new TGenerator();
			Generator.Init(MethodInfo, null, GenerateLines: true);
			Generator.Generate(AstNode);
			return Generator.Lines.ToArray();
		}

		static public TDelegate GenerateDelegate<TGenerator, TDelegate>(string MethodName, AstNode AstNode) where TGenerator : GeneratorIL, new()
		{
			var MethodInfo = typeof(TDelegate).GetMethod("Invoke");
			var DynamicMethod = new DynamicMethod(
				MethodName,
				MethodInfo.ReturnType,
				MethodInfo.GetParameters().Select(Parameter => Parameter.ParameterType).ToArray(),
				Assembly.GetExecutingAssembly().ManifestModule
			);
			var ILGenerator = DynamicMethod.GetILGenerator();
			var Generator = new TGenerator();
			Generator.Init(MethodInfo, ILGenerator, GenerateLines: false);
			Generator.Generate(AstNode);
			return (TDelegate)(object)DynamicMethod.CreateDelegate(typeof(TDelegate));
		}

		protected void EmitHook(OpCode OpCode, object Param)
		{
			if (GenerateLines)
			{
				Lines.Add(String.Format("  {0} {1}", OpCode, Param));
			}
		}

		protected void EmitComment(string Text)
		{
			if (GenerateLines)
			{
				Lines.Add(String.Format("; {0}", Text));
			}
		}

		protected void DefineLabelHook()
		{
		}

		protected void MarkLabelHook(AstLabel Label)
		{
			if (GenerateLines)
			{
				Lines.Add(String.Format("Label_{0}:;", Label.Name));
			}
		}

		protected AstLabel DefineLabel(string Name) { DefineLabelHook(); if (ILGenerator != null) return AstLabel.CreateFromLabel(ILGenerator.DefineLabel(), Name); return AstLabel.CreateDelayedWithName(Name); }
		protected void MarkLabel(AstLabel Label) { MarkLabelHook(Label); if (ILGenerator != null) ILGenerator.MarkLabel(Label.Label); }

		protected void Emit(OpCode OpCode) { EmitHook(OpCode, null); if (ILGenerator != null) ILGenerator.Emit(OpCode); }
		protected void Emit(OpCode OpCode, int Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, long Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, float Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, double Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, string Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, LocalBuilder Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, MethodInfo Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, FieldInfo Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, Type Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value); }
		protected void Emit(OpCode OpCode, AstLabel Value) { EmitHook(OpCode, Value); if (ILGenerator != null) ILGenerator.Emit(OpCode, Value.Label); }

		protected virtual void _Generate(AstNodeExprImm Item)
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
			else if (ItemType == typeof(string))
			{
				Emit(OpCodes.Ldstr, (string)Item.Value);
			}
			else
			{
				throw (new NotImplementedException(String.Format("Can't handle immediate type {0}", ItemType)));
			}
		}

		protected virtual void _Generate(AstNodeStmComment Comment)
		{
			EmitComment(Comment.CommentText);
		}

		protected virtual void _Generate(AstNodeStmContainer Container)
		{
			foreach (var Node in Container.Nodes)
			{
				Generate(Node);
			}
		}

		protected virtual void _Generate(AstNodeExprArgument Argument)
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

		protected virtual void _Generate(AstNodeExprLocal Local)
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

		protected virtual void _Generate(AstNodeExprFieldAccess FieldAccess)
		{
			Generate(FieldAccess.Instance);
			Emit(OpCodes.Ldfld, FieldAccess.Field);
		}

		protected virtual void _Generate(AstNodeExprStaticFieldAccess FieldAccess)
		{
			Emit(OpCodes.Ldsfld, FieldAccess.Field);
		}

		protected virtual void _Generate(AstNodeExprArrayAccess ArrayAccess)
		{
			Generate(ArrayAccess.ArrayInstance);
			Generate(ArrayAccess.Index);
			Emit(OpCodes.Ldelem_I4);
		}

		protected virtual void _Generate(AstNodeExprIndirect Indirect)
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

		protected virtual void _Generate(AstNodeExprGetAddress GetAddress)
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

		protected virtual void _Generate(AstNodeStmAssign Assign)
		{
			//Assign.Local.LocalBuilder.LocalIndex
			var AstNodeExprLocal = (Assign.LValue as AstNodeExprLocal);
			var AstNodeExprArgument = (Assign.LValue as AstNodeExprArgument);
			var AstNodeExprFieldAccess = (Assign.LValue as AstNodeExprFieldAccess);
			var AstNodeExprStaticFieldAccess = (Assign.LValue as AstNodeExprStaticFieldAccess);
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
			else if (AstNodeExprStaticFieldAccess != null)
			{
				Generate(Assign.Value);
				Emit(OpCodes.Stsfld, AstNodeExprStaticFieldAccess.Field);
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
				else if (PointerType == typeof(bool)) Emit(OpCodes.Stind_I1);
				else throw (new NotImplementedException("Can't store indirect value"));
			}
			else
			{
				throw (new NotImplementedException("Not implemented " + Assign.LValue.GetType()));
			}
			//Assign.Local
		}

		protected virtual void _Generate(AstNodeStmReturn Return)
		{
			var ExpressionType = (Return.Expression != null) ? Return.Expression.Type : typeof(void);

			if (ExpressionType != MethodInfo.ReturnType) throw (new Exception("Return type mismatch"));

			if (Return.Expression != null) Generate(Return.Expression);
			Emit(OpCodes.Ret);
		}

		protected virtual void _Generate(AstNodeExprCallTail Call)
		{
			Generate(Call.Call);
			Emit(OpCodes.Ret);
		}

		protected virtual void _Generate(AstNodeExprCallStatic Call)
		{
			if (Call.MethodInfo.CallingConvention.HasFlag(CallingConventions.HasThis))
			{
				throw (new Exception("CallString calling convention shouldn't have this"));
			}
			switch (Call.MethodInfo.CallingConvention & CallingConventions.Any)
			{
				case CallingConventions.Standard:
					foreach (var Parameter in Call.Parameters) Generate(Parameter);
					if (Call.IsTail) Emit(OpCodes.Tailcall);
					Emit(OpCodes.Call, Call.MethodInfo);
					break;
				default:
					throw (new Exception(String.Format("Can't handle calling convention {0}", Call.MethodInfo.CallingConvention)));
			}
			
		}

		protected virtual void _Generate(AstNodeExprCallDelegate Call)
		{
			_Generate((AstNodeExprCallInstance)Call);
		}

		protected virtual void _Generate(AstNodeExprCallInstance Call)
		{
			if (!Call.MethodInfo.CallingConvention.HasFlag(CallingConventions.HasThis))
			{
				throw(new Exception("CallInstance calling convention should have this"));
			}
			switch (Call.MethodInfo.CallingConvention & CallingConventions.Any)
			{
				case CallingConventions.Standard:
					Generate(Call.Instance);
					foreach (var Parameter in Call.Parameters) Generate(Parameter);
					if (Call.IsTail) Emit(OpCodes.Tailcall);
					//OpCodes.Calli
					Emit(OpCodes.Call, Call.MethodInfo);
					break;
				default:
					throw (new Exception(String.Format("Can't handle calling convention {0}", Call.MethodInfo.CallingConvention)));
			}
		}

		protected virtual void _GenerateCastToType(Type CastedType)
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

            else if (CastedType == typeof(bool)) Emit(OpCodes.Conv_I1);

			else if (CastedType.IsPointer) Emit(OpCodes.Conv_I);
			else if (CastedType.IsByRef) Emit(OpCodes.Conv_I);

			else if (CastedType.IsPrimitive)
			{
				throw (new NotImplementedException("Not implemented cast other primitives"));
			}

			else if (CastedType.IsEnum)
			{
				_GenerateCastToType(CastedType.GetEnumUnderlyingType());
				//throw (new NotImplementedException("Not implemented cast other primitives"));
			}

			else
			{
				Emit(OpCodes.Castclass, CastedType);
				//throw (new NotImplementedException("Not implemented cast class"));
			}
		}

		protected virtual void _Generate(AstNodeExprCast Cast)
		{
			var CastedType = Cast.CastedType;

			Generate(Cast.Expr);

			if (Cast.Explicit)
			{
				_GenerateCastToType(CastedType);
			}
		}

		protected virtual void _Generate(AstNodeStmIfElse IfElse)
		{
			var AfterIfLabel = DefineLabel("AfterIf");

			Generate(IfElse.Condition);
			Emit(OpCodes.Brfalse, AfterIfLabel);
			Generate(IfElse.True);

			if (IfElse.False != null)
			{
				var AfterElseLabel = DefineLabel("AfterElse");
				Emit(OpCodes.Br, AfterElseLabel);

				MarkLabel(AfterIfLabel);

				Generate(IfElse.False);

				MarkLabel(AfterElseLabel);
			}
			else
			{
				MarkLabel(AfterIfLabel);
			}
		}

		protected virtual void _Generate(AstNodeExprBinop Item)
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

		protected virtual void _Generate(AstNodeStmExpr Stat)
		{
			var ExpressionType = Stat.AstNodeExpr.Type;
			Generate(Stat.AstNodeExpr);

			if (ExpressionType != typeof(void))
			{
				Emit(OpCodes.Pop);
			}
		}

		protected virtual void _Generate(AstNodeStmEmpty Empty)
		{
		}

		protected virtual void _Generate(AstNodeStmLabel Label)
		{
			MarkLabel(Label.AstLabel);
		}

		protected virtual void _Generate(AstNodeStmGotoIfTrue Goto)
		{
			Generate(Goto.Condition);
			Emit(OpCodes.Brtrue, Goto.AstLabel);
		}

		protected virtual void _Generate(AstNodeStmGotoIfFalse Goto)
		{
			Generate(Goto.Condition);
			Emit(OpCodes.Brfalse, Goto.AstLabel);
		}

		protected virtual void _Generate(AstNodeStmGotoAlways Goto)
		{
			Emit(OpCodes.Br, Goto.AstLabel);
		}

		protected virtual void _Generate(AstNodeExprUnop Item)
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
