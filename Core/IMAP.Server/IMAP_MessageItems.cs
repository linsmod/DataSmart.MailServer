using System;

namespace System.NetworkToolkit.IMAP.Server
{
	public enum IMAP_MessageItems
	{
		None,
		Header = 2,
		Envelope = 4,
		BodyStructure = 8,
		Message = 16
	}
}
