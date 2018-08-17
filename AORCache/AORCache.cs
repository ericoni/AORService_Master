using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ActiveAORCache
{
	public class AORCache : IAORCache, IDisposable
	{
		const string ADLDS_Config = "ADLDS_Configuration2.xml";
		public AORCache()
		{
			//CreatePermXml(ADLDS_Config);
			//ReadPermXml(ADLDS_Config);
			var aorCacheModel= AORCacheModel.Instance;
		}

		public void SynchronizeAORConfig()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		private void CreatePermXml(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Permission>));
			TextWriter writer = new StreamWriter(filename);

			List<Permission> perms = new List<Permission>(5);
			Permission p1 = new Permission("DNA_AuthorityDispatcher", "Some description");
			Permission p2 = new Permission("DNAAuthorityDBAdmin");
			Permission p3 = new Permission("DNA_AuthorityOperator");
			Permission p4 = new Permission("DNA_AuthorityWebManager");
			Permission p5 = new Permission("DNA_AuthorityNetworkEditor");
			perms.AddRange(new List<Permission>() { p1, p2, p3, p4, p5 });

			serializer.Serialize(writer, perms);
			writer.Close();
		}

		private void ReadPermXml(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Permission>));
			serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
			serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

			using (FileStream fs = new FileStream(filename, FileMode.Open))
			{
				List<Permission> permList;
				permList = (List<Permission>)serializer.Deserialize(fs);
			}
			int a = 5;
			a++;
		}

		private void serializer_UnknownNode (object sender, XmlNodeEventArgs e)
		{
			Trace.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
		}

		private void serializer_UnknownAttribute (object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Trace.WriteLine("Unknown attribute " +
			attr.Name + "='" + attr.Value + "'");
		}
	}
}
