using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using NUnit.Framework;

using Java.Interop.Tools.JavaSource;

using Irony;
using Irony.Parsing;

namespace Java.Interop.Tools.JavaSource.Tests
{
	[TestFixture]
	public class SourceJavadocToXmldocGrammarHtmlBnfTermsTests : SourceJavadocToXmldocGrammarFixture {

		[Test]
		public void PBlockDeclaration ()
		{
			var p = CreateParser (g => g.HtmlTerms.PBlockDeclaration);

			var r = p.Parse ("<p>paragraph text\nand more!");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<para>paragraph text\nand more!</para>".Replace ("\n", Environment.NewLine),
					r.Root.AstNode.ToString ());

			r = p.Parse ("<p>r= {@code Object} and following {@literal A<B>C}   text</p>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<para>r= <c>Object</c> and following A&lt;B&gt;C   text</para>", r.Root.AstNode.ToString ());

			r = p.Parse("<p>r= <em>unknown</em> text");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<para>r= &lt;em&gt;unknown&lt;/em&gt; text</para>", r.Root.AstNode.ToString ());
		}

		[Test]
		public void PreBlockDeclaration ()
		{
			var p = CreateParser (g => g.HtmlTerms.PreBlockDeclaration);

			var r = p.Parse ("<pre>this @contains <arbitrary/> text.</pre>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<code lang=\"text/java\">this @contains &lt;arbitrary/&gt; text.</code>",
					r.Root.AstNode.ToString ());

			r = p.Parse ("<pre class=\"prettyprint\">ColorSpace cs = ColorSpace.get(ColorSpace.Named.DCI_P3);");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ($"<code lang=\"text/java\">ColorSpace cs = ColorSpace.get(ColorSpace.Named.DCI_P3);</code>",
					r.Root.AstNode.ToString ());
		}

		[Test]
		public void HyperLinkDeclaration ()
		{
			var p = CreateParser (g => g.HtmlTerms.InlineHyperLinkDeclaration);

			var r = p.Parse ("<a href=\"https://developer.android.com/guide/topics/manifest/application-element.html\">application</a>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<see href=\"https://developer.android.com/guide/topics/manifest/application-element.html\">application</see>",
					r.Root.AstNode.ToString ());

			r = p.Parse ("<a href=\"http://www.ietf.org/rfc/rfc2396.txt\">RFC&nbsp;2396: Uniform Resource Identifiers (URI): Generic Syntax</a>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("<see href=\"http://www.ietf.org/rfc/rfc2396.txt\">RFC 2396: Uniform Resource Identifiers (URI): Generic Syntax</see>",
					r.Root.AstNode.ToString ());

			r = p.Parse ("<a href=\"AutofillService.html#FieldClassification\">field classification</a>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ("field classification", r.Root.AstNode.ToString ());

			r = p.Parse ("<a href='https://material.io/guidelines/components/progress-activity.html#progress-activity-types-of-indicators'>\nProgress & activity</a>");
			Assert.IsFalse (r.HasErrors (), DumpMessages (r, p));
			Assert.AreEqual ($"<see href=\"https://material.io/guidelines/components/progress-activity.html#progress-activity-types-of-indicators\">{Environment.NewLine}Progress &amp; activity</see>",
					r.Root.AstNode.ToString ());
		}
	}
}
