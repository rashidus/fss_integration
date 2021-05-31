using System;
using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using FSSExtensions.Tools;

namespace FSSExtensions
{
    [TypeConverter(typeof(X509CertificateConverter))]
    public class X509CertificateConfigurationElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;
        
        public ConfigurationPropertyCollection GetProperties()
        {
            return Properties;
        }
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new ConfigurationPropertyCollection
                    {
                        new ConfigurationProperty("findValue", typeof(string), string.Empty, null, new StringValidator(0, 2147483647, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("storeLocation", typeof(StoreLocation), StoreLocation.CurrentUser, null, new StandardRuntimeEnumValidator(typeof(StoreLocation)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("storeName", typeof(StoreName), StoreName.My, null, new StandardRuntimeEnumValidator(typeof(StoreName)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("x509FindType", typeof(X509FindType), X509FindType.FindBySubjectDistinguishedName, null, new StandardRuntimeEnumValidator(typeof(X509FindType)), ConfigurationPropertyOptions.None)
                    };
                }
                return this.properties;
            }
        }
        [Browsable(true)]
        [ConfigurationProperty("findValue", DefaultValue = ""), StringValidator(MinLength = 0)]
        public string FindValue
        {
            get
            {
                return (string)base["findValue"];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["findValue"] = value;
            }
        }
        [Browsable(true)]
        [ConfigurationProperty("storeLocation", DefaultValue = StoreLocation.CurrentUser), StandardRuntimeEnumValidator(typeof(StoreLocation))]
        public StoreLocation StoreLocation
        {
            get
            {
                return (StoreLocation)base["storeLocation"];
            }
            set
            {
                base["storeLocation"] = value;
            }
        }
        [Browsable(true)]
        [ConfigurationProperty("storeName", DefaultValue = StoreName.My), StandardRuntimeEnumValidator(typeof(StoreName))]
        public StoreName StoreName
        {
            get
            {
                return (StoreName)base["storeName"];
            }
            set
            {
                base["storeName"] = value;
            }
        }
        [Browsable(true)]
        [ConfigurationProperty("x509FindType", DefaultValue = X509FindType.FindBySubjectDistinguishedName), StandardRuntimeEnumValidator(typeof(X509FindType))]
        public X509FindType X509FindType
        {
            get
            {
                return (X509FindType)base["x509FindType"];
            }
            set
            {
                base["x509FindType"] = value;
            }
        }

        public void Copy(X509CertificateConfigurationElement from)
        {
            this.FindValue = from.FindValue;
            this.StoreLocation = from.StoreLocation;
            this.StoreName = from.StoreName;
            this.X509FindType = from.X509FindType;
        }

        internal X509Certificate2 FindCertificate()
        {
            return SecurityUtils.GetCertificateFromStoreCore(StoreName, StoreLocation, X509FindType, FindValue);
        }

        public static void ValidateCertExpiration(X509Certificate2 cert, string description)
        {
            DateTime curDateTime = DateTime.Now;
            if (cert.NotAfter <= curDateTime || cert.NotBefore > curDateTime)
            {
                throw new Exception($"{description} недействителен! Период действия - с {cert.NotBefore} по {cert.NotAfter}");
            }
        }
    }
   
    public class X509CertificateConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
        {
            if (destType == typeof(string) && value is X509CertificateConfigurationElement)
            {
                X509CertificateConfigurationElement cert = (X509CertificateConfigurationElement)value;

                X509Certificate2 x509cert = cert.FindCertificate();

                if(x509cert == null)
                {
                    return String.Empty;
                }
                else
                {
                    return x509cert.GetNameInfo(X509NameType.SimpleName, false);
                }
            }
            return base.ConvertTo(context, culture, value, destType);
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if(value is X509CertificateConfigurationElement)
            {
                X509CertificateConfigurationElement cert = (X509CertificateConfigurationElement)value;
                               
                PropertyDescriptorCollection allProps = TypeDescriptor.GetProperties(cert);
                
                PropertyDescriptor[] propertyDescriptor = new PropertyDescriptor[4];
                propertyDescriptor[0] = allProps["FindValue"];
                propertyDescriptor[1] = allProps["StoreLocation"];
                propertyDescriptor[2] = allProps["StoreName"];
                propertyDescriptor[3] = allProps["X509FindType"];
                              
                return new PropertyDescriptorCollection(propertyDescriptor);
            }
            return base.GetProperties(context, value, attributes);
        }
    }
}
