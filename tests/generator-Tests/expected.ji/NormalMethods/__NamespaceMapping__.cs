using System;

[assembly:global::Android.Runtime.NamespaceMapping (Java = "java.lang", Managed="Java.Lang")]
[assembly:global::Android.Runtime.NamespaceMapping (Java = "xamarin.test", Managed="Xamarin.Test")]

delegate int _JniMarshal_PP_I (IntPtr jnienv, IntPtr klass);
delegate IntPtr _JniMarshal_PP_L (IntPtr jnienv, IntPtr klass);
delegate void _JniMarshal_PP_V (IntPtr jnienv, IntPtr klass);
delegate IntPtr _JniMarshal_PPI_L (IntPtr jnienv, IntPtr klass, int p0);
delegate void _JniMarshal_PPL_V (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate void _JniMarshal_PPLIL_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, IntPtr p2);
delegate int _JniMarshal_PPLL_I (IntPtr jnienv, IntPtr klass, IntPtr p0, IntPtr p1);
