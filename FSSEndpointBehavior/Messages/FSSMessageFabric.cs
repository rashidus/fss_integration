using System;
using System.Xml;
using System.ServiceModel.Channels;

namespace FSSExtensions
{
    class FSSMessageFabric
    {
        protected WcfEndpointBehaviorBase endpointBehavior;
        protected Message message;
        public FSSMessageFabric(Message _message, WcfEndpointBehaviorBase _endpointBehavior)
        {
            message = _message;
            endpointBehavior = _endpointBehavior;
        }

        public FSSMessage GetMessage()
        {
            FSSMessage fssMessage = null;
            XmlDocument xmlDoc = FSSMessage.Message2XmlDocument(message);
            string messageSignature = xmlDoc.SelectSingleNode("/*/*/*").InnerText;

            if (endpointBehavior is SickList.EndpointBehaviorSickList)
            {
                switch (messageSignature)
                {
                    case "http://www.fss.ru/integration/ws/eln/ins/getPrivateLNData/v01":
                        fssMessage = new FSSMessageSickListGet(message, endpointBehavior as SickList.EndpointBehaviorSickList);
                        break;
                    case "http://www.fss.ru/integration/ws/eln/ins/prParseReestrFile/v01":
                        fssMessage = new FSSMessageSickListSet(message, endpointBehavior as SickList.EndpointBehaviorSickList);
                        break;
                    default:
                        fssMessage = new FSSMessageSickList(message, endpointBehavior as SickList.EndpointBehaviorSickList);
                        break;
                }
            }
            else if (endpointBehavior is WcfEndpointBehaviorBase)
            {
                switch (messageSignature)
                {
                    case "http://asystems.fss/IGatewayService/SendFileResponse":
                    case "http://asystems.fss/IGatewayService/SendFile":
                    case "http://asystems.fss/IGatewayService/UploadGetByExtIDResponse":
                    case "http://asystems.fss/IGatewayService/UploadGetByExtID":
                        fssMessage = new FSSMessageDirectPayments(message, endpointBehavior);
                        break;
                }                
            }            
            else
            {
                throw new ArgumentOutOfRangeException("endpointBehavior", $"Не настроено взаимодейтсвие с конечной точкой с типом {endpointBehavior.GetType()}");
            }
            return fssMessage;
        }

        public static FSSMessage ConstructMessage(Message message, WcfEndpointBehaviorBase endpointBehavior)
        {
            FSSMessageFabric fSSMessageCreator = new FSSMessageFabric(message, endpointBehavior);

            return fSSMessageCreator.GetMessage();
        }
    }
}
