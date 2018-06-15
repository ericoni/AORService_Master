using System;
using System.Runtime.Serialization;

namespace FTN.Common
{	
	public enum FuelType : short
	{
		Coal = 0,
		Oil = 1,
		Gas = 2,
		Lignite = 3,
		Wind = 4,
		Sun = 5
	}


	[Serializable]
	[DataContract]
    public enum PowerType : short
	{
        [EnumMember]
        Active = 0,
        [EnumMember]
        Reactive = 1
	}
}
