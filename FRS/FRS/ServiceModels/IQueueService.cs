using System.ServiceModel;

namespace ServiceModels
{
    [ServiceContract]
	public interface IQueueService
	{
		//[OperationContract]
		//string Test(string s);

		//[OperationContract]
		//void XmlMethod(System.Xml.Linq.XElement xml);

		[OperationContract]
		QueueServiceResponse CallQueue(QueueServiceParam param);

		[OperationContract]
		GetTokenResponse GetToken(GetTokenParam param);
	}
}
