using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AORManagementProxyNS;
using FTN.Common.AORContract;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.ServiceModel.Description;
using System.Security.Principal;
using ActiveAORCache.Helpers;

namespace AORService
{ 
	// [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	/// <summary>
	/// Naci ulogu AOR management service (  radi  AOR management win i setovanje principala (login) ? )
	/// Sredi imena servisa i kontrakata.
	/// 
	/// Ovaj se igra sa kontekstom i evaluate.
	/// </summary>

	public class AORManagementService : IDisposable
	{
		AORManagement aorLogin = null;
		ServiceHost host = null;
		NetTcpBinding binding = null;
		//AOREventAlarmChannel eventProxy = null; // to do odkomentarisi
		string address = "net.tcp://localhost:10038/IAORManagement";  

		public AORManagementService()
		{
			aorLogin = AORManagement.Instance;
			binding = new NetTcpBinding();

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

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("AOR LoginService can not be closed because it is not initialized.");
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

            public bool Evaluate(EvaluationContext context, ref object state)
            {
                object obj;
                if (!context.Properties.TryGetValue("Identities", out obj))
                    return false;

                IList<IIdentity> identities = obj as IList<IIdentity>;
                if (obj == null || identities.Count <= 0)
                    return false;

                //var areas = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic"); //to do cache
                var areas = new string[1] { "greskaUevaluate" };

				context.Properties["Principal"] = new CustomPrincipal(identities[0], "perica", areas);
				return true;
			}
		}

		class CustomPrincipal : IMyPrincipal
		{
			IIdentity identity;
			string username = string.Empty;
			string[] areas;

			public CustomPrincipal(IIdentity identity, string username, string[] roles)
			{
				this.identity = identity;
				this.username = username;
				this.areas = roles;
			}

			public IIdentity Identity
			{
				get { return this.identity; }
			}

			public string Username
			{
				get { return this.username; }
				set { this.username = value; }
			}

			public string[] Areas
			{
				get { return this.areas; }
				set { this.areas = value; }
			}

			public bool IsInRole(string role)
			{
				return true;
			}

			public WindowsImpersonationContext Impersonate()
			{
				throw new NotImplementedException();
			}
		}

		interface IMyPrincipal : IPrincipal
		{
			string Username { get; set; }

			WindowsImpersonationContext Impersonate();
		}
	}
}
