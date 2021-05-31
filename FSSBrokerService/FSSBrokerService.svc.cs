using System;
using System.Diagnostics;
using System.ServiceModel;

namespace FSSBrokerService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class FileOperationsLnService : SickList.FileOperationsLnService
    {
        public SickList.getPrivateLNDataResponse1 GetPrivateLNData(SickList.getPrivateLNDataRequest1 request1)
        {
            SickList.GetPrivateLNDataResponse response;

            try
            {
                SickList.FileOperationsLnServiceClient client = new SickList.FileOperationsLnServiceClient("SickList");

                response = client.GetPrivateLNData(request1.getPrivateLNDataRequest);
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "FSS-SickList";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Information, 101, 1);
                }
                               
                SickList.FileOperationsLnUserGetLNDataOut lnDataOut = new SickList.FileOperationsLnUserGetLNDataOut();
                lnDataOut.status = 0;
                lnDataOut.mess = ex.Message;

                response = new SickList.GetPrivateLNDataResponse();
                response.fileOperationsLnUserGetPrivateLNDataOut = lnDataOut;
            }
            return new SickList.getPrivateLNDataResponse1(response);
        }

        public SickList.prParseReestrFileResponse1 PrParseReestrFile(SickList.prParseReestrFileRequest1 request1)
        {
            SickList.PrParseReestrFileResponse response;

            try
            {
                SickList.FileOperationsLnServiceClient client = new SickList.FileOperationsLnServiceClient("SickList");

                response = client.PrParseReestrFile(request1.prParseReestrFileRequest);
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "FSS-SickList";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Information, 101, 1);
                }

                SickList.WSResult wsResult = new SickList.WSResult();
                wsResult.status = 0;
                wsResult.mess = ex.Message;

                response = new SickList.PrParseReestrFileResponse();
                response.wsResult = wsResult;
            }
            return new SickList.prParseReestrFileResponse1(response);
        }
    }

    public class GatewayService : DirectPayments.IGatewayService
    {
        public DirectPayments.UploadResult SendFile(byte[] data, string fileName)
        {
            DirectPayments.UploadResult result;

            try
            {
                DirectPayments.GatewayServiceClient client = new DirectPayments.GatewayServiceClient("DirectPayments");

                result = client.SendFile(data, fileName);

                return result;
            }
            catch (FaultException ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "FSS-DirectPayments";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Information, 101, 1);
                }
                throw ex;
            }
        }

        public DirectPayments.UPLOADS UploadGetByExtID(string id)
        {
            DirectPayments.UPLOADS upload;
            
            try
            {
                DirectPayments.GatewayServiceClient client = new DirectPayments.GatewayServiceClient("DirectPayments");

                upload = client.UploadGetByExtID(id);

                return upload;
            }
            catch (FaultException ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "FSS-DirectPayments";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Information, 101, 1);
                }
                throw ex;
            }
        }

        public DirectPayments.UPLOADS[] UploadsGet(string regNum, DirectPayments.DateFilter filter)
        {
            DirectPayments.UPLOADS[] uploads;

            try
            {
                DirectPayments.GatewayServiceClient client = new DirectPayments.GatewayServiceClient("DirectPayments");

                uploads = client.UploadsGet(regNum, filter);

                return uploads;
            }
            catch (FaultException ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "FSS-DirectPayments";
                    eventLog.WriteEntry(ex.Message, EventLogEntryType.Information, 101, 1);
                }
                throw ex;
            }
        }
    }
}
