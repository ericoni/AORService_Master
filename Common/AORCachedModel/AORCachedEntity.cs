using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
    [DataContract]
    [Serializable]
	public class AORCachedEntity
	{
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
		public AORCachedEntity() { }
		public AORCachedEntity(string name)
		{
			this.Name = name;
		}
		public AORCachedEntity(string description, bool nebitno)
		{
			this.Description = description;
		}
		public AORCachedEntity(string name, string description)
		{
			this.Name = name;
			this.Description = description;
		}
	}
}
