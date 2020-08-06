using System;
using System.Collections.Generic;

namespace Xamarin.SourceWriter
{
	public abstract class TypeWriter : ISourceWriter
	{
		private Visibility visibility;

		public string Name { get; set; }
		public string Inherits { get; set; }
		public List<string> Implements { get; } = new List<string> ();
		public bool IsPartial { get; set; } = true;
		public bool IsPublic { get => visibility == Visibility.Public; set => visibility = value ? Visibility.Public : Visibility.Default; }
		public bool IsAbstract { get; set; }
		public bool IsInternal { get => visibility == Visibility.Internal; set => visibility = value ? Visibility.Internal : Visibility.Default; }
		public bool IsShadow { get; set; }
		public bool IsSealed { get; set; }
		public bool IsStatic { get; set; }
		public bool IsPrivate { get => visibility == Visibility.Private; set => visibility = value ? Visibility.Private : Visibility.Default; }
		public bool IsProtected { get => visibility == Visibility.Protected; set => visibility = value ? Visibility.Protected : Visibility.Default; }
		public List<MethodWriter> Methods { get; } = new List<MethodWriter> ();
		public List<string> Comments { get; } = new List<string> ();
		public List<AttributeWriter> Attributes { get; } = new List<AttributeWriter> ();
		public List<FieldWriter> Fields { get; } = new List<FieldWriter> ();
		public List<PropertyWriter> Properties { get; } = new List<PropertyWriter> ();

		public void SetVisibility (string visibility)
		{
			switch (visibility?.ToLowerInvariant ()) {
				case "public":
					IsPublic = true;
					break;
				case "internal":
					IsInternal = true;
					break;
				case "protected":
					IsProtected = true;
					break;
				case "private":
					IsPrivate = true;
					break;
			}
		}

		public virtual void Write (CodeWriter writer)
		{
			WriteComments (writer);
			WriteAttributes (writer);
			WriteSignature (writer);
			WriteMembers (writer);
			WriteTypeClose (writer);
		}

		public virtual void WriteComments (CodeWriter writer)
		{
			foreach (var c in Comments)
				writer.WriteLine (c);
		}

		public virtual void WriteAttributes (CodeWriter writer)
		{
			foreach (var att in Attributes)
				att.WriteAttribute (writer);
		}

		public virtual void WriteSignature (CodeWriter writer)
		{
			if (IsPublic)
				writer.Write ("public ");
			if (IsInternal)
				writer.Write ("internal ");
			if (IsProtected)
				writer.Write ("protected ");
			if (IsPrivate)
				writer.Write ("private ");

			if (IsShadow)
				writer.Write ("new ");

			if (IsStatic)
				writer.Write ("static ");

			if (IsAbstract)
				writer.Write ("abstract ");

			if (IsSealed)
				writer.Write ("sealed ");

			if (IsPartial)
				writer.Write ("partial ");

			writer.Write (this is InterfaceWriter ? "interface " : "class ");
			writer.Write (Name + " ");

			if (Inherits.HasValue () || Implements.Count > 0)
				writer.Write (": ");

			if (Inherits.HasValue ()) {
				writer.Write (Inherits);

				if (Implements.Count > 0)
					writer.Write (",");

				writer.Write (" ");
			}

			if (Implements.Count > 0)
				writer.Write (string.Join (", ", Implements) + " ");

			writer.WriteLine ("{");
			writer.Indent ();
		}

		public virtual void WriteMembers (CodeWriter writer)
		{
			if (Fields.Count > 0) {
				writer.WriteLine ();
				WriteFields (writer);
			}

			WriteConstructors (writer);

			writer.WriteLine ();
			WriteProperties (writer);
			writer.WriteLine ();
			WriteMethods (writer);
		}

		public virtual void WriteConstructors (CodeWriter writer) { }

		public virtual void WriteFields (CodeWriter writer)
		{
			foreach (var field in Fields) {
				field.Write (writer);
				writer.WriteLine ();
			}
		}

		public virtual void WriteMethods (CodeWriter writer)
		{
			foreach (var method in Methods) {
				method.Write (writer);
				writer.WriteLine ();
			}
		}

		public virtual void WriteProperties (CodeWriter writer)
		{
			foreach (var prop in Properties) {
				prop.Write (writer);
				writer.WriteLine ();
			}
		}

		public virtual void WriteTypeClose (CodeWriter writer)
		{
			writer.Unindent ();
			writer.WriteLine ("}");
		}
	}
}