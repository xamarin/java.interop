using System;

using Mono.Cecil;

namespace Android.Runtime {

	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property)]
#if !JCW_ONLY_TYPE_NAMES
	public
#endif  // !JCW_ONLY_TYPE_NAMES
	sealed class RegisterAttribute : Attribute, Java.Interop.IJniNameProviderAttribute {

		string connector;
		string name;
		string signature;

		public RegisterAttribute (string name, CustomAttribute originAttribute)
		{
			this.name = name;
			OriginAttribute = originAttribute;
		}

		public RegisterAttribute (string name, string signature, string connector, CustomAttribute originAttribute)
			: this (name, originAttribute)
		{
			this.connector = connector;
			this.signature = signature;
		}

		public CustomAttribute OriginAttribute { get; }

		public string Connector {
			get { return connector; }
			set { connector = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Signature {
			get { return signature; }
			set { signature = value; }
		}

		public bool DoNotGenerateAcw {get; set;}

		public int ApiSince {get; set;}
	}
}
