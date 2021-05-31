using System;
using System.ComponentModel;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace FSSExtensions
{
    public class WcfEndpointBehaviorBase : IEndpointBehavior
    {
        public X509Certificate2 InsurerCertificate { get; set; }
        public X509Certificate2 FSSCertificate { get; set; }
        //public ProviderType ProviderType { get; set; }
        public bool UseEncryption { get; set; }
        public bool SkipSSLValidation { get; set; }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }

        public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //ProviderType = ProviderType;
            clientRuntime.MessageInspectors.Add(new WcfMessageInspectorBase(this));
        }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public virtual void Validate(ServiceEndpoint endpoint)
        {
            if (InsurerCertificate == null)
            {
                throw new Exception("Сертификат страхователя не найден, проверьте настройки (InsurerCertificate)");
            }
            else if (UseEncryption && InsurerCertificate != null)
            {
                X509CertificateConfigurationElement.ValidateCertExpiration(InsurerCertificate, "Сертификат страхователя (InsurerCertificate)");
            }
            if (UseEncryption && FSSCertificate == null)
            {
                throw new Exception("Сертификат ФСС не найден, проверьте настройки (FSSCertificate)");
            }
            else if (UseEncryption && FSSCertificate != null)
            {
                X509CertificateConfigurationElement.ValidateCertExpiration(FSSCertificate, "Сертификат ФСС (FSSCertificate)");
            }
        }
    }
}
