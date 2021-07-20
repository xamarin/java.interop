using System;
using System.Collections.Generic;
using System.Linq;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaFieldModel : JavaMemberModel
	{
		public string Type { get; }
		public string TypeGeneric { get; }
		public string? Value { get; }
		public bool IsTransient { get; }
		public bool IsVolatile { get; }
		public bool IsNotNull { get; }

		public JavaTypeReference? TypeReference { get; private set; }

		public JavaFieldModel (string name, string visibility, string type, string typeGeneric, string? value, bool isStatic, JavaTypeModel parent, bool isFinal, string deprecated, string jniSignature, bool isTransient, bool isVolatile, bool isNotNull)
			: base (name, isStatic, isFinal, visibility, parent, deprecated, jniSignature)
		{
			Type = type;
			TypeGeneric = typeGeneric;
			Value = value;
			IsTransient = isTransient;
			IsVolatile = isVolatile;
			IsNotNull = isNotNull;
		}

		public override void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables)
		{
			if (Name.Contains ('$')) {
				unresolvables.Add (new JavaUnresolvableModel (this, "$", UnresolvableType.DollarSign));
				return;
			}

			var type_parameters = ParentType.GetApplicableTypeParameters ().ToArray ();

			try {
				TypeReference = types.ResolveTypeReference (TypeGeneric, type_parameters);
			} catch (JavaTypeResolutionException) {
				unresolvables.Add (new JavaUnresolvableModel (this, TypeGeneric, UnresolvableType.FieldType));

				return;
			}
		}
	}
}
