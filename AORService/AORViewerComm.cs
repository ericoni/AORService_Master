using FTN.Common.AORContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.Model;

namespace AORService
{
	public class AORViewerComm : IAORViewerCommunication
	{
		/// <summary>
		/// Helper class used for XML (de)serialization
		/// </summary>
		private AORXmlHelper aorXmlHelper;

		public AORViewerComm()
		{
			aorXmlHelper = new AORXmlHelper();
		}

		public List<Permission> GetAllPermissions()
		{
			return aorXmlHelper.ReadPermXml();
		}

		public void SerializeDNAs()
		{
			aorXmlHelper.CreatePermXml();
		}

		public List<DNAAuthority> GetAllDNAs()
		{
			return aorXmlHelper.ReadDNAFromXml();
		}
	}
}
