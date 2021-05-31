using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace FSSExtensions.SickList
{
    public class EndpointBehaviorSickList : FSSExtensions.WcfEndpointBehaviorBase
    {
        public X509Certificate2 ChiefCertificate { get; set; }
        public X509Certificate2 AccountantCertificate { get; set; }
        public string RegNum { get; set; }

        public override void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //ProviderType = ProviderType;
            clientRuntime.MessageInspectors.Add(new WcfMessageInspectorBase(this));
        }
        public override void Validate(ServiceEndpoint endpoint)
        {
            base.Validate(endpoint);

            if (ChiefCertificate == null)
            {
                throw new Exception("Сертификат руководителя не найден, проверьте настройки (ChiefCertificate)");
            }
            else
            {
                X509CertificateConfigurationElement.ValidateCertExpiration(ChiefCertificate, "Сертификат руководителя (ChiefCertificate)");
            }
            if (AccountantCertificate == null)
            {
                throw new Exception("Сертификат главного бухгалтера не найден, проверьте настройки (AccountantCertificate)");
            }
            else
            {
                X509CertificateConfigurationElement.ValidateCertExpiration(AccountantCertificate, "Сертификат главного бухгалтера (AccountantCertificate)");
            }
            if (String.IsNullOrEmpty(RegNum))
            {
                throw new Exception("Не задан регистрационный номер страхователя (RegNum)");
            }
        }
    }
}
