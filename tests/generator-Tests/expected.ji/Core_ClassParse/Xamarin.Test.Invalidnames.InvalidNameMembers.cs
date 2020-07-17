using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test.Invalidnames {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test.invalidnames']/class[@name='InvalidNameMembers']"
	[global::Android.Runtime.Register ("xamarin/test/invalidnames/InvalidNameMembers", DoNotGenerateAcw=true)]
	public partial class InvalidNameMembers : global::Java.Lang.Object {

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/invalidnames/InvalidNameMembers", typeof (InvalidNameMembers));
		internal static new IntPtr class_ref {
			get {
				return _members.JniPeerType.PeerReference.Handle;
			}
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected InvalidNameMembers (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

	}
}
