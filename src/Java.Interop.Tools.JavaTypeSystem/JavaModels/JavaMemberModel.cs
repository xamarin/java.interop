using System;
using System.Collections.Generic;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public abstract class JavaMemberModel : IJavaResolvable
	{
		public string Name { get; }
		public bool IsStatic { get; }
		public JavaTypeModel ParentType { get; }
		public bool IsFinal { get; }
		public string Visibility { get; }
		public string Deprecated { get; }
		public string JniSignature { get; }

		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();

		public JavaMemberModel (string name, bool isStatic, bool isFinal, string visibility, JavaTypeModel parentType, string deprecated, string jniSignature)
		{
			Name = name;
			IsStatic = isStatic;
			IsFinal = isFinal;
			Visibility = visibility;
			ParentType = parentType;
			Deprecated = deprecated;
			JniSignature = jniSignature;
		}

		public abstract void Resolve (JavaTypeCollection types, List<JavaUnresolvableModel> unresolvables);
	}
}
