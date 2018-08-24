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
		
		public AORCache()
		{
			//var aorCacheModel= AORCacheModel.Instance;
		}

		public void SynchronizeAORConfig()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}
	}
}
