using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Java.Interop;

partial class JniEnvironment {
	partial class StaticFields {

		public static unsafe JniFieldInfo GetStaticFieldID (JniObjectReference type, ReadOnlySpan<byte> name, ReadOnlySpan<byte> signature)
		{
			if (!type.IsValid)
				throw new ArgumentException ("Handle must be valid.", "type");

			IntPtr env = JniEnvironment.EnvironmentPointer;
			IntPtr field;
			IntPtr thrown;
			fixed (void* name_ptr       = &MemoryMarshal.GetReference (name))
			fixed (void* signature_ptr  = &MemoryMarshal.GetReference (signature)) {
				field   = JniNativeMethods.GetStaticFieldID (env, type.Handle, (IntPtr) name_ptr, (IntPtr) signature_ptr);
				thrown  = JniNativeMethods.ExceptionOccurred (env);
			}

			Exception? __e = JniEnvironment.GetExceptionForLastThrowable (thrown);
			if (__e != null)
				ExceptionDispatchInfo.Capture (__e).Throw ();

			if (field == IntPtr.Zero)
				throw new InvalidOperationException ("Should not be reached; `GetFieldID` should have thrown!");

#if DEBUG
			return new JniFieldInfo (name.ToString (), signature.ToString (), field, isStatic: false);
#else   // DEBUG
			return new JniFieldInfo (null!, null!, field, isStatic: false);
#endif  // DEBUG
		}
	}
}
