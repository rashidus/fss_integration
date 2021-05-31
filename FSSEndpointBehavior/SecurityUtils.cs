using System.Security.Cryptography.X509Certificates;

namespace FSSExtensions
{
    internal static class SecurityUtils
    {
        public static X509Certificate2 GetCertificateFromStoreCore(StoreName storeName, StoreLocation storeLocation, X509FindType findType, object findValue)
        {
            X509Store x509CertificateStore = new X509Store(storeName, storeLocation);
            X509Certificate2Collection x509Certificate2Collection = null;
            X509Certificate2 result;
            try
            {
                x509CertificateStore.Open(OpenFlags.ReadOnly);
                x509Certificate2Collection = x509CertificateStore.Certificates.Find(findType, findValue, false);
                if (x509Certificate2Collection.Count == 1)
                {
                    result = new X509Certificate2(x509Certificate2Collection[0]);
                }
                else
                {
                    result = null;
                }
            }
            finally
            {
                if (x509Certificate2Collection != null)
                {
                    for (int i = 0; i < x509Certificate2Collection.Count; i++)
                    {
                        x509Certificate2Collection[i].Reset();
                    }
                }
                x509CertificateStore.Close();
            }
            return result;
        }
    }
}
