#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Java.Interop {

	[JniTypeSignature (JniTypeName, GenerateJavaPeer=false)]
	sealed class JavaProxyObject : JavaObject, IEquatable<JavaProxyObject>
	{
		internal const string JniTypeName = "net/dot/jni/internal/JavaProxyObject";

		static  readonly    JniPeerMembers                                  _members        = new JniPeerMembers (JniTypeName, typeof (JavaProxyObject));
		static  readonly    ConditionalWeakTable<object, JavaProxyObject>   CachedValues    = new ConditionalWeakTable<object, JavaProxyObject> ();

		[JniAddNativeMethodRegistrationAttribute]
		static void RegisterNativeMembers (JniNativeMethodRegistrationArguments args)
		{
			args.Registrations.Add (new JniNativeMethodRegistration ("equals",   "(Ljava/lang/Object;)Z", new EqualsMarshalMethod (Equals)));
			args.Registrations.Add (new JniNativeMethodRegistration ("hashCode", "()I",                   new GetHashCodeMarshalMethod (GetHashCode)));
			args.Registrations.Add (new JniNativeMethodRegistration ("toString", "()Ljava/lang/String;",  new ToStringMarshalMethod (ToString)));
		}

		public override JniPeerMembers JniPeerMembers {
			get {
				return _members;
			}
		}

		JavaProxyObject (object value)
		{
			if (value == null)
				throw new ArgumentNullException (nameof (value));
			Value = value;
		}

		public object Value {get; private set;}

		public override int GetHashCode ()
		{
			return Value.GetHashCode ();
		}

		public override bool Equals (object? obj)
		{
			if (obj is JavaProxyObject other)
				return object.Equals (Value, other.Value);
			return object.Equals (Value, obj);
		}

		public bool Equals (JavaProxyObject? other) => object.Equals (Value, other?.Value);

		public override string? ToString ()
		{
			return Value.ToString ();
		}

		[return: NotNullIfNotNull ("object")]
		public static JavaProxyObject? GetProxy (object value)
		{
			if (value == null)
				return null;

			lock (CachedValues) {
				if (CachedValues.TryGetValue (value, out var proxy))
					return proxy;
				proxy = new JavaProxyObject (value);
				CachedValues.Add (value, proxy);
				return proxy;
			}
		}

		// TODO: Keep in sync with the code generated by ExportedMemberBuilder
		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate    bool    EqualsMarshalMethod (IntPtr jnienv, IntPtr n_self, IntPtr n_value);
		static bool Equals (IntPtr jnienv, IntPtr n_self, IntPtr n_value)
		{
			var envp = new JniTransition (jnienv);
			try {
				var self    = (JavaProxyObject?) JniEnvironment.Runtime.ValueManager.PeekPeer (new JniObjectReference (n_self));
				var r_value = new JniObjectReference (n_value);
				var value   = JniEnvironment.Runtime.ValueManager.GetValue (ref r_value, JniObjectReferenceOptions.Copy);
				return self?.Equals (value) ?? false;
			}
			catch (Exception e) when (JniEnvironment.Runtime.ExceptionShouldTransitionToJni (e)) {
				envp.SetPendingException (e);
				return false;
			}
			finally {
				envp.Dispose ();
			}
		}

		// TODO: Keep in sync with the code generated by ExportedMemberBuilder
		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate    int     GetHashCodeMarshalMethod (IntPtr jnienv, IntPtr n_self);
		static int GetHashCode (IntPtr jnienv, IntPtr n_self)
		{
			var envp = new JniTransition (jnienv);
			try {
				var self = (JavaProxyObject?) JniEnvironment.Runtime.ValueManager.PeekPeer (new JniObjectReference (n_self));
				return self?.GetHashCode () ?? 0;
			}
			catch (Exception e) when (JniEnvironment.Runtime.ExceptionShouldTransitionToJni (e)) {
				envp.SetPendingException (e);
				return 0;
			}
			finally {
				envp.Dispose ();
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate    IntPtr  ToStringMarshalMethod (IntPtr jnienv, IntPtr n_self);
		static IntPtr ToString (IntPtr jnienv, IntPtr n_self)
		{
			var envp = new JniTransition (jnienv);
			try {
				var self    = (JavaProxyObject?) JniEnvironment.Runtime.ValueManager.PeekPeer (new JniObjectReference (n_self));
				var s       = self?.ToString ();
				var r       = JniEnvironment.Strings.NewString (s);
				try {
					return JniEnvironment.References.NewReturnToJniRef (r);
				} finally {
					JniObjectReference.Dispose (ref r);
				}
			}
			catch (Exception e) when (JniEnvironment.Runtime.ExceptionShouldTransitionToJni (e)) {
				envp.SetPendingException (e);
				return IntPtr.Zero;
			}
			finally {
				envp.Dispose ();
			}
		}
	}
}
