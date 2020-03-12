using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']"
	[global::Android.Runtime.Register ("xamarin/test/Blues", DoNotGenerateAcw=true)]
	public abstract partial class Blues : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/field[@name='BLUE']"
		[Register ("BLUE")]
		public const int Blue__ = (int) -16776961;

		static IntPtr blue_jfieldId;

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/field[@name='blue']"
		[Register ("blue")]
		public int Blue_ {
			get {
				if (blue_jfieldId == IntPtr.Zero)
					blue_jfieldId = JNIEnv.GetFieldID (class_ref, "blue", "I");
				return JNIEnv.GetIntField (((global::Java.Lang.Object) this).Handle, blue_jfieldId);
			}
			set {
				if (blue_jfieldId == IntPtr.Zero)
					blue_jfieldId = JNIEnv.GetFieldID (class_ref, "blue", "I");
				try {
					JNIEnv.SetField (((global::Java.Lang.Object) this).Handle, blue_jfieldId, value);
				} finally {
				}
			}
		}
		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/Blues", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (Blues); }
		}

		protected Blues (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_getBlue;
		public static unsafe int Blue {
			// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Blues']/method[@name='getBlue' and count(parameter)=0]"
			[Register ("getBlue", "()I", "")]
			get {
				if (id_getBlue == IntPtr.Zero)
					id_getBlue = JNIEnv.GetStaticMethodID (class_ref, "getBlue", "()I");
				try {
					return JNIEnv.CallStaticIntMethod  (class_ref, id_getBlue);
				} finally {
				}
			}
		}

	}

	[global::Android.Runtime.Register ("xamarin/test/Blues", DoNotGenerateAcw=true)]
	internal partial class BluesInvoker : Blues {

		public BluesInvoker (IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer) {}

		protected override global::System.Type ThresholdType {
			get { return typeof (BluesInvoker); }
		}

	}

}
