using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ObjectModel
{
	[Serializable]
	public class ConstantField
	{
		public ConstantField (string javaNamespace, string name, string javaName, string apiAdded, string value)
		{
			JavaNamespace = javaNamespace;
			Name = name;
			JavaName = javaName;
			ApiAdded = apiAdded;
			Value = value;
		}

		public string JavaNamespace { get; set; }

		public string Name { get; set; }

		public string JavaName { get; set; }

		public string ApiAdded { get; set; }

		public string Value { get; set; }
	}
}
