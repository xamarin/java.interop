using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Interop.Tools.TypeNameMappings;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace MonoDroid.Generation
{
	class CecilApiImporter
	{
		public static Ctor CreateCtor (GenBase declaringType, MethodDefinition m)
		{
			var reg_attr = GetRegisterAttribute (m.CustomAttributes);

			var ctor = new Ctor (declaringType) {
				AssemblyName = m.DeclaringType.Module.Assembly.FullName,
				Deprecated = m.Deprecated (),
				GenericArguments = m.GenericArguments (),
				IsAcw = reg_attr != null,
				// not a beautiful way to check static type, yes :|
				IsNonStaticNestedType = m.DeclaringType.IsNested && !(m.DeclaringType.IsAbstract && m.DeclaringType.IsSealed),
				Name = m.Name,
				Visibility = m.Visibility ()
			};

			// If 'elem' is a constructor for a non-static nested type, then
			// the type of the containing class must be inserted as the first
			// argument
			if (ctor.IsNonStaticNestedType)
				ctor.Parameters.AddFirst (CreateParameter (m.DeclaringType.DeclaringType.FullName, ctor.DeclaringType.JavaName));

			foreach (var p in m.GetParameters (reg_attr))
				ctor.Parameters.Add (p);

			return ctor;
		}

		public static Field CreateField (FieldDefinition f)
		{
			var obs_attr = GetObsoleteAttribute (f.CustomAttributes);
			var reg_attr = GetRegisterAttribute (f.CustomAttributes);

			var field = new Field {
				DeprecatedComment = GetObsoleteComment (obs_attr),
				IsAcw = reg_attr != null,
				IsDeprecated = obs_attr != null,
				IsEnumified = GetGeneratedEnumAttribute (f.CustomAttributes) != null,
				IsFinal = f.Constant != null,
				IsStatic = f.IsStatic,
				JavaName = reg_attr != null ? ((string) reg_attr.ConstructorArguments [0].Value).Replace ('/', '.') : f.Name,
				Name = f.Name,
				TypeName = f.FieldType.FullNameCorrected (),
				Value = f.Constant == null ? null : f.FieldType.FullName == "System.String" ? '"' + f.Constant.ToString () + '"' : f.Constant.ToString (),
				Visibility = f.IsPublic ? "public" : f.IsFamilyOrAssembly ? "protected internal" : f.IsFamily ? "protected" : f.IsAssembly ? "internal" : "private"
			};

			field.SetterParameter = CreateParameter (f.FieldType.Resolve ()?.FullName ?? f.FieldType.FullName, null);
			field.SetterParameter.Name = "value";

			return field;
		}

		public static Method CreateMethod (GenBase declaringType, MethodDefinition m)
		{
			var reg_attr = GetRegisterAttribute (m.CustomAttributes);

			var method = new Method (declaringType) {
				AssemblyName = m.DeclaringType.Module.Assembly.FullName,
				Deprecated = m.Deprecated (),
				GenerateAsyncWrapper = false,
				GenericArguments = m.GenericArguments (),
				IsAbstract = m.IsAbstract,
				IsAcw = reg_attr != null,
				IsFinal = m.IsFinal,
				IsInterfaceDefaultMethod = GetJavaDefaultInterfaceMethodAttribute (m.CustomAttributes) != null,
				IsReturnEnumified = GetGeneratedEnumAttribute (m.MethodReturnType.CustomAttributes) != null,
				IsStatic = m.IsStatic,
				IsVirtual = m.IsVirtual,
				JavaName = reg_attr != null ? ((string) reg_attr.ConstructorArguments [0].Value) : m.Name,
				ManagedReturn = m.ReturnType.FullNameCorrected (),
				Return = m.ReturnType.FullNameCorrected (),
				Visibility = m.Visibility ()
			};

			foreach (var p in m.GetParameters (reg_attr))
				method.Parameters.Add (p);

			if (reg_attr != null) {
				var jnisig = (string) (reg_attr.ConstructorArguments.Count > 1 ? reg_attr.ConstructorArguments [1].Value : reg_attr.Properties.First (p => p.Name == "JniSignature").Argument.Value);
				var rt = JavaNativeTypeManager.ReturnTypeFromSignature (jnisig);
				if (rt?.Type != null)
					method.Return = rt.Type;
			}

			method.FillReturnType ();

			// Strip "Formatted" from ICharSequence-based method.
			var name_base = method.IsReturnCharSequence ? m.Name.Substring (0, m.Name.Length - "Formatted".Length) : m.Name;

			method.Name = m.IsGetter ? (m.Name.StartsWith ("get_Is") && m.Name.Length > 6 && char.IsUpper (m.Name [6]) ? string.Empty : "Get") + name_base.Substring (4) : m.IsSetter ? (m.Name.StartsWith ("set_Is") && m.Name.Length > 6 && char.IsUpper (m.Name [6]) ? string.Empty : "Set") + name_base.Substring (4) : name_base;

			return method;
		}

		public static Parameter CreateParameter (ParameterDefinition p, string jnitype, string rawtype)
		{
			// FIXME: safe to use CLR type name? assuming yes as we often use it in metadatamap.
			// FIXME: IsSender?
			var isEnumType = GetGeneratedEnumAttribute (p.CustomAttributes) != null;;
			return new Parameter (SymbolTable.MangleName (p.Name), jnitype ?? p.ParameterType.FullNameCorrected (), null, isEnumType, rawtype);
		}

		public static Parameter CreateParameter (string managedType, string javaType)
		{
			return new Parameter ("__self", javaType ?? managedType, managedType, false);
		}

		static CustomAttribute GetJavaDefaultInterfaceMethodAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullName == "Java.Interop.JavaInterfaceDefaultMethodAttribute");

		static CustomAttribute GetGeneratedEnumAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullName == "Android.Runtime.GeneratedEnumAttribute");

		static CustomAttribute GetObsoleteAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "System.ObsoleteAttribute");

		static string GetObsoleteComment (CustomAttribute attribute) =>
			attribute?.ConstructorArguments.Any () == true ? (string) attribute.ConstructorArguments [0].Value : null;

		static CustomAttribute GetRegisterAttribute (Collection<CustomAttribute> attributes) =>
			attributes.FirstOrDefault (a => a.AttributeType.FullNameCorrected () == "Android.Runtime.RegisterAttribute");
	}
}
