using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoDroid.Generation;
using Xamarin.SourceWriter;

namespace generator.SourceWriters
{
	public class MethodExplicitInterfaceImplementation : MethodWriter
	{
		readonly Method method;
		readonly CodeGenerationOptions opt;

		public MethodExplicitInterfaceImplementation (Method method, GenBase iface, CodeGenerationOptions opt)
		{
			this.method = method;
			this.opt = opt;

			Name = method.Name;

			ReturnType = new TypeReferenceWriter (opt.GetTypeReferenceName (method.RetVal));
			ExplicitInterfaceImplementation = opt.GetOutputName (iface.FullName);

			SourceWriterExtensions.AddMethodCustomAttributes (Attributes, method);
		}

		protected override void WriteBody (CodeWriter writer)
		{
			writer.WriteLine ($"return {Name} ({method.Parameters.GetCall (opt)})");
		}

		protected override void WriteParameters (CodeWriter writer)
		{
			writer.Write (method.GetSignature (opt));
		}
	}
}
