using System;

namespace Java.Interop.Tools.JavaTypeSystem.Models
{
	public class JavaConstructorModel : JavaMethodModel
	{
		public JavaConstructorModel (string javaName, string javaVisibility, bool javaAbstract, bool javaFinal, bool javaStatic, JavaTypeModel javaParentType, string deprecated, string jniSignature, bool isSynthetic, bool isBridge)
			: base (javaName, javaVisibility, javaAbstract, javaFinal, javaStatic, "void", javaParentType, deprecated, jniSignature, isSynthetic, isBridge, string.Empty, false, false, false)
		{
		}

		public override string ToString () => $"Constructor: {ParentType.FullName}.{Name}";
	}
}
