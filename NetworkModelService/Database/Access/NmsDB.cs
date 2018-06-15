using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Services.NetworkModelService.Database.Data;
using FTN.Common;

namespace FTN.Services.NetworkModelService.Database.Access
{
	public class NmsDB : INmsDB
	{
		private static INmsDB myDB;

		public static INmsDB Instance
		{
			get
			{
				if (myDB == null)
					myDB = new NmsDB();

				return myDB;
			}
			set
			{
				if (myDB == null)
					myDB = value;
			}
		}

		public bool AddDelta(Delta delta)
		{
			using (var access = new AccessDB())
			{
				DeltaItem deltaItem = new DeltaItem();

				List<ResourceDescription> activeOps = new List<ResourceDescription>();

				if (delta.InsertOperations.Count != 0)
				{
					deltaItem.InsertOps = delta.InsertOperations;
					activeOps = delta.InsertOperations;
				}
				else if (delta.UpdateOperations.Count != 0)
				{
					deltaItem.UpdateOps = delta.UpdateOperations;
					activeOps = delta.UpdateOperations;
				}
				else if (delta.DeleteOperations.Count != 0)
				{
					deltaItem.DeleteOps = delta.DeleteOperations;
					activeOps = delta.DeleteOperations;
				}
				else
				{
					throw new InvalidOperationException("All Delta's operations are empty.");
				}

				try
				{

					foreach (var rd in activeOps)
					{
						access.RDescriptions.Add(rd);

						foreach (var prop in rd.Properties)
						{
							access.Properties.Add(prop);
						}
					}

					access.Deltas.Add(deltaItem);
				}
				catch (Exception e )
				{
					throw e;
				}
				
				int i = access.SaveChanges();

				if (i > 0)
					return true;
				return false;
			}
		}

		public List<Delta> ReadDeltas()
		{
			using (var access = new AccessDB())
			{
				var resourceDescriptions = (from rd in access.Deltas.Include("RDescriptions").Include("Properties")
									  select rd.InsertOps).ToList();

				var properties = (from p in access.Properties
							 select p).ToList();

				var returnValue = (from r in resourceDescriptions
						   select new Delta() { InsertOperations = r }).ToList();

				if (returnValue.Count == 0)
				{
					return new List<Delta>();
				}
				else
				{
					return returnValue;
				}
			}
		}
	}
}
