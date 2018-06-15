using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Services.NetworkModelService.Database.Data
{
	public class DeltaItem
	{
		private int id;
		private List<ResourceDescription> insertOps = new List<ResourceDescription>();
		private List<ResourceDescription> deleteOps = new List<ResourceDescription>();
		private List<ResourceDescription> updateOps = new List<ResourceDescription>();

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public List<ResourceDescription> InsertOps
		{
			get { return insertOps; }
			set { insertOps = value; }
		}

		public List<ResourceDescription> DeleteOps
		{
			get { return deleteOps; }
			set { deleteOps = value; }
		}

		public List<ResourceDescription> UpdateOps
		{
			get { return updateOps; }
			set { updateOps = value; }
		}
	}
}
