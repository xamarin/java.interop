using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Google.Composable {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.com.google.compose']/class[@name='MyClass']"
	[global::Android.Runtime.Register ("com/com/google/compose/MyClass", DoNotGenerateAcw=true)]
	public partial class MyClass : global::Java.Lang.Object {

		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("com/com/google/compose/MyClass", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (MyClass); }
		}

		protected MyClass (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

	}
}
