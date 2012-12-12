//#define USE_STATIC_REFERENCE
#define USE_NORMAL_INVOKE

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
	public abstract class Generator<TGenerator>
	{
#if !USE_NORMAL_INVOKE
#if USE_STATIC_REFERENCE
		delegate void MapDelegate(AstNode AstNode);
		private ILInstanceHolderPoolItem StaticThisReference;
#else
		delegate void MapDelegate(object This, AstNode AstNode);
#endif
#endif

#if USE_NORMAL_INVOKE
		private Dictionary<Type, MethodInfo> GenerateMappings = new Dictionary<Type, MethodInfo>();
#else
		private Dictionary<Type, MapDelegate> GenerateMappings = new Dictionary<Type, MapDelegate>();
#endif

		public Generator()
		{
#if !USE_NORMAL_INVOKE
#if USE_STATIC_REFERENCE
			this.StaticThisReference = ILInstanceHolder.Alloc(this.GetType(), this);
#endif
#endif

			foreach (
				var Method
				in
				this.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(Method => Method.ReturnType == typeof(void))
					.Where(Method => Method.GetParameters().Count() == 1)
			)
			{
				GenerateMappings[Method.GetParameters().First().ParameterType] = GenerateInvoke(Method);
			}

			this.Reset();
		}

		static Generator<GeneratorCSharp> Test;

		private void MyMethod(AstNode AstNode)
		{
			Test.MyMethod(AstNode);
		}

#if USE_NORMAL_INVOKE
		private MethodInfo GenerateInvoke(MethodInfo MethodInfo)
		{
			return MethodInfo;
		}
#else
		private MapDelegate GenerateInvoke(MethodInfo MethodInfo)
		{

			var DM = new DynamicMethod("GenerateInvoke_" + MethodInfo.Name, typeof(void), new Type[] { typeof(object), typeof(AstNode) }, this.GetType());
			var ILGenerator = DM.GetILGenerator();
			
#if USE_STATIC_REFERENCE
			ILGenerator.Emit(OpCodes.Ldsfld, StaticThisReference.FieldInfo);
#else
			ILGenerator.Emit(OpCodes.Ldarg_0);
			ILGenerator.Emit(OpCodes.Castclass, this.GetType());
#endif

			ILGenerator.Emit(OpCodes.Ldarg_1);
			ILGenerator.Emit(OpCodes.Castclass, MethodInfo.GetParameters()[0].ParameterType);
			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			if (MethodInfo.ReturnType != typeof(void)) ILGenerator.Emit(OpCodes.Pop);

			ILGenerator.Emit(OpCodes.Ret);

			return (MapDelegate)DM.CreateDelegate(typeof(MapDelegate));
		}
#endif

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
#if USE_NORMAL_INVOKE
			GenerateMappings[AstNodeType].Invoke(this, new object[] { AstNode });
#else
#if USE_STATIC_REFERENCE
			GenerateMappings[AstNodeType](AstNode);
#else
			GenerateMappings[AstNodeType](this, AstNode);
#endif
#endif
		}
	}
}
