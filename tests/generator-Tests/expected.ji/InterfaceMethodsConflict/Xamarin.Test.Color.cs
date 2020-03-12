using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

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
		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/Color", typeof (Color));
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

		protected Color (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer) {}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/constructor[@name='Color' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe Color ()
			: base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
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

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/method[@name='blue' and count(parameter)=0]"
		[Register ("blue", "()F", "GetBlueHandler")]
		public virtual unsafe float Blue ()
		{
			const string __id = "blue.()F";
			try {
				var __rm = _members.InstanceMethods.InvokeVirtualSingleMethod (__id, this, null);
				return __rm;
			} finally {
			}
		}

		// Metadata.xml XPath method reference: path="/api/package[@name='xamarin.test']/class[@name='Color']/method[@name='blue' and count(parameter)=1 and parameter[1][@type='long']]"
		[Register ("blue", "(J)F", "")]
		public static unsafe float Blue (long color)
		{
			const string __id = "blue.(J)F";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue (color);
				var __rm = _members.StaticMethods.InvokeSingleMethod (__id, __args);
				return __rm;
			} finally {
			}
		}

	}
}
