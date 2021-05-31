using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Security.Cryptography.X509Certificates;

namespace FSSExtensions
{
    public class WcfMessageInspectorBase : IClientMessageInspector
    {
        public WcfMessageInspectorBase(WcfEndpointBehaviorBase endpointBehavior)
        {
            EndpointBehavior = endpointBehavior;
        }

        public WcfEndpointBehaviorBase EndpointBehavior { get; set; }

        public static bool ValidateCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (EndpointBehavior.SkipSSLValidation)
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    new RemoteCertificateValidationCallback(ValidateCertificate);
            }

            FSSMessage fssMessage = FSSMessageFabric.ConstructMessage(request, EndpointBehavior);

            fssMessage.PrepareBeforeSendRequest();

            request = FSSMessage.XmlDocument2Message(fssMessage.XmlDocument, request.Version);

            return null;
        }

        public virtual void AfterReceiveReply(ref Message reply, object correlationState) 
        {
            FSSMessage fssMessage = FSSMessageFabric.ConstructMessage(reply, EndpointBehavior);

            fssMessage.PrepareAfterReceiveReply();

            reply = FSSMessage.XmlDocument2Message(fssMessage.XmlDocument, reply.Version);
        }
    }
}
