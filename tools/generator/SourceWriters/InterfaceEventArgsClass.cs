using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class InterfaceEventArgsClass : ClassWriter
	{
		public InterfaceEventArgsClass (InterfaceGen @interface, Method method, CodeGenerationOptions opt, CodeGeneratorContext context)
		{
			Name = @interface.GetArgsName (method);
			Inherits = "global::System.EventArgs";

			IsPublic = true;
			IsPartial = true;

			UsePriorityOrder = true;

			Comments.Add ($"// event args for {@interface.JavaName}.{method.JavaName}");

			AddConstructor (@interface, method, opt);

			if (method.IsEventHandlerWithHandledProperty)
				Properties.Add (new HandledProperty { Priority = GetNextPriority () });

			AddProperties (method, opt);
		}

		void AddConstructor (InterfaceGen @interface, Method method, CodeGenerationOptions opt)
		{
			var ctor = new ConstructorWriter {
				Name = @interface.GetArgsName (method),
				IsPublic = true,
				Priority = GetNextPriority ()
			};

			if (method.IsEventHandlerWithHandledProperty) {
				ctor.Parameters.Add (new MethodParameterWriter ("handled", TypeReferenceWriter.Bool));
				ctor.Body.Add ("this.handled = handled;");
			}

			foreach (var p in method.Parameters) {
				if (p.IsSender)
					continue;

				ctor.Parameters.Add (new MethodParameterWriter (p.Name, new TypeReferenceWriter (opt.GetTypeReferenceName (p))));
				ctor.Body.Add ($"this.{opt.GetSafeIdentifier (p.Name)} = {opt.GetSafeIdentifier (p.Name)};");
			}

			Constructors.Add (ctor);
		}

		void AddProperties (Method method, CodeGenerationOptions opt)
		{
			foreach (var p in method.Parameters) {
				if (p.IsSender)
					continue;

				Fields.Add (new FieldWriter {
					Name = opt.GetSafeIdentifier (p.Name),
					Type = new TypeReferenceWriter (opt.GetTypeReferenceName (p)),
					Priority = GetNextPriority ()
				});

				var prop = new PropertyWriter {
					Name = p.PropertyName,
					PropertyType = new TypeReferenceWriter (opt.GetTypeReferenceName (p)),
					IsPublic = true,
					HasGet = true,
					Priority = GetNextPriority ()
				};

				prop.GetBody.Add ($"return {opt.GetSafeIdentifier (p.Name)};");

				Properties.Add (prop);
			}
		}
	}

	public class HandledProperty : PropertyWriter
	{
		public HandledProperty ()
		{
			Name = "Handled";
			PropertyType = TypeReferenceWriter.Bool;

			IsPublic = true;

			HasGet = true;
			GetBody.Add ("return handled;");

			HasSet = true;
			SetBody.Add ("handled = value;");
		}

		public override void Write (CodeWriter writer)
		{
			writer.Write ("bool handled;");

			base.Write (writer);
		}
	}
}
