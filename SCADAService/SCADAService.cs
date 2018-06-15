using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.ServiceModel;
using FTN.Common.SCADA.Services;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using ClientProxy;
using FTN.Common.Services;
using APSCommon;

namespace SCADAService
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class SCADAService : StatefulService
	{
		private NetTcpBinding binding = null;
		private SCADASetpoint scadaSetpoint = null;
		private SCADACrunching scadaCrunching = null;
		private StatefulServiceContext context = null;
		
		public SCADAService(StatefulServiceContext context)
			: base(context)
		{
			binding = new NetTcpBinding();
			this.context = context;
			scadaSetpoint = SCADASetpoint.Instance;
		}

		/// <summary>
		/// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
		/// </summary>
		/// <remarks>
		/// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
		/// </remarks>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			var scadaSetpointListener = new ServiceReplicaListener((context) =>
				new WcfCommunicationListener<ISCADAReceiving>(
					wcfServiceObject: scadaSetpoint,
					serviceContext: context,
					//
					// The name of the endpoint configured in the ServiceManifest under the Endpoints section
					// that identifies the endpoint that the WCF ServiceHost should listen on.
					//
					endpointResourceName: "ScadaSPServiceEndpoint",

                    //
                    // Populate the binding information that you want the service to use.
                    //
                    listenerBinding: new NetTcpBinding() { ReceiveTimeout = TimeSpan.MaxValue }
                ), "SCADASetpointService"
			);

            var twoPCCallbackListener = new ServiceReplicaListener((context) =>
                new WcfCommunicationListener<ITwoPhaseCommit>(
                    wcfServiceObject: SCADAModel.Instance,
                    serviceContext: context,
                    //
                    // The name of the endpoint configured in the ServiceManifest under the Endpoints section
                    // that identifies the endpoint that the WCF ServiceHost should listen on.
                    //
                    endpointResourceName: "2PCCallbackEndpoint",

                    //
                    // Populate the binding information that you want the service to use.
                    //
                    listenerBinding: new NetTcpBinding() { ReceiveTimeout = TimeSpan.MaxValue }
                ), "2PCCallbackListener"
            );

            return new[] { scadaSetpointListener, twoPCCallbackListener };
		}

		/// <summary>
		/// This is the main entry point for your service replica.
		/// This method executes when this replica of your service becomes primary and has write status.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			NMSSubscriberClient subClient = new NMSSubscriberClient();
			subClient.Subscribed(new List<FTN.Common.DMSType>() { FTN.Common.DMSType.ANALOGVALUE }, new ServiceReference(base.Context));
			scadaCrunching = new SCADACrunching();
		}
	}
}
