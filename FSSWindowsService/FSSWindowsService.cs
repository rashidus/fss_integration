using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;

namespace FSSService
{
    public partial class FSSWindowsService : ServiceBase
    {
        public FSSWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Type sickListServiceType = typeof(FSSBrokerService.FileOperationsLnService);
            Type sickListContractType = typeof(FSSBrokerService.SickList.FileOperationsLnService);

            Type directPaymentsServiceType = typeof(FSSBrokerService.GatewayService);
            Type directPaymentsContractType = typeof(FSSBrokerService.DirectPayments.IGatewayService);

            sickListServiceHost = new ServiceHost(sickListServiceType);
            directPaymentsServiceHost = new ServiceHost(directPaymentsServiceType);

            // Add behavior for Services - enable WSDL access
            ServiceMetadataBehavior sickListBehavior = new ServiceMetadataBehavior();
            sickListBehavior.HttpGetEnabled = true;
            sickListBehavior.HttpGetUrl = new Uri("http://localhost:9001/sickList");
            sickListServiceHost.Description.Behaviors.Add(sickListBehavior);

            ServiceMetadataBehavior directPaymentsBehavior = new ServiceMetadataBehavior();
            directPaymentsBehavior.HttpGetEnabled = true;
            directPaymentsBehavior.HttpGetUrl = new Uri("http://localhost:9001/directPayments");
            directPaymentsServiceHost.Description.Behaviors.Add(directPaymentsBehavior);

            // Create basicHttpBinding endpoint at http://localhost:9001/sickList/  
            sickListServiceHost.AddServiceEndpoint(sickListContractType, new BasicHttpBinding(), "http://localhost:9001/sickList");
            // Create basicHttpBinding endpoint at http://localhost:9001/directPayments/  
            directPaymentsServiceHost.AddServiceEndpoint(directPaymentsContractType, new BasicHttpBinding(), "http://localhost:9001/directPayments");

            sickListServiceHost.Open();
            directPaymentsServiceHost.Open();
        }

        

        /*protected override void OnStart(string[] args)
        {
            if (sickListServiceHost != null)
            {
                sickListServiceHost.Close();
            }
            if (directPaymentsServiceHost != null)
            {
                directPaymentsServiceHost.Close();
            }

            string address_HTTP = "http://localhost:9001/";
            //string address_TCP = "net.tcp://localhost:9002/FSSBrokerService";

            //Больничные
            Uri baseUri = new Uri(address_HTTP);
            Uri sickListUri = new Uri(baseUri, "SickList");
            Uri testSickListUri = new Uri(sickListUri, "test");

            Uri[] sickListAddresses = { sickListUri, testSickListUri };

            sickListServiceHost = new ServiceHost(typeof(FSSBrokerService.FileOperationsLn), sickListAddresses);

            ServiceMetadataBehavior behaviorSickList = new ServiceMetadataBehavior();
            behaviorSickList.HttpGetEnabled = true;
            behaviorSickList.HttpsGetEnabled = true;

            sickListServiceHost.Description.Behaviors.Add(behaviorSickList);

            sickListServiceHost.AddServiceEndpoint(typeof(FSSBrokerService.SickList.FileOperationsLn), new BasicHttpBinding(), address_HTTP);
            sickListServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            sickListServiceHost.Open();
            
            //DirectPayments
            Uri directPaymentsUri = new Uri(baseUri, "DirectPayments");
            Uri testDirectPaymentsUri = new Uri(directPaymentsUri, "test");

            Uri[] directPaymentsAddresses = {directPaymentsUri, testDirectPaymentsUri};
            
            directPaymentsServiceHost = new ServiceHost(typeof(FSSBrokerService.GatewayService), directPaymentsAddresses);

            ServiceMetadataBehavior behaviorDirectPayments = new ServiceMetadataBehavior();
            behaviorDirectPayments.HttpGetEnabled = true;
            behaviorDirectPayments.HttpsGetEnabled = true;

            directPaymentsServiceHost.Description.Behaviors.Add(behaviorDirectPayments);

            directPaymentsServiceHost.AddServiceEndpoint(typeof(FSSBrokerService.DirectPayments.IGatewayService), new BasicHttpBinding(), address_HTTP);
            directPaymentsServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            directPaymentsServiceHost.Open();
        }*/

        protected override void OnStop()
        { 
            if (sickListServiceHost != null)
            {
                sickListServiceHost.Close();
                sickListServiceHost = null;
            }
            if (directPaymentsServiceHost != null)
            {
                directPaymentsServiceHost.Close();
                directPaymentsServiceHost = null;
            }
        }
    
    }
}
