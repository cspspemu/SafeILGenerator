using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;

namespace NPhp.Codegen
{
	public partial class SafeILGenerator
	{
		public void UnaryOperation(SafeUnaryOperator Operator)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));

				TypeStack.Push(TypeRight);
			}

			if (DoEmit)
			{
				switch (Operator)
				{
					case SafeUnaryOperator.Negate: ILGenerator.Emit(OpCodes.Neg); break;
					case SafeUnaryOperator.Not: ILGenerator.Emit(OpCodes.Not); break;
					default: throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("UnaryOperation({0}) :: Stack -> {1}", Operator, TypeStack.Count));
			}
		}

		public void MarkSequencePoint(ISymbolDocumentWriter Document, int StartLine, int StartColumn, int EndLine, int EndColumn)
		{
			ILGenerator.MarkSequencePoint(Document, StartLine, StartColumn, EndLine, EndColumn);
		}

		public void StoreElement(Type Type)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
				var StoreIndexValue = TypeStack.Pop();
				var StoreArrayValue = TypeStack.Pop();
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Stelem_I); break; }
					if (Type == typeof(sbyte) || Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Stelem_I1); break; }
					if (Type == typeof(short) || Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Stelem_I2); break; }
					if (Type == typeof(int) || Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Stelem_I4); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Stelem_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Stelem_R8); break; }
					throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("StoreElement({0}) :: Stack -> {1}", Type.Name, TypeStack.Count));
			}
		}

		public void StoreElement<TType>()
		{
			StoreElement(typeof(TType));
		}

		public void StoreIndirect(Type Type)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
				var StoreAddressValue = TypeStack.Pop();
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Stind_I); break; }
					if (Type == typeof(sbyte) || Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Stind_I1); break; }
					if (Type == typeof(short) || Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Stind_I2); break; }
					if (Type == typeof(int) || Type == typeof(uint)) {ILGenerator.Emit(OpCodes.Stind_I4); break; }
					if (Type == typeof(long) || Type == typeof(ulong)) { ILGenerator.Emit(OpCodes.Stind_I8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Stind_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Stind_R8); break; }
					throw(new NotImplementedException("Can't store indirectly type '" + Type.Name + "'"));
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("StoreIndirect({0}) :: Stack -> {1}", Type.Name, TypeStack.Count));
			}
		}

		public void StoreIndirect<TType>()
		{
			StoreIndirect(typeof(TType));
		}

		public void StoreLocal(LocalBuilder Local)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
				if (StoreValueType != Local.LocalType) throw (new InvalidOperationException(String.Format("Type {0} != {1}", StoreValueType, Local.LocalType)));
			}

			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;
				switch (LocalIndex)
				{
					case 0: ILGenerator.Emit(OpCodes.Stloc_0); break;
					case 1: ILGenerator.Emit(OpCodes.Stloc_1); break;
					case 2: ILGenerator.Emit(OpCodes.Stloc_2); break;
					case 3: ILGenerator.Emit(OpCodes.Stloc_3); break;
					default: ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Stloc_S : OpCodes.Stloc, Local); break;
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("StoreLocal({0}) :: Stack -> {1}", Local.LocalType, TypeStack.Count));
			}
		}

		public void StoreArgument<TType>(int ArgumentIndex)
		{
			if (TrackStack)
			{
				var ArgumentIndexType = TypeStack.Pop();
				var ArgumentValueType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(((int)(byte)ArgumentIndex == (int)ArgumentIndex) ? OpCodes.Starg_S : OpCodes.Starg, ArgumentIndex);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("StoreArgument<{0}>({1}) :: Stack -> {2}", typeof(TType).Name, ArgumentIndex, TypeStack.Count));
			}
		}

		public void Box<TType>()
		{
			Box(typeof(TType));
		}

		public void Box(Type Type)
		{
			if (TrackStack)
			{
				var TypeType = TypeStack.Pop();
				if (TypeType != Type) throw(new InvalidCastException());
				TypeStack.Push(typeof(object));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Box, Type);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Box({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}

		public void Unbox(Type Type)
		{
			if (TrackStack)
			{
				var ObjectType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Unbox);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Unbox({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}

		public void Unbox<TType>()
		{
			Unbox(typeof(TType));
		}

		public void SetPointerAttributes(SafePointerAttributes Attributes)
		{
			if (TrackStack)
			{
				var AddressType = TypeStack.Pop();
				TypeStack.Push(AddressType); // Unaligned
			}

			if (DoEmit)
			{
				if ((Attributes & SafePointerAttributes.Unaligned) != 0)
				{
					ILGenerator.Emit(OpCodes.Unaligned);
				}

				if ((Attributes & SafePointerAttributes.Volatile) != 0)
				{
					ILGenerator.Emit(OpCodes.Volatile);
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("SetPointerAttributes({0}) :: Stack -> {1}", Attributes, TypeStack.Count));
			}
		}

		public void Tailcall()
		{
			throw (new NotImplementedException());
			//ILGenerator.Emit(OpCodes.Tailcall);
		}

		public void Switch(Dictionary<int, SafeLabel> Labels, SafeLabel DefaultLabel)
		{
			var MinKey = Labels.Keys.Min();
			var MaxKey = Labels.Keys.Max();
			var Length = MaxKey - MinKey;

			BranchAlways(DefaultLabel);
			throw (new NotImplementedException());
		}

		public void Switch(SafeLabel[] Labels)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Switch, Labels.Select(Label => Label.ReflectionLabel).ToArray());
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Switch({0}) :: Stack -> {1}", Labels, TypeStack.Count));
			}
		}

		public void Sizeof()
		{
			if (TrackStack)
			{
				var BaseType = TypeStack.Pop();
				TypeStack.Push(typeof(int));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Sizeof);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Sizeof() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void Return()
		{
			if (TrackStack)
			{
				// @TODO CHECK RETURN TYPE
				//var ReturnType = TypeStack.Pop();
				//throw(new NotImplementedException());
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ret);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Return() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void NoOperation()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Nop);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("NoOperation() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void BinaryOperation(SafeBinaryOperator Operator)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (CheckTypes)
				{
					if (TypeLeft != TypeRight)
					{
						throw (new InvalidOperationException(String.Format(
							"Binary operation mismatch Left:{0} != Right:{1}",
							TypeLeft.Name,
							TypeRight.Name
						)));
					}
				}

				TypeStack.Push(TypeRight);
			}

			if (DoEmit)
			{
				switch (Operator)
				{
					case SafeBinaryOperator.AdditionSigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Add_Ovf : OpCodes.Add); break;
					case SafeBinaryOperator.AdditionUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Add_Ovf_Un : OpCodes.Add); break;
					case SafeBinaryOperator.SubstractionSigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Sub_Ovf : OpCodes.Sub); break;
					case SafeBinaryOperator.SubstractionUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Sub_Ovf_Un : OpCodes.Sub); break;
					case SafeBinaryOperator.DivideSigned: ILGenerator.Emit(OpCodes.Div); break;
					case SafeBinaryOperator.DivideUnsigned: ILGenerator.Emit(OpCodes.Div_Un); break;
					case SafeBinaryOperator.RemainingSigned: ILGenerator.Emit(OpCodes.Rem); break;
					case SafeBinaryOperator.RemainingUnsigned: ILGenerator.Emit(OpCodes.Rem_Un); break;
					case SafeBinaryOperator.MultiplySigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Mul_Ovf : OpCodes.Mul); break;
					case SafeBinaryOperator.MultiplyUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Mul_Ovf_Un : OpCodes.Mul); break;
					case SafeBinaryOperator.And: ILGenerator.Emit(OpCodes.And); break;
					case SafeBinaryOperator.Or: ILGenerator.Emit(OpCodes.Or); break;
					case SafeBinaryOperator.Xor: ILGenerator.Emit(OpCodes.Xor); break;
					case SafeBinaryOperator.ShiftLeft: ILGenerator.Emit(OpCodes.Shl); break;
					case SafeBinaryOperator.ShiftRightSigned: ILGenerator.Emit(OpCodes.Shr); break;
					case SafeBinaryOperator.ShiftRightUnsigned: ILGenerator.Emit(OpCodes.Shr_Un); break;
					default: throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("BinaryOperation({0}) :: Stack -> {1}", Operator, TypeStack.Count));
			}
		}

		public void Duplicate()
		{
			if (TrackStack)
			{
				var TypeToDuplicate = TypeStack.Pop();
				TypeStack.Push(TypeToDuplicate);
				TypeStack.Push(TypeToDuplicate);
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Dup);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Duplicate() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void CompareBinary(SafeBinaryComparison Comparison)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));

				TypeStack.Push(typeof(bool));
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case SafeBinaryComparison.Equals: ILGenerator.Emit(OpCodes.Ceq); break;
					case SafeBinaryComparison.NotEquals: ILGenerator.Emit(OpCodes.Ceq); ILGenerator.Emit(OpCodes.Neg); break;
					case SafeBinaryComparison.GreaterThanSigned: ILGenerator.Emit(OpCodes.Cgt); break;
					case SafeBinaryComparison.GreaterThanUnsigned: ILGenerator.Emit(OpCodes.Cgt_Un); break;
					case SafeBinaryComparison.GreaterOrEqualSigned: ILGenerator.Emit(OpCodes.Clt); ILGenerator.Emit(OpCodes.Ldc_I4_0); ILGenerator.Emit(OpCodes.Ceq); break;
					case SafeBinaryComparison.GreaterOrEqualUnsigned: ILGenerator.Emit(OpCodes.Clt_Un); ILGenerator.Emit(OpCodes.Ldc_I4_0); ILGenerator.Emit(OpCodes.Ceq); break;
					case SafeBinaryComparison.LessThanSigned: ILGenerator.Emit(OpCodes.Clt); break;
					case SafeBinaryComparison.LessThanUnsigned: ILGenerator.Emit(OpCodes.Clt_Un); break;
					case SafeBinaryComparison.LessOrEqualSigned: ILGenerator.Emit(OpCodes.Cgt); ILGenerator.Emit(OpCodes.Ldc_I4_0); ILGenerator.Emit(OpCodes.Ceq); break;
					case SafeBinaryComparison.LessOrEqualUnsigned: ILGenerator.Emit(OpCodes.Cgt_Un); ILGenerator.Emit(OpCodes.Ldc_I4_0); ILGenerator.Emit(OpCodes.Ceq); break;
					default: throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("CompareBinary({0}) :: Stack -> {1}", Comparison, TypeStack.Count));
			}
		}

		public void ConvertTo<TType>()
		{
			ConvertTo(typeof(TType));
		}

		public void ConvertTo(Type Type)
		{
			// @TODO: From unsigned values

			if (TrackStack)
			{
				var PreviousType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I : OpCodes.Conv_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I4 : OpCodes.Conv_I4); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U4 : OpCodes.Conv_U4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I8 : OpCodes.Conv_I8); break; }
					if (Type == typeof(ulong)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U8 : OpCodes.Conv_U8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Conv_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Conv_R8); break; }

					throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("ConvertTo({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}

		public void CopyBlock()
		{
			if (TrackStack)
			{
				var CountType = TypeStack.Pop();
				var DestinationAddressType = TypeStack.Pop();
				var SourceAddressType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Cpblk);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("CopyBlock() :: Stack -> {0}", TypeStack.Count));
			}

			throw (new NotImplementedException());
		}

		public void CopyObject()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Cpobj);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("CopyObject() :: Stack -> {0}", TypeStack.Count));
			}

			throw (new NotImplementedException());
		}

		public void Constrained()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Constrained);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Constrained() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void CheckFinite()
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ckfinite);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("CheckFinite() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void BranchBinaryComparison(SafeBinaryComparison Comparison, SafeLabel Label)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case SafeBinaryComparison.Equals: ILGenerator.Emit(OpCodes.Beq, Label.ReflectionLabel); break;
					case SafeBinaryComparison.NotEquals: ILGenerator.Emit(OpCodes.Bne_Un, Label.ReflectionLabel); break;

					case SafeBinaryComparison.GreaterOrEqualSigned: ILGenerator.Emit(OpCodes.Bge, Label.ReflectionLabel); break;
					case SafeBinaryComparison.GreaterOrEqualUnsigned: ILGenerator.Emit(OpCodes.Bge_Un, Label.ReflectionLabel); break;
					case SafeBinaryComparison.GreaterThanSigned: ILGenerator.Emit(OpCodes.Bgt, Label.ReflectionLabel); break;
					case SafeBinaryComparison.GreaterThanUnsigned: ILGenerator.Emit(OpCodes.Bgt_Un, Label.ReflectionLabel); break;

					case SafeBinaryComparison.LessOrEqualSigned: ILGenerator.Emit(OpCodes.Ble, Label.ReflectionLabel); break;
					case SafeBinaryComparison.LessOrEqualUnsigned: ILGenerator.Emit(OpCodes.Ble_Un, Label.ReflectionLabel); break;
					case SafeBinaryComparison.LessThanSigned: ILGenerator.Emit(OpCodes.Blt, Label.ReflectionLabel); break;
					case SafeBinaryComparison.LessThanUnsigned: ILGenerator.Emit(OpCodes.Blt_Un, Label.ReflectionLabel); break;

					default: throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("BranchBinaryComparison({0}, {1}) :: Stack -> {2}", Comparison, Label, TypeStack.Count));
			}
		}

		public void BranchUnaryComparison(SafeUnaryComparison Comparison, SafeLabel Label)
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();

				if (CheckTypes)
				{
					if (Type != typeof(bool))
					{
						throw (new InvalidOperationException("Required boolean value"));
					}
				}
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case SafeUnaryComparison.False: ILGenerator.Emit(OpCodes.Brfalse, Label.ReflectionLabel); break;
					case SafeUnaryComparison.True: ILGenerator.Emit(OpCodes.Brtrue, Label.ReflectionLabel); break;
					default: throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("_BranchUnaryComparison({0}, {1}) :: Stack -> {2}", Comparison, Label, TypeStack.Count));
			}
		}

		public void BranchIfTrue(SafeLabel Label)
		{
			BranchUnaryComparison(SafeUnaryComparison.True, Label);
		}

		public void BranchIfFalse(SafeLabel Label)
		{
			BranchUnaryComparison(SafeUnaryComparison.False, Label);
		}

		static private Type GetCommonType(Type Type)
		{
			if (Type == typeof(byte)) return typeof(sbyte);
			if (Type == typeof(ushort)) return typeof(short);
			if (Type == typeof(uint)) return typeof(int);
			if (Type == typeof(ulong)) return typeof(long);
			if (Type.IsPointer) return typeof(void*);
			if (Type.IsEnum)  return typeof(int);
			return Type;
		}

		private void _Jmp_Call(OpCode OpCode, MethodInfo MethodInfo)
		{
			if (MethodInfo == null)
			{
				throw(new ArgumentNullException("MethodInfo can't be null!"));
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("_Jmp_Call({0}, {1}) :: Stack -> {2}", OpCode.Name, MethodInfo, TypeStack.Count));
			}

			if (TrackStack)
			{
				int CurrentArgumentIndex = 0;
				foreach (var Parameter in MethodInfo.GetParameters().Reverse())
				{
					var FunctionParameterType = Parameter.ParameterType;
					var StackParameterType = TypeStack.Pop();
					
					if (FunctionParameterType == typeof(bool)) FunctionParameterType = typeof(int);
					if (StackParameterType == typeof(bool)) StackParameterType = typeof(int);

					if (CheckTypes)
					{
						if (FunctionParameterType != StackParameterType)
						{
							if (GetCommonType(FunctionParameterType) != GetCommonType(StackParameterType))
							{
								throw (new InvalidOperationException(
									String.Format(
										"Type mismatch : Argument{0}. Expected: '{1}' but found on Stack: '{2}' on function '{3}.{4}'",
										CurrentArgumentIndex, FunctionParameterType.Name, StackParameterType.Name, MethodInfo.DeclaringType.Name, MethodInfo.Name
									)
								));
							}
						}
					}
					CurrentArgumentIndex++;
				}

				if (!MethodInfo.IsStatic)
				{
					var ThisType = TypeStack.Pop();
				}

				if (MethodInfo.ReturnType != typeof(void))
				{
					TypeStack.Push(MethodInfo.ReturnType);
				}
			}

			if (DoEmit)
			{
				//Console.WriteLine(OpCode);
				ILGenerator.Emit(OpCode, MethodInfo);
			}
		}

		public void ResetStack()
		{
			int StackCount = TypeStack.Count;

			for (int n = 0; n > StackCount; n++) Pop();
		}

		public void Jmp(MethodInfo MethodInfo)
		{
			ResetStack();
			_Jmp_Call(OpCodes.Jmp, MethodInfo);
		}

		public void Call(Delegate Delegate)
		{
			Call(Delegate.Method);
		}

		public void Call(MethodInfo MethodInfo)
		{
			_Jmp_Call(MethodInfo.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, MethodInfo);
		}

		public void Break()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Break);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Break() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void BranchAlways(SafeLabel Label)
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Br, Label.ReflectionLabel);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("BranchAlways() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void Pop()
		{
			if (TrackStack)
			{
				TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Pop);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Pop() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void LoadArgument<TType>(int ArgumentIndex)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(TType));
			}

			if (DoEmit)
			{
				switch (ArgumentIndex)
				{
					case 0: ILGenerator.Emit(OpCodes.Ldarg_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldarg_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldarg_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldarg_3); break;
					default: ILGenerator.Emit(((int)(byte)ArgumentIndex == (int)ArgumentIndex) ? OpCodes.Ldarg_S : OpCodes.Ldarg, ArgumentIndex); break;
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadArgument<{0}>({1}) :: Stack -> {2}", typeof(TType).Name, ArgumentIndex,  TypeStack.Count));
			}
		}

		public void LoadArgumentFromIndexAtStack()
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();
				
				// @check type is integer

				TypeStack.Push(typeof(object));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldarg_S);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadArgumentFromIndexAtStack() :: Stack -> {0}", TypeStack.Count));
			}
		}

		private void _PushIntAs<TType>(int Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(TType));
			}

			if (DoEmit)
			{
				switch (Value)
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
					default:
						if ((int)(sbyte)Value == (int)Value)
						{
							ILGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)Value);
						}
						else
						{
							ILGenerator.Emit(OpCodes.Ldc_I4, Value);
						}
						break;
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Push({0}) :: Stack -> {1}", Value, TypeStack.Count));
			}
		}

		public void Push(bool Value)
		{
			_PushIntAs<bool>(Value ? 1 : 0);
		}

		public void Push(int Value)
		{
			_PushIntAs<int>(Value);
		}

		public void Push(long Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(long));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_I8, Value);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Push({0}) :: Stack -> {1}", Value, TypeStack.Count));
			}
		}

		public void Push(float Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(float));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_R4, Value);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Push({0}) :: Stack -> {1}", Value, TypeStack.Count));
			}
		}

		public void Push(double Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(float));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_R8, Value);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Push({0}) :: Stack -> {1}", Value, TypeStack.Count));
			}
		}

		public void Push(string Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(string));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldstr, Value);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("Push('{0}') :: Stack -> {1}", Value, TypeStack.Count));
			}
		}

		private void _LoadElement_Reference_Address(OpCode OpCode)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
				var ArrayType = TypeStack.Pop();

				// @TODO: reference
				TypeStack.Push(typeof(object));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCode);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("_LoadElement_Reference_Address({0}) :: Stack -> {1}", OpCode.Name, TypeStack.Count));
			}
		}

		public void LoadElementAddress()
		{
			_LoadElement_Reference_Address(OpCodes.Ldelema);
		}

		public void LoadElementReference()
		{
			_LoadElement_Reference_Address(OpCodes.Ldelem_Ref);
		}

		public void StoreField(FieldInfo FieldInfo)
		{
			if (TrackStack)
			{
				var ValueType = TypeStack.Pop();
				var ObjectType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Stfld, FieldInfo);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("StoreField({0}) :: Stack -> {1}", FieldInfo, TypeStack.Count));
			}
		}

		private void _LoadField_Address(OpCode OpCode, FieldInfo FieldInfo)
		{
			if (TrackStack)
			{
				//var FieldInfoType = TypeStack.Pop();
				var ObjectType = TypeStack.Pop();

				// @TODO: Field reference
				TypeStack.Push(typeof(object));
			}
	
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldfld, FieldInfo);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("_LoadField_Address({0}) :: Stack -> {1}", OpCode.Name, TypeStack.Count));
			}
		}

		public void LoadField(FieldInfo FieldInfo)
		{
			_LoadField_Address(OpCodes.Ldfld, FieldInfo);
		}

		public void LoadFieldAddress(FieldInfo FieldInfo)
		{
			_LoadField_Address(OpCodes.Ldflda, FieldInfo);
		}

		public void LoadMethodAddress()
		{
			if (TrackStack)
			{
				var MethodInfoType = TypeStack.Pop();
				var ObjectType = TypeStack.Pop();

				// @TODO: Field reference
				TypeStack.Push(typeof(object));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldftn);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadMethodAddress() :: Stack -> {0}", TypeStack.Count));
			}
			//OpCodes.Ldind_I
		}

		public void LoadLength()
		{
			if (TrackStack)
			{
				var ArrayType = TypeStack.Pop();
				TypeStack.Push(typeof(int));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldlen); 
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadLength() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void LoadLocal(LocalBuilder Local)
		{
			if (TrackStack)
			{
				TypeStack.Push(Local.LocalType);
			}

			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;
				switch (Local.LocalIndex)
				{
					case 0: ILGenerator.Emit(OpCodes.Ldloc_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldloc_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldloc_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldloc_3); break;
					default: ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Ldloc_S : OpCodes.Ldloc, Local); break;
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadLocal({0}) :: Stack -> {1}", Local, TypeStack.Count));
			}
		}

		public void LoadLocalAddress(LocalBuilder Local)
		{
			if (TrackStack)
			{
				// @TODO: Address
				TypeStack.Push(Local.LocalType);
			}

			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;

				ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Ldloca_S : OpCodes.Ldloca);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadLocalAddress({0}) :: Stack -> {1}", Local, TypeStack.Count));
			}
		}

		public void LoadNull()
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(DBNull));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldnull);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadNull() :: Stack -> {0}", TypeStack.Count));
			}
		}

		public void LoadIndirect<TType>()
		{
			LoadIndirect(typeof(TType));
		}

		public void LoadIndirect(Type Type)
		{
			if (TrackStack)
			{
				var PointerType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Ldind_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OpCodes.Ldind_I1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OpCodes.Ldind_I2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OpCodes.Ldind_I4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OpCodes.Ldind_I8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Ldind_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Ldind_R8); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Ldind_U1); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Ldind_U2); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Ldind_U4); break; }
					//ILGenerator.Emit(OpCodes.Ldelem);
					throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadIndirect({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}

		public void LoadElementFromArray(Type Type)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
				var ArrayType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Ldelem_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OpCodes.Ldelem_I1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OpCodes.Ldelem_I2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OpCodes.Ldelem_I4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OpCodes.Ldelem_I8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Ldelem_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Ldelem_R8); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Ldelem_U1); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Ldelem_U2); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Ldelem_U4); break; }
					//ILGenerator.Emit(OpCodes.Ldelem);
					throw (new NotImplementedException());
				}
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("LoadElementFromArray({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}

		public void LoadElementFromArray<TType>()
		{
			LoadElementFromArray(typeof(TType));
		}

		public void Push(uint Value)
		{
			Push(unchecked((int)Value));
		}

		public void Push(ulong Value)
		{
			Push(unchecked((long)Value));
		}

		public void EmitWriteLine(String Value)
		{
			ILGenerator.EmitWriteLine(Value);
		}

		public void CastClass<TType>()
		{
			CastClass(typeof(TType));
		}

		public void CastClass(Type Type)
		{
			if (TrackStack)
			{
				var ObjectType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Castclass, Type);
			}

			if (DoDebug)
			{
				Debug.WriteLine(String.Format("CastClass({0}) :: Stack -> {1}", Type, TypeStack.Count));
			}
		}
	}
}
