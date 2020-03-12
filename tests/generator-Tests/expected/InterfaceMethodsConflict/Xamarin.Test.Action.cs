using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='Action']"
	[global::Android.Runtime.Register ("xamarin/test/Action", DoNotGenerateAcw=true)]
	public partial class Action : global::Java.Lang.Object {


		static IntPtr icon_jfieldId;

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/field[@name='icon']"
		[Register ("icon")]
		public int Icon_ {
			get {
				if (icon_jfieldId == IntPtr.Zero)
					icon_jfieldId = JNIEnv.GetFieldID (class_ref, "icon", "I");
				return JNIEnv.GetIntField (((global::Java.Lang.Object) this).Handle, icon_jfieldId);
			}
			set {
				if (icon_jfieldId == IntPtr.Zero)
					icon_jfieldId = JNIEnv.GetFieldID (class_ref, "icon", "I");
				try {
					JNIEnv.SetField (((global::Java.Lang.Object) this).Handle, icon_jfieldId, value);
				} finally {
				}
			}
		}

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/field[@name='SEMANTIC_ACTION_ARCHIVE']"
		[Register ("SEMANTIC_ACTION_ARCHIVE")]
		public const int SemanticActionArchive = (int) 5;
		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/Action", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (Action); }
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

		static IntPtr id_getIcon;
		public virtual unsafe int Icon {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Action']/method[@name='getIcon' and count(parameter)=0]"
			[Register ("getIcon", "()I", "GetGetIconHandler")]
			get {
				if (id_getIcon == IntPtr.Zero)
					id_getIcon = JNIEnv.GetMethodID (class_ref, "getIcon", "()I");
				try {

					if (((object) this).GetType () == ThresholdType)
						return JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getIcon);
					else
						return JNIEnv.CallNonvirtualIntMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getIcon", "()I"));
				} finally {
				}
			}
		}

	}
}
