using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ObjectModel
{
	[Serializable]
	public class Method
	{
		public Method (string name, string javaName, string apiAdded, List<Tuple<string, List<string>>> arguments, string returnType, List<string> returnValues)
		{
			Name = name;
			JavaName = javaName;
			ApiAdded = apiAdded;
			ReturnType = returnType;
			ReturnValues = returnValues;
			Arguments = arguments;
		}

		public string Name { get; set; }

		public List<Tuple<string, List<string>>> Arguments { get; set; }

		public string ReturnType { get; set; }

		public List<string> ReturnValues { get; set; }

		public string JavaName { get; set; }

		public string ApiAdded { get; set; }
	}
}
