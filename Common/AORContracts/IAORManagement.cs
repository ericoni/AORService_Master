using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORCachedModel;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace FTN.Common.AORContract
{
	/// <summary>
	/// Relationship between AOR service and DERMS app. Jos ovo analizirati.
	/// </summary>
	[ServiceContract]
	public interface IAORManagement
	{
		[OperationContract]
		List<AORCachedArea> Login(string username, string password);

		[OperationContract]
		void Test();

		[OperationContract]
		List<long> GetUsersSynchronousMachines();

		//[OperationContract]
		//List<string> GetAORAreasForUsername(string username); // to do voo bi trebalo premjestiti, da ima jedinstven pristup cache-u
	}
}
