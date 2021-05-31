using System;
using System.Xml;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace FSSExtensions.SickList
{
    public class WcfBehaviorExtensionElementSickList : WcfBehaviorExtensionElementBase
    {
        private ConfigurationPropertyCollection properties;

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        [Category("Insurer")]
        [ConfigurationProperty("RegNum", DefaultValue = ""), StringValidator(MinLength = 0)]
        public string RegNum
        {
            get
            {
                return (string)base["RegNum"];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["RegNum"] = value;
            }
        }

        [Category("Insurer")]
        [ConfigurationProperty("ChiefCertificate")]
        public X509CertificateConfigurationElement ChiefCertificate
        {
            get
            {
                return (X509CertificateConfigurationElement)base["ChiefCertificate"];
            }
        }

        [Category("Insurer")]
        [ConfigurationProperty("AccountantCertificate")]
        public X509CertificateConfigurationElement AccountantCertificate
        {
            get
            {
                return (X509CertificateConfigurationElement)base["AccountantCertificate"];
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new ConfigurationPropertyCollection
                    {
                        new ConfigurationProperty("InsurerCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("ChiefCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("AccountantCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("FSSCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("RegNum", typeof(string), string.Empty, null, new StringValidator(0, 2147483647, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("UseEncryption", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("SkipSSLValidation", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        //new ConfigurationProperty("ProviderType", typeof(ProviderType), ProviderType.VipNet, null, new StandardRuntimeEnumValidator(typeof(ProviderType)), ConfigurationPropertyOptions.None)
                    };
                }
                return this.properties;
            }
        }

        public override Type BehaviorType
        {
            get { return typeof(EndpointBehaviorSickList); }
        }
        protected override object CreateBehavior()
        {
            // Create the  endpoint behavior that will insert the message  
            // inspector into the client runtime  
            EndpointBehaviorSickList fssEndpointBehavior = new EndpointBehaviorSickList();

            this.ApplyConfiguration(fssEndpointBehavior);
            return fssEndpointBehavior;
        }
        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            WcfBehaviorExtensionElementBase fssBehaviorExtensionElement = (WcfBehaviorExtensionElementBase)from;
            this.InsurerCertificate.Copy(fssBehaviorExtensionElement.InsurerCertificate);

        }
        protected internal void ApplyConfiguration(EndpointBehaviorSickList behavior)
        {
            PropertyInformationCollection propertyInformationCollection = base.ElementInformation.Properties;

            base.ApplyConfiguration(behavior);

            if (propertyInformationCollection["ChiefCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.ChiefCertificate = ChiefCertificate.FindCertificate();
            }
            if (propertyInformationCollection["AccountantCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.AccountantCertificate = AccountantCertificate.FindCertificate();
            }
            if (propertyInformationCollection["RegNum"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.RegNum = RegNum;
            }
        }
    }
}
