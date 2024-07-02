#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace Java.Interop {

	public static class JavaPeerableExtensions {

		public static string? GetJniTypeName (this IJavaPeerable self)
		{
			JniPeerMembers.AssertSelf (self);
			return JniEnvironment.Types.GetJniTypeNameFromInstance (self.PeerReference);
		}

		public static bool TryJavaCast<
				[DynamicallyAccessedMembers (JavaObject.ConstructorsAndInterfaces)]
				TResult
		> (this IJavaPeerable? self, [NotNullWhen (true)] out TResult? result)
			where TResult : class, IJavaPeerable
		{
			result = JavaAs<TResult> (self);
			return result != null;
		}

		public static TResult? JavaAs<
				[DynamicallyAccessedMembers (JavaObject.ConstructorsAndInterfaces)]
				TResult
		> (this IJavaPeerable? self)
			where TResult : class, IJavaPeerable
		{
			if (self == null) {
				return null;
			}

			if (self is TResult result) {
				return result;
			}

			if (!self.PeerReference.IsValid) {
				throw new ObjectDisposedException (self.GetType ().FullName);
			}

			var r = self.PeerReference;
			return JniEnvironment.Runtime.ValueManager.CreatePeer (
					ref r, JniObjectReferenceOptions.Copy,
					targetType: typeof (TResult))
				as TResult;
		}
	}
}
