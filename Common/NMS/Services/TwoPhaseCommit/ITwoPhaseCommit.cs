using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace FTN.Common.Services
{
	[ServiceContract]
	public interface ITwoPhaseCommit
	{
		[OperationContract]
		bool Prepare(Delta delta);

		[OperationContract(IsOneWay = true)]
		void Commit();

		[OperationContract(IsOneWay = true)]
		void Rollback();
	}
}
