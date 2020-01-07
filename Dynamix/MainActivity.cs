using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dynamix
{
	[Activity (Label = "Dynamix", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private TextView textField;
		private Button theButton;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.activity_main);

			textField = FindViewById<TextView> (Resource.Id.textField);
			theButton = FindViewById<Button> (Resource.Id.theButton);

			theButton.Click += OnButtonClick;
		}

		private void OnButtonClick (object sender, EventArgs eventArgs)
		{
			dynamic calculatorConstructor = new Java.Interop.Dynamic.DynamicJavaClass ("com/xamarin/dynamix/Calculator");

			var calculator = calculatorConstructor ();

			// normal
			var answer = calculator.add (3, 6);
			textField.Text += $"\nNORMAL: 3 + 6 = {answer}";

			// use a callback
			var listener = new MyListener (textField);
			calculator.add (2, 4, (IAnswerListener) listener);
		}

		// this is to create the interface on the Java side
		[Register ("com/xamarin/dynamix/AnswerListener")]
		public interface IAnswerListener : IJavaObject { }

		public class MyListener : Java.Lang.Object, IAnswerListener
		{
			private readonly TextView textField;

			public MyListener (TextView textField)
			{
				this.textField = textField;
			}

			// export this member to Java
			[Java.Interop.Export]
			public void onCalculated (int value)
			{
				textField.Text += $"\nCALLBACK: 2 + 4 = {value}";
			}
		}
	}
}
