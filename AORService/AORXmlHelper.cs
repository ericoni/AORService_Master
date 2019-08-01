using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AORViewer.Model;
using FTN.Common.AORCachedModel;

namespace AORService
{
	/// <summary>
	/// Helper class used for XML (de)serialization
	/// </summary>
	public class AORXmlHelper
	{
		public bool CreateXml(bool isDnaSerialization) 
		{
            string filename = "ADLDS_Configuration2.xml";
            TextWriter writer = new StreamWriter(filename);// 2 for Perms, 3 for DNAs. Problem kad nema nijedan fajl pa kreira oba!

            List<Permission> objList = new List<Permission>(8);
			Permission p1 = new Permission("DNA_PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
			Permission p2 = new Permission("DNA_PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
			Permission p3 = new Permission("DNA_PermissionViewSystem", "Permission to view content of AORViewer");
			Permission p4 = new Permission("DNA_PermissionSystemAdministration", "Permission to view system settings in AORViewer");
			Permission p5 = new Permission("DNA_PermissionViewSecurity", "Permission to view security content of AORViewer");
			Permission p6 = new Permission("DNA_PermissionSecurityAdministration", "Permission to edit security content of AORViewer");
			Permission p7 = new Permission("DNA_PermissionViewAdministration", "Permission to edit security content of AORViewer");
			Permission p8 = new Permission("DNA_PermissionViewSCADA", "Permission to view content operating under SCADA system.");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Permission>));
            objList.AddRange(new List<Permission>() { p1, p2, p3, p4, p5, p6, p7, p8 });

            if (isDnaSerialization) //if DNAs serizalization is required
            {
                filename = "ADLDS_Configuration3.xml";
                TextWriter writer2 = new StreamWriter(filename);

                XmlSerializer serializer2 = new XmlSerializer(typeof(List<DNAAuthority>));
                DNAAuthority dna1 = new DNAAuthority("DNA_AuthorityDispatcher", new List<Permission>() { p1, p8, p5, p7 });
                DNAAuthority dna2 = new DNAAuthority("DNA_AuthorityNetworkEditor", new List<Permission>() { p2 });
                DNAAuthority dna3 = new DNAAuthority("DNA_SCADAAdmin", "Provides complete control to all aspects of the SCADA system.", new List<Permission>() { p1, p8 });
                DNAAuthority dna4 = new DNAAuthority("DNA_Viewer", "Required for a user to access the SCADA system.  Provides non-interactive access to data according to AOR.", new List<Permission>() { p3, p5, p7, p8 });
                DNAAuthority dna5 = new DNAAuthority("DNA_DMSAdmin", new List<Permission>() { p3, p5, p7 });
                DNAAuthority dna6 = new DNAAuthority("DNA_Operator", new List<Permission>() { p1, p2, p8 });

                List<DNAAuthority> dnaObjList = new List<DNAAuthority>(6);
                dnaObjList.AddRange(new List<DNAAuthority>() { dna1, dna2, dna3, dna4, dna5, dna6 });

                return SerializeToFile(serializer2, writer2, dnaObjList, filename);
            }

            return SerializeToFile(serializer, writer, objList, filename);
		}

        private bool SerializeToFile(XmlSerializer serializer, TextWriter writer, object objList, string filename)
        {
            try
            {
                serializer.Serialize(writer, objList);
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                Trace.Write("Failed to create xml file named " + filename + ex.StackTrace);
                return false;
            }
        }

		public List<Permission> ReadPermXml(string filename = "ADLDS_Configuration2.xml")
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Permission>));

			serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
			serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
			List<Permission> permList = null;

			try
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					permList = (List<Permission>)serializer.Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				Trace.Write("Failed to read from xml file + " + ex.StackTrace);
			}
			return permList;
		}

		public List<DNAAuthority> ReadDNAFromXml(string filename = "ADLDS_Configuration3.xml")
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<DNAAuthority>));

			serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
			serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
			List<DNAAuthority> permList = null;

			try
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					permList = (List<DNAAuthority>)serializer.Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				Trace.Write("Failed to read DNAs from xml file + " + ex.StackTrace);
			}
			return permList;
		}


		private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			Trace.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
		}

		private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Trace.WriteLine("Unknown attribute " +
			attr.Name + "='" + attr.Value + "'");
		}
	}
}
