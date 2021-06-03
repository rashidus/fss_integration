﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FSSExtensionsTest.DirectPayments_winService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://asystems.fss", ConfigurationName="DirectPayments_winService.IGatewayService")]
    public interface IGatewayService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://asystems.fss/IGatewayService/SendFile", ReplyAction="http://asystems.fss/IGatewayService/SendFileResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(FSSBrokerService.DirectPayments.GeneralFault), Action="http://asystems.fss/IGatewayService/SendFileGeneralFaultFault", Name="GeneralFault", Namespace="http://schemas.datacontract.org/2004/07/AS.FSS.Gateway.ExtService.Model")]
        FSSBrokerService.DirectPayments.UploadResult SendFile(byte[] data, string fileName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://asystems.fss/IGatewayService/UploadsGet", ReplyAction="http://asystems.fss/IGatewayService/UploadsGetResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(FSSBrokerService.DirectPayments.GeneralFault), Action="http://asystems.fss/IGatewayService/UploadsGetGeneralFaultFault", Name="GeneralFault", Namespace="http://schemas.datacontract.org/2004/07/AS.FSS.Gateway.ExtService.Model")]
        FSSBrokerService.DirectPayments.UPLOADS[] UploadsGet(string regNum, FSSBrokerService.DirectPayments.DateFilter filter);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://asystems.fss/IGatewayService/UploadGetByExtID", ReplyAction="http://asystems.fss/IGatewayService/UploadGetByExtIDResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(FSSBrokerService.DirectPayments.GeneralFault), Action="http://asystems.fss/IGatewayService/UploadGetByExtIDGeneralFaultFault", Name="GeneralFault", Namespace="http://schemas.datacontract.org/2004/07/AS.FSS.Gateway.ExtService.Model")]
        FSSBrokerService.DirectPayments.UPLOADS UploadGetByExtID(string id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGatewayServiceChannel : FSSExtensionsTest.DirectPayments_winService.IGatewayService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GatewayServiceClient : System.ServiceModel.ClientBase<FSSExtensionsTest.DirectPayments_winService.IGatewayService>, FSSExtensionsTest.DirectPayments_winService.IGatewayService {
        
        public GatewayServiceClient() {
        }
        
        public GatewayServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GatewayServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GatewayServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GatewayServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public FSSBrokerService.DirectPayments.UploadResult SendFile(byte[] data, string fileName) {
            return base.Channel.SendFile(data, fileName);
        }
        
        public FSSBrokerService.DirectPayments.UPLOADS[] UploadsGet(string regNum, FSSBrokerService.DirectPayments.DateFilter filter) {
            return base.Channel.UploadsGet(regNum, filter);
        }
        
        public FSSBrokerService.DirectPayments.UPLOADS UploadGetByExtID(string id) {
            return base.Channel.UploadGetByExtID(id);
        }
    }
}
