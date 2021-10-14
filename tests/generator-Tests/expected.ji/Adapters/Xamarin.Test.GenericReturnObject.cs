using System;
using System.Collections.Generic;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='GenericReturnObject']"
	[global::Java.Interop.JniTypeSignature ("xamarin/test/GenericReturnObject", GenerateJavaPeer=false)]
	public partial class GenericReturnObject : global::Java.Lang.Object {
		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/GenericReturnObject", typeof (GenericReturnObject));

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected GenericReturnObject (ref JniObjectReference reference, JniObjectReferenceOptions options) : base (ref reference, options)
		{
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='GenericReturnObject']/method[@name='GenericReturn' and count(parameter)=0]"
		public virtual unsafe global::Xamarin.Test.AdapterView GenericReturn ()
		{
			const string __id = "GenericReturn.()Lxamarin/test/AdapterView;";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualObjectMethod (__id, this, null);
				return global::Java.Interop.JniEnvironment.Runtime.ValueManager.GetValue<global::Xamarin.Test.AdapterView> (ref __rm, JniObjectReferenceOptions.CopyAndDispose);
			} finally {
			}
		}

	}
}
