using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Android.Text {

	// Metadata.xml XPath interface reference: path="/api/package[@name='android.text']/interface[@name='Spannable']"
	[Register ("android/text/Spannable", "", "Android.Text.ISpannableInvoker")]
	public partial interface ISpannable : global::Android.Text.ISpanned {

	}

	[global::Android.Runtime.Register ("android/text/Spannable", DoNotGenerateAcw=true)]
	internal partial class ISpannableInvoker : global::Java.Lang.Object, ISpannable {

		static readonly JniPeerMembers _members = new JniPeerMembers ("android/text/Spannable", typeof (ISpannableInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		new IntPtr class_ref;

		public static ISpannable GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<ISpannable> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "android.text.Spannable"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public ISpannableInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

		static Delegate cb_getSpanFlags_Ljava_lang_Object_;
#pragma warning disable 0169
		static Delegate GetGetSpanFlags_Ljava_lang_Object_Handler ()
		{
			if (cb_getSpanFlags_Ljava_lang_Object_ == null)
				cb_getSpanFlags_Ljava_lang_Object_ = JNINativeWrapper.CreateDelegate ((_JniMarshal_PPL_I) n_GetSpanFlags_Ljava_lang_Object_);
			return cb_getSpanFlags_Ljava_lang_Object_;
		}

		static int n_GetSpanFlags_Ljava_lang_Object_ (IntPtr jnienv, IntPtr native__this, IntPtr native_tag)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Android.Text.ISpannable> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var tag = global::Java.Lang.Object.GetObject<global::Java.Lang.Object> (native_tag, JniHandleOwnership.DoNotTransfer);
			int __ret = (int) __this.GetSpanFlags (tag);
			return __ret;
		}
#pragma warning restore 0169

		IntPtr id_getSpanFlags_Ljava_lang_Object_;
		public unsafe global::Android.Text.SpanTypes GetSpanFlags (global::Java.Lang.Object tag)
		{
			if (id_getSpanFlags_Ljava_lang_Object_ == IntPtr.Zero)
				id_getSpanFlags_Ljava_lang_Object_ = JNIEnv.GetMethodID (class_ref, "getSpanFlags", "(Ljava/lang/Object;)I");
			JValue* __args = stackalloc JValue [1];
			__args [0] = new JValue ((tag == null) ? IntPtr.Zero : ((global::Java.Lang.Object) tag).Handle);
			var __ret = (global::Android.Text.SpanTypes) JNIEnv.CallIntMethod (((global::Java.Lang.Object) this).Handle, id_getSpanFlags_Ljava_lang_Object_, __args);
			return __ret;
		}

	}

}
