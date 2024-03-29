using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Java.Interop;

partial class JniEnvironment {
	partial class StaticMethods {

		public static unsafe JniMethodInfo GetStaticMethodID (JniObjectReference type, ReadOnlySpan<byte> name, ReadOnlySpan<byte> signature)
		{
			if (!type.IsValid)
				throw new ArgumentException ("Handle must be valid.", "type");

			IntPtr env = JniEnvironment.EnvironmentPointer;
			IntPtr method;
			IntPtr thrown;
			fixed (void* name_ptr       = &MemoryMarshal.GetReference (name))
			fixed (void* signature_ptr  = &MemoryMarshal.GetReference (signature)) {
				method  = JniNativeMethods.GetStaticMethodID (env, type.Handle, (IntPtr) name_ptr, (IntPtr) signature_ptr);
				thrown  = JniNativeMethods.ExceptionOccurred (env);
			}

			Exception? __e = JniEnvironment.GetExceptionForLastThrowable (thrown);
			if (__e != null)
				ExceptionDispatchInfo.Capture (__e).Throw ();

			if (method == IntPtr.Zero)
				throw new InvalidOperationException ("Should not be reached; `GetStaticMethodID` should have thrown!");

#if DEBUG
			return new JniMethodInfo (name.ToString (), signature.ToString (), method, isStatic: true);
#else   // DEBUG
			return new JniMethodInfo (null!, null!, method, isStatic: true);
#endif  // DEBUG
		}

		internal static unsafe bool TryGetStaticMethod (
                JniObjectReference type,
                ReadOnlySpan<byte> name,
                ReadOnlySpan<byte> signature,
                [NotNullWhen(true)]
                out JniMethodInfo? method)
		{
            method = null;

			if (!type.IsValid)
				throw new ArgumentException ("Handle must be valid.", "type");

			IntPtr env = JniEnvironment.EnvironmentPointer;
			IntPtr id;
			IntPtr thrown;
			fixed (void* name_ptr       = &MemoryMarshal.GetReference (name))
			fixed (void* signature_ptr  = &MemoryMarshal.GetReference (signature)) {
				id      = JniNativeMethods.GetStaticMethodID (env, type.Handle, (IntPtr) name_ptr, (IntPtr) signature_ptr);
				thrown  = JniNativeMethods.ExceptionOccurred (env);
			}

            if (thrown != IntPtr.Zero) {
                JniNativeMethods.ExceptionClear (env);
				JniEnvironment.References.RawDeleteLocalRef (env, thrown);
                thrown = IntPtr.Zero;
                return false;
            }

			Debug.Assert (id != IntPtr.Zero);
            if (id == IntPtr.Zero) {
                return false;
            }

#if DEBUG
			method = new JniMethodInfo (name.ToString (), signature.ToString (), id, isStatic: true);
#else   // DEBUG
			method = new JniMethodInfo (null!, null!, id, isStatic: true);
#endif  // DEBUG

            return true;
		}
	}
}
