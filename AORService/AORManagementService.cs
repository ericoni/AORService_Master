using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.AORContract;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.ServiceModel.Description;
using System.Security.Principal;
using ActiveAORCache.Helpers;
using AORCommon.Principal;
using System.ServiceModel.Channels;

namespace AORService
{ 
	// [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	/// <summary>
	/// Naci ulogu AOR management service (  radi  AOR window i setovanje principala (login) ? )
	/// Sredi ime servisa i kontrakata.
	/// 
	/// Ovaj radi context i evaluate.
	/// </summary>

	public class AORManagementService : IDisposable
	{
		ServiceHost host = null;
		NetTcpBinding binding = null;
		AORManagement aorManagement = null;
		//AOREventAlarmChannel eventProxy = null; // to do odkomentarisi
		string address = "net.tcp://localhost:10038/IAORManagement";  

		public AORManagementService()
		{
			binding = new NetTcpBinding();
			aorManagement = new AORManagement();

			//eventProxy = new AOREventAlarmChannel();
			//eventProxy.Test();

			InitializeHosts();
		}
		private void InitializeHosts()
		{
			host = new ServiceHost(typeof(AORManagement));
		}
		public void Dispose()
		{
			CloseHosts();
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			StartHosts();
		}

		private void StartHosts()
		{
			if (host == null)
			{
				throw new Exception("AOR LoginService can not be opened because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IAORManagement), binding, address);

				List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
				policies.Add(new CustomAuthorizationPolicy());

				host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
				host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

				host.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF service {0} is ready.", host.Description.Name);
			Console.WriteLine(message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine(address);

			message = "The AOR LoginService is started.";
			Console.WriteLine("{0}", message);

			Console.WriteLine("************************************************************************AOR Login started", message);
		}

		public Binding GetBinding()
		{
			WSHttpBinding binding = new WSHttpBinding(SecurityMode.Message); // bio je message
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			binding.Security.Mode = SecurityMode.Transport; // ova linija nije bila
			
			return binding;
		}
		private void CloseHosts()
		{
			if (host == null)
			{
				throw new ArgumentNullException("AOR LoginService can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "AOR LoginService is closed.";
			Console.WriteLine("\n\n{0}", message);
		}

		class CustomAuthorizationPolicy : IAuthorizationPolicy
		{
			string id = Guid.NewGuid().ToString();

			public string Id
			{
				get { return this.id; }
			}

			public ClaimSet Issuer
			{
				get { return ClaimSet.System; }
			}

			/// <summary>
			/// The evaluate method, if it returns true it stops other authorization polices from being evaluated 
			/// (there can be multiple authorization policies, that will become clear when we create the configuration to add our own configuration policy).
			/// Source: https://www.blinkingcaret.com/2016/03/02/wcf-security-survival-guide-part-2-authorization/
			/// </summary>
			/// <param name="context"></param>
			/// <param name="state"></param>
			/// <returns> False znaci da za svaku metodu, svaki poziv, dva puta se pozove Evalute(). Kad se stavi true bude 1 poziv prije svake metode. </returns>
			public bool Evaluate(EvaluationContext context, ref object state)
			{
				object obj;
				if (!context.Properties.TryGetValue("Identities", out obj))
					return false;

				IList<IIdentity> identities = obj as IList<IIdentity>;
				if (obj == null || identities.Count <= 0)
					return false;

				String name = identities[0].Name;
				int backslashLastIndex = name.LastIndexOf('\\');

				string[] assignedAreas = AORCacheConfigurations.GetAORAreasForUsername(name.Substring(backslashLastIndex + 1));

				context.Properties["Principal"] = new CustomPrincipal(identities[0], "perica", assignedAreas);

				//return EvaluationResult(assignedAreas);
				return true;
			}

			private bool EvaluationResult(string[] assignedAreas)
			{
				int assignedAreasCount = assignedAreas.Count();
				if (assignedAreasCount == 0 || assignedAreas == null)
				{
					throw new ArgumentException("EvaluationResult threw an exception.");
				}

				if (assignedAreasCount == 1)//only default area has been assigned
				{
					if (assignedAreas[0].Equals("None"))
					{
						return false;
					}
				}

				return true;
			}
		}
	}
}
