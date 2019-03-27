using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace FTN.Common.Services
{
	/// <summary>
	/// Callback interface of <seealso cref="INMSSubscriber"/>.
	/// </summary>
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
