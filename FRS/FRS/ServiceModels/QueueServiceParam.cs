using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceModels
{
	[DataContract]
	public class QueueServiceParam
    {
        [DataMember]
		public string MessageId { get; set; }

		[DataMember]
		public string CallAction { get; set; }

        [DataMember]
		public string InstitutionId { get; set; }

		[DataMember]
		public string ClinicId { get; set; }

		[DataMember]
		public string TerminalId { get; set; }

		[DataMember]
		public string TerminalName { get; set; }

		[DataMember]
		public string QueueNumber { get; set; }

		[DataMember]
		public string Timestamp { get; set; }
		
		[DataMember]
		public string Token { get; set; }

	}
}
