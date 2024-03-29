using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Java.Interop;

partial class JniEnvironment {
	partial class InstanceMethods {
		public static unsafe JniMethodInfo GetMethodID (JniObjectReference type, ReadOnlySpan<byte> name, ReadOnlySpan<byte> signature)
		{
			if (!type.IsValid)
				throw new ArgumentException ("Handle must be valid.", "type");

			IntPtr env = JniEnvironment.EnvironmentPointer;
			IntPtr method;
			IntPtr thrown;
			fixed (void* name_ptr       = &MemoryMarshal.GetReference (name))
			fixed (void* signature_ptr  = &MemoryMarshal.GetReference (signature)) {
				method  = JniNativeMethods.GetMethodID (env, type.Handle, (IntPtr) name_ptr, (IntPtr) signature_ptr);
				thrown  = JniNativeMethods.ExceptionOccurred (env);
			}

			Exception? __e = JniEnvironment.GetExceptionForLastThrowable (thrown);
			if (__e != null)
				ExceptionDispatchInfo.Capture (__e).Throw ();

			if (method == IntPtr.Zero)
				throw new InvalidOperationException ("Should not be reached; `GetMethodID` should have thrown!");

#if DEBUG
			return new JniMethodInfo (name.ToString (), signature.ToString (), method, isStatic: false);
#else   // DEBUG
			return new JniMethodInfo (null!, null!, method, isStatic: false);
#endif  // DEBUG
		}
	}
}
