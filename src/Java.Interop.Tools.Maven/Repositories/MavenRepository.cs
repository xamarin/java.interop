using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using Java.Interop.Tools.Maven.Models;

namespace Java.Interop.Tools.Maven.Repositories;

public class MavenRepository : IMavenRepository
{
	readonly string base_url;
	readonly HttpClient client;

	public string Name { get; }

	public MavenRepository (string baseUrl, string name)
	{
		Name = name;
		base_url = baseUrl;

		client = new HttpClient {
			BaseAddress = new Uri (base_url)
		};
	}

	public bool TryGetFile (Artifact artifact, string filename, [NotNullWhen (true)] out Stream? stream)
	{
		// ex: https://repo1.maven.org/maven2/dev/chrisbanes/snapper/snapper/0.3.0/{filename}
		var file = $"{artifact.GroupId.Replace ('.', '/')}/{artifact.Id}/{artifact.Version}/{filename}";
		stream = client.GetStreamAsync (file).Result;

		return true;
	}

	public static MavenRepository Google = new MavenRepository ("https://dl.google.com/android/maven2/", "google");

	public static MavenRepository Central = new MavenRepository ("https://repo1.maven.org/maven2/", "central");
}
