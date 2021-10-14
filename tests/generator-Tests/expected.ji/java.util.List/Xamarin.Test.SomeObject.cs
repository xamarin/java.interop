using System;
using System.Collections.Generic;
using Java.Interop;

namespace Xamarin.Test {

	// Metadata.xml XPath class reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']"
	[global::Java.Interop.JniTypeSignature ("xamarin/test/SomeObject", GenerateJavaPeer=false)]
	public partial class SomeObject : global::Java.Lang.Object {

		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myStrings']"
		public global::System.Collections.Generic.IList<string> MyStrings {
			get {
				const string __id = "myStrings.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<string>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "myStrings.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<string>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myInts']"
		public global::System.Collections.Generic.IList<int> MyInts {
			get {
				const string __id = "myInts.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<int>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "myInts.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<int>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mybools']"
		public global::System.Collections.Generic.IList<bool> Mybools {
			get {
				const string __id = "mybools.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<bool>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "mybools.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<bool>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myObjects']"
		public global::System.Collections.Generic.IList<global::Java.Lang.Object> MyObjects {
			get {
				const string __id = "myObjects.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<global::Java.Lang.Object>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "myObjects.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<global::Java.Lang.Object>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='myfloats']"
		public global::System.Collections.Generic.IList<float> Myfloats {
			get {
				const string __id = "myfloats.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<float>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "myfloats.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<float>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mydoubles']"
		public global::System.Collections.Generic.IList<double> Mydoubles {
			get {
				const string __id = "mydoubles.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<double>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "mydoubles.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<double>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}


		// Metadata.xml XPath field reference: path="/api/package[@name='xamarin.test']/class[@name='SomeObject']/field[@name='mylongs']"
		public global::System.Collections.Generic.IList<long> Mylongs {
			get {
				const string __id = "mylongs.Ljava/util/List;";

				var __v = _members.InstanceFields.GetObjectValue (__id, this);
				return global::Android.Runtime.JavaList<long>.FromJniHandle (__v.Handle, JniHandleOwnership.TransferLocalRef);
			}
			set {
				const string __id = "mylongs.Ljava/util/List;";

				IntPtr native_value = global::Android.Runtime.JavaList<long>.ToLocalJniHandle (value);
				try {
					_members.InstanceFields.SetValue (__id, this, new JniObjectReference (native_value));
				} finally {
					JNIEnv.DeleteLocalRef (native_value);
				}
			}
		}

		static readonly JniPeerMembers _members = new JniPeerMembers ("xamarin/test/SomeObject", typeof (SomeObject));

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected SomeObject (ref JniObjectReference reference, JniObjectReferenceOptions options) : base (ref reference, options)
		{
		}

	}
}
