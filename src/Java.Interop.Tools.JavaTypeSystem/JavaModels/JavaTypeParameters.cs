using System;
using System.Collections.Generic;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaTypeParameters : List<JavaTypeParameter>
	{
		public JavaTypeModel? ParentType { get; }
		public JavaMethodModel? ParentMethod { get; }

		public Dictionary<string, string> PropertyBag { get; } = new Dictionary<string, string> ();

		public JavaTypeParameters (JavaTypeModel parent) => ParentType = parent;
		public JavaTypeParameters (JavaMethodModel parent) => ParentMethod = parent;
	}
}
