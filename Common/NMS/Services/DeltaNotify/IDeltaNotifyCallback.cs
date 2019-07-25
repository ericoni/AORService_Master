using System.ServiceModel;

namespace FTN.Common.Services
{
	/// <summary>
	/// Callback interface for <seealso cref="IDeltaNotify"/> interface.
	/// </summary>
	[ServiceContract]
	public interface IDeltaNotifyCallback
	{
		[OperationContract]
		void Refresh();
	}
}