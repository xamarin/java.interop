package com.xamarin.interop.performance;

public class JavaTiming {

	public static void StaticVoidMethod ()
	{
	}

	public static int StaticIntMethod ()
	{
		return 0;
	}

	public static Object StaticObjectMethod ()
	{
		return null;
	}

	public void VirtualVoidMethod ()
	{
	}

	public int VirtualIntMethod ()
	{
		return 0;
	}

	public Object VirtualObjectMethod ()
	{
		return null;
	}

	public final void FinalVoidMethod ()
	{
	}

	public final int FinalIntMethod ()
	{
		return 0;
	}

	public final Object FinalObjectMethod ()
	{
		return null;
	}

	public int VirtualIntMethod1Args (int value)
	{
		return value;
	}

	public int VirtualIntMethod1Args (int[][][] value)
	{
		return 0;
	}

	public static void StaticVoidMethod1Args (Object obj1)
	{
	}

	public static void StaticVoidMethod2Args (Object obj1, Object obj2)
	{
	}

	public static void StaticVoidMethod3Args (Object obj1, Object obj2, Object obj3)
	{
	}

	public static void StaticVoidMethod1IArgs (int obj1)
	{
	}

	public static void StaticVoidMethod2IArgs (int obj1, int obj2)
	{
	}

	public static void StaticVoidMethod3IArgs (int obj1, int obj2, int obj3)
	{
	}

	public static Runnable CreateRunnable ()
	{
		return new Runnable () {
			public void run ()
			{
			}
		};
	}
}

