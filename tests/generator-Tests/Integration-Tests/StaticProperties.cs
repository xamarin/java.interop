﻿using System;
using NUnit.Framework;

namespace generatortests
{
	[TestFixture]
	public class StaticProperties : BaseGeneratorTest
	{
		[Test]
		public void GeneratedOK ()
		{
			RunAllTargets (
					outputRelativePath:     "StaticProperties",
					apiDescriptionFile:     "expected.ji/StaticProperties/StaticProperties.xml",
					expectedRelativePath:   "StaticProperties");
		}
	}
}

