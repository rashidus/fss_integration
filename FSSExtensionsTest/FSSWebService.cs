using FSSExtensions.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;

namespace FSSExtansionsTest
{
    public class FSSWebService
    {
        static public FSSWSInsurer.FileOperationsLnClient 
            CreateClient(string regNum, string insurerSertSerialNum, string fssSertSerialNum)
        {
            /*EndpointAddress endpointAdress = new EndpointAddress("https://docs-test.fss.ru/ws-insurer-v11/FileOperationsLnPort?WSDL");
            BasicHttpBinding binding1 = new BasicHttpBinding();
            binding1.Security.Mode = BasicHttpSecurityMode.Transport;*/

            FSSWSInsurer.FileOperationsLnClient client = new FSSWSInsurer.FileOperationsLnClient();

            /*FSSEndpointBehavior endpointBehavior = new FSSEndpointBehavior()
            {
                RegNum = regNum,
                InsurerCertificate = FindCertificate(insurerSertSerialNum),
                CryptoCertificate = FindCertificate(fssSertSerialNum)
            };

            client.Endpoint.Behaviors.Add(endpointBehavior);*/
            return client;
        }
        static public X509Certificate2 FindCertificate(string serialNumber)
        {
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection cers = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, false);

            X509Certificate2 cer = new X509Certificate2();

            if (cers.Count > 0)
            {
                cer = cers[0];
            };
            store.Close();

            return cer;
        }

    }
}
