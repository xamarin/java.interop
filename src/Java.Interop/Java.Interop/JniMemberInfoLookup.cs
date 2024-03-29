using System;

namespace Java.Interop;

public ref struct JniMemberInfoLookup {
	public  string                  EncodedMember   {get; private set;}
	public  ReadOnlySpan<byte>      MemberName      {get; private set;}
	public  ReadOnlySpan<byte>      MemberSignature {get; private set;}

	[Obsolete ("Use the JniMemberInfoLookup(string, ReadOnlySpan<byte>, ReadOnlySpan<byte>) constructor.", error: true)]
	public JniMemberInfoLookup ()
	{
		throw new NotSupportedException ();
	}

	public JniMemberInfoLookup (string encodedMember, ReadOnlySpan<byte> memberName, ReadOnlySpan<byte> memberSignature)
	{
		EncodedMember   = encodedMember;
		MemberName      = memberName;
		MemberSignature = memberSignature;
	}
}
