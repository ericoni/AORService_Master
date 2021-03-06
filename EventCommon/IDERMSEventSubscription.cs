﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
	/// <summary>
	/// Handles client's subscribe/unsubscribe request.
	/// Has a <seealso cref="IDERMSEventSubscriptionCallback"/> interface.
	/// </summary>
	[ServiceContract(CallbackContract = typeof(IDERMSEventSubscriptionCallback))]
	public interface IDERMSEventSubscription
	{
		[OperationContract]
		void Subscribe(List<string> areaNames);
		[OperationContract]
		void Unsubscribe();
	}
}
