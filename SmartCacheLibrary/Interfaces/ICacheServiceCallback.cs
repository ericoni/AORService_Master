using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SmartCacheLibrary.Interfaces
{
	/// <summary>
	/// Callback interface for <seealso cref="ICacheService"/>.
	/// </summary>
	public interface ICacheServiceCallback
	{
		[OperationContract(IsOneWay = true)] // Govori da li je neblokirajuci poziv, kada se odradi poziv
		void NotifyUserOfCache(string userName, /*string cache*/Cache cache);

	}
}
