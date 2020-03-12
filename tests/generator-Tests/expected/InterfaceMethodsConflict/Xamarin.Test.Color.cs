using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='Color']"
	[global::Android.Runtime.Register ("xamarin/test/Color", DoNotGenerateAcw=true)]
	public partial class Color : global::Java.Lang.Object {


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/field[@name='BLACK']"
		[Register ("BLACK")]
		public const int Black = (int) -16777216;

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/field[@name='BLUE']"
		[Register ("BLUE")]
		public const int Blue__ = (int) -16776961;
		internal static new IntPtr java_class_handle;
		internal static new IntPtr class_ref {
			get {
				return JNIEnv.FindClass ("xamarin/test/Color", ref java_class_handle);
			}
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return typeof (Color); }
		}

		protected Color (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		static IntPtr id_ctor;
		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/constructor[@name='Color' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe Color ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				if (((object) this).GetType () != typeof (Color)) {
					SetHandle (
							global::Android.Runtime.JNIEnv.StartCreateInstance (((object) this).GetType (), "()V"),
							JniHandleOwnership.TransferLocalRef);
					global::Android.Runtime.JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, "()V");
					return;
				}

				if (id_ctor == IntPtr.Zero)
					id_ctor = JNIEnv.GetMethodID (class_ref, "<init>", "()V");
				SetHandle (
						global::Android.Runtime.JNIEnv.StartCreateInstance (class_ref, id_ctor),
						JniHandleOwnership.TransferLocalRef);
				JNIEnv.FinishCreateInstance (((global::Java.Lang.Object) this).Handle, class_ref, id_ctor);
			} finally {
			}
		}

		static Delegate cb_blue;
#pragma warning disable 0169
		static Delegate GetBlueHandler ()
		{
			if (cb_blue == null)
				cb_blue = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, float>) n_Blue);
			return cb_blue;
		}

		static float n_Blue (IntPtr jnienv, IntPtr native__this)
		{
			global::Xamarin.Test.Color __this = global::Java.Lang.Object.GetObject<global::Xamarin.Test.Color> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.Blue ();
		}
#pragma warning restore 0169

		static IntPtr id_blue;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/method[@name='blue' and count(parameter)=0]"
		[Register ("blue", "()F", "GetBlueHandler")]
		public virtual unsafe float Blue ()
		{
			if (id_blue == IntPtr.Zero)
				id_blue = JNIEnv.GetMethodID (class_ref, "blue", "()F");
			try {

				if (((object) this).GetType () == ThresholdType)
					return JNIEnv.CallFloatMethod (((global::Java.Lang.Object) this).Handle, id_blue);
				else
					return JNIEnv.CallNonvirtualFloatMethod (((global::Java.Lang.Object) this).Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "blue", "()F"));
			} finally {
			}
		}

		static IntPtr id_blue_J;
		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/method[@name='blue' and count(parameter)=1 and parameter[1][@type='long']]"
		[Register ("blue", "(J)F", "")]
		public static unsafe float Blue (long color)
		{
			if (id_blue_J == IntPtr.Zero)
				id_blue_J = JNIEnv.GetStaticMethodID (class_ref, "blue", "(J)F");
			try {
				JValue* __args = stackalloc JValue [1];
				__args [0] = new JValue (color);
				return JNIEnv.CallStaticFloatMethod  (class_ref, id_blue_J, __args);
			} finally {
			}
		}

	}
}
