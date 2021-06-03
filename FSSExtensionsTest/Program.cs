using System;
using System.IO;

namespace FSSExtensionsTest
{
    class Program
    {
        static private string RegNum
        {
            get
            {
                return "регномер_в_ФСС";
            }
        }
        static private string LNCode
        {
            get
            {
                return "номер_листка_нетрудоспособности";
            }
        }
        static private string Snils
        {
            get
            {
                return "СНИЛС";
            }
        }

        static void Main(string[] args)
        {
            //TestDirectPayments();
            //TestGetDirectPaymentsByExtId("идентификатор_в_ФСС");
            TestGetSickListWindowsService();
            //TestGetSickListBroker();
            //TestSendSickListWindowsService();
            //TestSendSickListBroker();
        }

        static void TestDirectPayments()
        {
            string ticketFile = @"C:\Users\usmanov\Desktop\E_регномерВфсс_год_месяц_день_порядковыйНомерЗаДень.xml"; //наименование файла согласно спецификации

            FSSBrokerService.DirectPayments.GatewayServiceClient client = new FSSBrokerService.DirectPayments.GatewayServiceClient("DirectPayments_test");
            FSSBrokerService.DirectPayments.UploadResult uploadResult = client.SendFile(File.ReadAllBytes(ticketFile), String.Format("{0}.esl", Path.GetFileNameWithoutExtension(ticketFile)));

            Console.WriteLine(uploadResult.ExtID);

            FSSBrokerService.DirectPayments.UPLOADS uploads = client.UploadGetByExtID(uploadResult.ExtID);
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("windows-1251");
            Console.WriteLine(encoding.GetString(uploads.LAST_EXCEPTION)); 
            Console.WriteLine(uploads.STATE);
            Console.WriteLine(uploads.STATE_ID);
            Console.WriteLine(uploads.FSS_ERROR);
            if (uploads.FILE_TICKET != null)
            {
                File.WriteAllBytes(ticketFile, uploads.FILE_TICKET);
            }            

            Console.ReadKey();
        }
        static void TestGetDirectPaymentsByExtId(string _extId)
        {
            FSSBrokerService.DirectPayments.GatewayServiceClient client = new FSSBrokerService.DirectPayments.GatewayServiceClient("DirectPayments");
            FSSBrokerService.DirectPayments.UPLOADS uploads = client.UploadGetByExtID(_extId);
            Console.WriteLine(uploads.FSS_ERROR);
        }        

        static void TestGetSickListWindowsService()
        {
            SickList_winService.GetPrivateLNDataRequest request = new SickList_winService.GetPrivateLNDataRequest();
            request.regNum = RegNum;
            request.lnCode = LNCode;
            request.snils = Snils;

            SickList_winService.FileOperationsLnServiceClient windowsServiceClient = new SickList_winService.FileOperationsLnServiceClient("SickList_win");
            SickList_winService.GetPrivateLNDataResponse response = windowsServiceClient.GetPrivateLNData(request);
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.status}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.mess}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.info}");
            Console.ReadKey();
        }

        static void TestGetSickListBroker()
        {
            FSSBrokerService.SickList.GetPrivateLNDataRequest request = new FSSBrokerService.SickList.GetPrivateLNDataRequest();
            request.regNum = RegNum;
            request.lnCode = LNCode;
            request.snils = Snils;

            FSSBrokerService.SickList.FileOperationsLnServiceClient brokerServiceClient = new FSSBrokerService.SickList.FileOperationsLnServiceClient("SickList");
            FSSBrokerService.SickList.GetPrivateLNDataResponse response = brokerServiceClient.GetPrivateLNData(request);
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.status}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.mess}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.fileOperationsLnUserGetPrivateLNDataOut.info}");
            Console.ReadKey();
        }

        static void TestSendSickListWindowsService()
        {
            SickList_winService.PrParseReestrFileRequest request = new SickList_winService.PrParseReestrFileRequest();
            request.request = new SickList_winService.PrParseReestrFileType();
            request.request.regNum = RegNum;
            request.request.pXmlFile = new SickList_winService.RowsetWrapper();
            request.request.pXmlFile.rowset = new SickList_winService.Rowset();
            request.request.pXmlFile.rowset.author = "автор";
            request.request.pXmlFile.rowset.email = "эл.почта";
            request.request.pXmlFile.rowset.phone = "телефон";
            request.request.pXmlFile.rowset.software = "наименование_программы";
            request.request.pXmlFile.rowset.version_software = "версия_программы";
            request.request.pXmlFile.rowset.version = "2.0"; //версия ЭЛН

            SickList_winService.RowsetRow row = new SickList_winService.RowsetRow();
            row.lnCode = LNCode;
            row.snils = Snils;
            row.innPerson = "ИНН";
            row.employer = "Наименование_организации";
            row.emplFlag = true;
            row.emplRegNo = RegNum;
            row.emplParentNo = "Идентификатор_головного_подразделения";
            row.approve1 = "ФИО_директора";
            row.approve2 = "ФИО_гл.буха";
            row.baseAvgSal = 0; //база расчета (заработок за 2 года)
            row.baseAvgSalSpecified = true;
            row.baseAvgDailySal = 0; //среднедневной заработок
            row.baseAvgDailySalSpecified = true;
            row.insurYy = 0; //страховой стаж, лет
            row.insurMm = 0; //страховой стаж, месяцев
            row.notInsurYy = 0; //нестраховой стаж, лет
            row.notInsurMm = 0; //нестраховой стаж, месяцев          
            row.calcCondition1 = ""; //условия для расчета
            row.dt1Ln = new DateTime(2021, 4, 26); //дата начала периода нетрудосопобности (начисления пособия)
            row.dt2Ln = new DateTime(2021, 5, 3); //дата окончания периода нетрудосопобности (начисления пособия)
            row.emplPayment = 0; //сумма к оплате средствами работодателя
            row.emplPaymentSpecified = true;
            row.fssPayment = 0; //сумма к оплате средствами ФСС
            row.fssPaymentSpecified = true;
            row.payment = 0; //сумма к оплате средствами всего
            row.paymentSpecified = true;
            //если не указывать свойства Specified по суммам, то они будут нулевыми
            SickList_winService.RowsetRow[] rowsArray = new SickList_winService.RowsetRow[1];
            rowsArray[0] = row;

            request.request.pXmlFile.rowset.row = rowsArray;

            SickList_winService.FileOperationsLnServiceClient windowsServiceClient = new SickList_winService.FileOperationsLnServiceClient("SickList_win");
            SickList_winService.PrParseReestrFileResponse response = windowsServiceClient.PrParseReestrFile(request);
            Console.WriteLine($"{response.wsResult.status}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.wsResult.mess}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.wsResult.info}");
            Console.ReadKey();
        }

        static void TestSendSickListBroker()
        {
            FSSBrokerService.SickList.PrParseReestrFileRequest request = new FSSBrokerService.SickList.PrParseReestrFileRequest();
            request.request = new FSSBrokerService.SickList.PrParseReestrFileType();
            request.request.regNum = RegNum;
            request.request.pXmlFile = new FSSBrokerService.SickList.RowsetWrapper();
            request.request.pXmlFile.rowset = new FSSBrokerService.SickList.Rowset();
            request.request.pXmlFile.rowset.author = "автор";
            request.request.pXmlFile.rowset.email = "эл.почта";
            request.request.pXmlFile.rowset.phone = "телефон";
            request.request.pXmlFile.rowset.software = "наименование_программы";
            request.request.pXmlFile.rowset.version_software = "версия_программы";
            request.request.pXmlFile.rowset.version = "2.0"; //версия ЭЛН

            FSSBrokerService.SickList.RowsetRow row = new FSSBrokerService.SickList.RowsetRow();
            row.lnCode = LNCode;
            row.snils = Snils;
            row.innPerson = "ИНН";
            row.employer = "Наименование_организации";
            row.emplFlag = true;
            row.emplRegNo = RegNum;
            row.emplParentNo = "Идентификатор_головного_подразделения";
            row.approve1 = "ФИО_директора";
            row.approve2 = "ФИО_гл.буха";
            row.baseAvgSal = 0; //база расчета (заработок за 2 года)
            row.baseAvgSalSpecified = true;
            row.baseAvgDailySal = 0; //среднедневной заработок
            row.baseAvgDailySalSpecified = true;
            row.insurYy = 0; //страховой стаж, лет
            row.insurMm = 0; //страховой стаж, месяцев
            row.notInsurYy = 0; //нестраховой стаж, лет
            row.notInsurMm = 0; //нестраховой стаж, месяцев          
            row.calcCondition1 = ""; //условия для расчета
            row.dt1Ln = new DateTime(2021, 4, 26); //дата начала периода нетрудосопобности (начисления пособия)
            row.dt2Ln = new DateTime(2021, 5, 3); //дата окончания периода нетрудосопобности (начисления пособия)
            row.emplPayment = 0; //сумма к оплате средствами работодателя
            row.emplPaymentSpecified = true;
            row.fssPayment = 0; //сумма к оплате средствами ФСС
            row.fssPaymentSpecified = true;
            row.payment = 0; //сумма к оплате средствами всего
            row.paymentSpecified = true;
            //если не указывать свойства Specified по суммам, то они будут нулевыми

            FSSBrokerService.SickList.RowsetRow[] rowsArray = new FSSBrokerService.SickList.RowsetRow[1];
            rowsArray[0] = row;

            request.request.pXmlFile.rowset.row = rowsArray;

            FSSBrokerService.SickList.FileOperationsLnServiceClient brokerServiceClient = new FSSBrokerService.SickList.FileOperationsLnServiceClient("SickList");
            FSSBrokerService.SickList.PrParseReestrFileResponse response = brokerServiceClient.PrParseReestrFile(request);
            Console.WriteLine($"{response.wsResult.status}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.wsResult.mess}");
            Console.WriteLine("=================");
            Console.WriteLine($"{response.wsResult.info}");
            Console.ReadKey();
        }
    }
}
