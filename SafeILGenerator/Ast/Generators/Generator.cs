//#define USE_NORMAL_INVOKE

using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Generators
{
	public delegate void MapDelegate(object This, AstNode AstNode);

	public class MappingInfo
	{
#if USE_NORMAL_INVOKE
		public MethodInfo MethodInfo;
#else
		public MapDelegate Action;
#endif
		
		public void Call(Object This, AstNode AstNode)
		{
#if USE_NORMAL_INVOKE
			MethodInfo.Invoke(This, new object[] { AstNode });
#else
			Action(This, AstNode);
#endif
		}

#if USE_NORMAL_INVOKE
		static public MappingInfo FromMethodInfo<T>(Generator<T> that, MethodInfo MethodInfo)
		{
			return new MappingInfo()
			{
				MethodInfo = MethodInfo
			};
		}
#else
		static public MappingInfo FromMethodInfo<T>(Generator<T> that, MethodInfo MethodInfo)
		{
			var DM = new DynamicMethod("GenerateInvoke_" + MethodInfo.Name, typeof(void), new Type[] { typeof(object), typeof(AstNode) }, that.GetType());
			var ILGenerator = DM.GetILGenerator();
			
			ILGenerator.Emit(OpCodes.Ldarg_0);
			ILGenerator.Emit(OpCodes.Castclass, that.GetType());
			
			ILGenerator.Emit(OpCodes.Ldarg_1);
			ILGenerator.Emit(OpCodes.Castclass, MethodInfo.GetParameters()[0].ParameterType);
			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			if (MethodInfo.ReturnType != typeof(void)) ILGenerator.Emit(OpCodes.Pop);
			
			ILGenerator.Emit(OpCodes.Ret);
			
			return new MappingInfo()
			{
				Action = (MapDelegate)DM.CreateDelegate(typeof(MapDelegate)),
			};
		}
#endif
	}

	public abstract class Generator<TGenerator>
	{
		private Dictionary<Type, MappingInfo> GenerateMappings = new Dictionary<Type, MappingInfo>();

		public Generator()
		{
			foreach (
				var Method
				in
				this.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(Method => Method.ReturnType == typeof(void))
					.Where(Method => Method.GetParameters().Count() == 1)
			)
			{
				GenerateMappings[Method.GetParameters().First().ParameterType] = MappingInfo.FromMethodInfo(this, Method);
			}

			this.Reset();
		}

		static Generator<GeneratorCSharp> Test;

		private void MyMethod(AstNode AstNode)
		{
			Test.MyMethod(AstNode);
		}

		public virtual TGenerator Reset()
		{
			return (TGenerator)(object)this;
		}

		/// <summary>
		/// Determine dinamically which method to call.
		/// </summary>
		/// <param name="AstNode"></param>
		public virtual TGenerator GenerateRoot(AstNode AstNode)
		{
			Generate(AstNode);
			return (TGenerator)(object)this;
		}

		protected virtual void Generate(AstNode AstNode)
		{
			//if (AstNode == null) return;

			var AstNodeType = AstNode.GetType();
			if (!GenerateMappings.ContainsKey(AstNodeType))
			{
				foreach (var GenerateMapping in GenerateMappings)
				{
					Console.WriteLine(GenerateMapping);
				}
				throw (new NotImplementedException(String.Format("Don't know how to generate {0} for {1}", AstNodeType, this.GetType())));
			}

			GenerateMappings[AstNodeType].Call(this, AstNode);
		}
	}
}
