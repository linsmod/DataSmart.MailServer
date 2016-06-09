using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace DataSmart.MailServer.Management
{
	internal class Utils
	{
		public static DataSet DecompressDataSet(Stream source)
		{
			source.Position = 0L;
			GZipStream gZipStream = new GZipStream(source, CompressionMode.Decompress);
			MemoryStream memoryStream = new MemoryStream();
			byte[] array = new byte[8000];
			for (int i = gZipStream.Read(array, 0, array.Length); i > 0; i = gZipStream.Read(array, 0, array.Length))
			{
				memoryStream.Write(array, 0, i);
			}
			memoryStream.Position = 0L;
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(memoryStream);
			return dataSet;
		}
	}
}
