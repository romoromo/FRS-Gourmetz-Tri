using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceModels
{
	[DataContract]
	public class GetTokenParam
	{
        [DataMember]
		public string Username { get; set; }

		[DataMember]
		public string Password { get; set; }
	}
}
