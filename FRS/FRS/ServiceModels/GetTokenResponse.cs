using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceModels
{
	[DataContract]
	public class GetTokenResponse
	{
        [DataMember]
		public string Token { get; set; }

		[DataMember]
		public string Expire { get; set; }
		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public string ErrorCode { get; set; }

		[DataMember]
		public string ErrorDescription { get; set; }


	}
}
