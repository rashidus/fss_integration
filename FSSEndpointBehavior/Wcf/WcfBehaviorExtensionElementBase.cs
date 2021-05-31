using System;
using System.Xml;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace FSSExtensions
{
    public class WcfBehaviorExtensionElementBase : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        [Category("Insurer")]
        [ConfigurationProperty("InsurerCertificate")]
        public X509CertificateConfigurationElement InsurerCertificate
        {
            get
            {
                return (X509CertificateConfigurationElement)base["InsurerCertificate"];
            }
        }

        [Category("Encryption")]
        [ConfigurationProperty("UseEncryption", DefaultValue = false)]
        public bool UseEncryption
        {
            get
            {
                return (bool)base["UseEncryption"];
            }
            set
            {
                base["UseEncryption"] = value;
            }
        }
        [Category("Encryption")]
        [ConfigurationProperty("SkipSSLValidation", DefaultValue = false)]
        public bool SkipSSLValidation
        {
            get
            {
                return (bool)base["SkipSSLValidation"];
            }
            set
            {
                base["SkipSSLValidation"] = value;
            }
        }
        [Category("Encryption")]
        [ConfigurationProperty("FSSCertificate")]
        public X509CertificateConfigurationElement FSSCertificate
        {
            get
            {
                return (X509CertificateConfigurationElement)base["FSSCertificate"];
            }
        }
        /*
        [Category("Encryption")]
        [ConfigurationProperty("ProviderType", DefaultValue = ProviderType.VipNet), StandardRuntimeEnumValidator(typeof(ProviderType))]
        public ProviderType ProviderType
        {
            get
            {
                return (ProviderType)base["ProviderType"];
            }
            set
            {
                base["ProviderType"] = value;
            }
        }
        */
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new ConfigurationPropertyCollection
                    {
                        new ConfigurationProperty("InsurerCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("FSSCertificate", typeof(X509CertificateConfigurationElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("UseEncryption", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("SkipSSLValidation", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        //new ConfigurationProperty("ProviderType", typeof(ProviderType), ProviderType.CryptoPro, null, new StandardRuntimeEnumValidator(typeof(ProviderType)), ConfigurationPropertyOptions.None)
                    };
                }
                return this.properties;
            }
        }

        public override Type BehaviorType
        {
            get { return typeof(WcfEndpointBehaviorBase); }
        }
        public override bool IsReadOnly()
        {
            return false;
        }
        protected override object CreateBehavior()
        {
            // Create the  endpoint behavior that will insert the message  
            // inspector into the client runtime  
            WcfEndpointBehaviorBase endpointBehavior = new WcfEndpointBehaviorBase();

            this.ApplyConfiguration(endpointBehavior);
            return endpointBehavior;
        }
        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            WcfBehaviorExtensionElementBase behaviorExtensionElement = (WcfBehaviorExtensionElementBase)from;
            this.InsurerCertificate.Copy(behaviorExtensionElement.InsurerCertificate);
        }
        protected internal void ApplyConfiguration(WcfEndpointBehaviorBase behavior)
        {
            if (behavior == null)
            {
                throw new NullReferenceException(String.Format("Класс {0} не инициализирован", typeof(WcfEndpointBehaviorBase).ToString()));
            }
            PropertyInformationCollection propertyInformationCollection = base.ElementInformation.Properties;

            if (propertyInformationCollection["InsurerCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.InsurerCertificate = InsurerCertificate.FindCertificate();
            }
            if (propertyInformationCollection["FSSCertificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.FSSCertificate = FSSCertificate.FindCertificate();
            }
            if (propertyInformationCollection["UseEncryption"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.UseEncryption = UseEncryption;
            }
            if (propertyInformationCollection["SkipSSLValidation"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.SkipSSLValidation = SkipSSLValidation;
            }
            /*
            if (propertyInformationCollection["ProviderType"].ValueOrigin != PropertyValueOrigin.Default)
            {
                behavior.ProviderType = ProviderType;
            }
            */
        }
    }
}
