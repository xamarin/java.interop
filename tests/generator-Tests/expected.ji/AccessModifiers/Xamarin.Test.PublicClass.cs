//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable restore
using System;
using System.Collections.Generic;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='PublicClass']"
	[global::Java.Interop.JniTypeSignature ("xamarin/test/PublicClass", GenerateJavaPeer=false)]
	public partial class PublicClass : global::Java.Lang.Object {
		// Metadata.xml XPath interface reference: path="/api/package[@name='xamarin.test']/interface[@name='PublicClass.ProtectedInterface']"
		[global::Java.Interop.JniTypeSignature ("xamarin/test/PublicClass$ProtectedInterface", GenerateJavaPeer=false)]
		protected internal partial interface IProtectedInterface : IJavaPeerable {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/interface[@name='PublicClass.ProtectedInterface']/method[@name='foo' and count(parameter)=0]"
			[global::Java.Interop.JniMethodSignature ("foo", "()V")]
			void Foo ();

		}

		[global::Java.Interop.JniTypeSignature ("xamarin/test/PublicClass$ProtectedInterface", GenerateJavaPeer=false)]
		internal partial class IProtectedInterfaceInvoker : global::Java.Lang.Object, IProtectedInterface {
			[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
			[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
			public override global::Java.Interop.JniPeerMembers JniPeerMembers {
				get { return _members_xamarin_test_PublicClass_ProtectedInterface; }
			}

			static readonly JniPeerMembers _members_xamarin_test_PublicClass_ProtectedInterface = new JniPeerMembers ("xamarin/test/PublicClass$ProtectedInterface", typeof (IProtectedInterfaceInvoker));

			public IProtectedInterfaceInvoker (ref JniObjectReference reference, JniObjectReferenceOptions options) : base (ref reference, options)
			{
			}

			public unsafe void Foo ()
			{
				const string __id = "foo.()V";
				try {
					_members_xamarin_test_PublicClass_ProtectedInterface.InstanceMethods.InvokeAbstractVoidMethod (__id, this, null);
				} finally {
				}
			}

		}

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/PublicClass", typeof (PublicClass));

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected PublicClass (ref JniObjectReference reference, JniObjectReferenceOptions options) : base (ref reference, options)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='PublicClass']/constructor[@name='PublicClass' and count(parameter)=0]"
		[global::Java.Interop.JniConstructorSignature ("()V")]
		public unsafe PublicClass () : base (ref *InvalidJniObjectReference, JniObjectReferenceOptions.None)
		{
			const string __id = "()V";

			if (PeerReference.IsValid)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				Construct (ref __r, JniObjectReferenceOptions.CopyAndDispose);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='PublicClass']/method[@name='foo' and count(parameter)=0]"
		[global::Java.Interop.JniMethodSignature ("foo", "()V")]
		public virtual unsafe void Foo ()
		{
			const string __id = "foo.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

	}
}
