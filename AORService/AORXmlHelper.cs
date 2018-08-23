using FTN.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AORService
{
	public class AORXmlHelper
	{
		public bool CreatePermXml(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Permission>));
			TextWriter writer = new StreamWriter(filename);

			List<Permission> perms = new List<Permission>(8);
			Permission p1 = new Permission("DNA_PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
			Permission p2 = new Permission("DNA_PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
			Permission p3 = new Permission("DNA_PermissionViewSystem", "Permission to view content of AORViewer");
			Permission p4 = new Permission("DNA_PermissionSystemAdministration", "Permission to view system settings in AORViewer");
			Permission p5 = new Permission("DNA_PermissionViewSecurity", "Permission to view security content of AORViewer");
			Permission p6 = new Permission("DNA_PermissionSecurityAdministration", "Permission to edit security content of AORViewer");
			Permission p7 = new Permission("DNA_PermissionViewAdministration", "Permission to edit security content of AORViewer");
			Permission p8 = new Permission("DNA_PermissionViewSCADA", "Permission to view content operating under SCADA system.");
			perms.AddRange(new List<Permission>() { p1, p2, p3, p4, p5, p6, p7, p8 });

			try
			{
				serializer.Serialize(writer, perms);
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
