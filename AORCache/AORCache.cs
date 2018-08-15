using FTN.Common.Model;
using System;
using System.Collections.Generic;
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
		const string ADLDS_Config = "ADLDS_Configuration.xml";
		public AORCache()
		{
			WriteXmlToFile();
		}

		public void SynchronizeAORConfig()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		private void WriteXmlToFile()
		{
			using (XmlWriter writer = XmlWriter.Create(ADLDS_Config)) //LEFT TO DO
			{
				writer.Settings.Indent = true;
				writer.WriteStartDocument();
				writer.WriteStartElement("ADLSD_Population");

				List<Permission> perms = new List<Permission>(5);
				Permission p1 = new Permission("DNA_AuthorityDispatcher");
				Permission p2 = new Permission("DNAAuthorityDBAdmin");
				Permission p3 = new Permission("DNA_AuthorityOperator");
				Permission p4 = new Permission("DNA_AuthorityWebManager");
				Permission p5 = new Permission("DNA_AuthorityNetworkEditor");
				perms.AddRange(new List<Permission>() { p1, p2, p3, p4, p5 });

				foreach (var p in perms)
				{
					writer.WriteStartElement("Permission");
					writer.WriteElementString("Name", p.Name);

					writer.WriteEndElement();
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}

		[Obsolete]
		private static bool Serialize<T>(T value, ref string serializeXml)
		{
			if (value == null)
			{
				return false;
			}

			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				StringWriter stringWriter = new StringWriter();
				XmlWriter writer = XmlWriter.Create(stringWriter);

				xmlSerializer.Serialize(writer, value);

				serializeXml = stringWriter.ToString();

				writer.Close();
				return true;
			}
			catch 
			{
				return false;
				
			}
		}
	}
}
