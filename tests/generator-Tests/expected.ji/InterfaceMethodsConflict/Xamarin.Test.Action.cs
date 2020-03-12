using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='Action']"
	[global::Android.Runtime.Register ("xamarin/test/Action", DoNotGenerateAcw=true)]
	public partial class Action : global::Java.Lang.Object {



		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/field[@name='icon']"
		[Register ("icon")]
		public int Icon_ {
			get {
				const string __id = "icon.I";

				var __v = _members.InstanceFields.GetInt32Value (__id, this);
				return __v;
			}
			set {
				const string __id = "icon.I";

				try {
					_members.InstanceFields.SetValue (__id, this, value);
				} finally {
				}
			}
		}

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/field[@name='SEMANTIC_ACTION_ARCHIVE']"
		[Register ("SEMANTIC_ACTION_ARCHIVE")]
		public const int SemanticActionArchive = (int) 5;
		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/Action", typeof (Action));
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

		protected Action (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static Delegate cb_getIcon;
#pragma warning disable 0169
		static Delegate GetGetIconHandler ()
		{
			if (cb_getIcon == null)
				cb_getIcon = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, int>) n_GetIcon);
			return cb_getIcon;
		}

		static int n_GetIcon (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.Action __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.Action> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Icon;
		}
#pragma warning restore 0169

		public virtual unsafe int Icon {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/method[@name='getIcon' and count(parameter)=0]"
			[Register ("getIcon", "()I", "GetGetIconHandler")]
			get {
				const string __id = "getIcon.()I";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, null);
					return __rm;
				} finally {
				}
			}
		}

	}
}
