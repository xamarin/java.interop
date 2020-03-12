using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']"
	[global::Android.Runtime.Register ("xamarin/test/Blues", DoNotGenerateAcw=true)]
	public abstract partial class Blues : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/field[@name='BLUE']"
		[Register ("BLUE")]
		public const int Blue__ = (int) -16776961;


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/field[@name='blue']"
		[Register ("blue")]
		public int Blue_ {
			get {
				const string __id = "blue.I";

				var __v = _members.InstanceFields.GetInt32Value (__id, this);
				return __v;
			}
			set {
				const string __id = "blue.I";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}
		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/Blues", typeof (Blues));
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

		protected Blues (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		public static unsafe int Blue {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/method[@name='getBlue' and count(parameter)=0]"
			[Register ("getBlue", "()I", "")]
			get {
				const string __id = "getBlue.()I";
				try {
					var __rm = _members.StaticMethods.InvokeInt32Method (__id, null);
					return __rm;
				} finally {
				}
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/Blues", DoNotGenerateAcw=true)]
	internal partial class BluesInvoker : Blues {

		public BluesInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/Blues", typeof (BluesInvoker));

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

	}

}
