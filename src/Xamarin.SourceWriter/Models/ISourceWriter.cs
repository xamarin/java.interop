using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.SourceWriter
{
	public interface ISourceWriter
	{
		void Write (CodeWriter writer);
	}
}
