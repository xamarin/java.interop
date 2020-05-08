using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Android.Tools.ObjectModel
{
	[Serializable]
	public class AndroidType
	{
		public string Name { get; set; }

		public string FullName { get; set; }

		public string PackageName { get; set; }

		public string JavaName { get; set; }

		public string OriginalJavaName { get; set; }

		public string JavaPackageName { get; set; }

		public Uri Url { get; set; }

		public AndroidObjectType AndroidObjectType { get; set; }

		public List<AndroidType> Types { get; set; } = new List<AndroidType> ();

		public List<ConstantField> ConstantFields { get; set; } = new List<ConstantField> ();

		public List<Method> Methods { get; set; } = new List<Method> ();

		public bool ParentObjectCorrectlyIdentified { get; set; }

		public string ApiAdded { get; set; }

		public void AddType (AndroidType androidType)
		{
			Types.Add (androidType);
		}
	}
}
