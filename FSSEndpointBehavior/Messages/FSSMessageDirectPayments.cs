using System;
using System.Xml;
using System.ServiceModel.Channels;

namespace FSSExtensions
{
    class FSSMessageDirectPayments : FSSMessage
    {
        public FSSMessageDirectPayments(Message _message, WcfEndpointBehaviorBase _endpoinBehavior) : base(_message, _endpoinBehavior) { }

        public override void Sign()
        {
            XmlNode dataNode = GetDataNodeRequest();
            if (dataNode != null)
            {
                byte[] data = System.Convert.FromBase64String(dataNode.InnerText);
                byte[] signedData = ByteHelper.Sign(data, endpointBehavior.InsurerCertificate);
                dataNode.InnerText = System.Convert.ToBase64String(signedData);
            }            
        }

        public override void Encrypt()
        {
            XmlNode dataNode = GetDataNodeRequest();
            if (dataNode != null)
            {
                byte[] data = System.Convert.FromBase64String(dataNode.InnerText);
                byte[] encryptedData = ByteHelper.Encrypt(data, endpointBehavior.FSSCertificate);
                dataNode.InnerText = System.Convert.ToBase64String(encryptedData);
            }            
        }

        public override void Decrypt()
        {
            XmlNode fileTicketNode = GetDataNodeResponse();
            if (fileTicketNode != null)
            {
                string decryptedData = System.Convert.ToBase64String(ByteHelper.Decrypt(System.Convert.FromBase64String(fileTicketNode.InnerText)));
                fileTicketNode.InnerText = decryptedData;
            }            
        }

        protected XmlNode GetDataNodeRequest()
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(XmlDocument.NameTable);
            nsmgr.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
            nsmgr.AddNamespace("f", "http://asystems.fss");

            XmlNode dataNode = XmlDocument.SelectSingleNode("/s:Envelope/s:Body/f:SendFile/f:data", nsmgr);

            return dataNode;
        }

        protected XmlNode GetDataNodeResponse()
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(XmlDocument.NameTable);
            nsmgr.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
            nsmgr.AddNamespace("f", "http://asystems.fss");
            nsmgr.AddNamespace("b", "http://schemas.datacontract.org/2004/07/AS.FSS.Gateway.DataAccess.Model");

            XmlNode fileTicketNode = XmlDocument.SelectSingleNode("/s:Envelope/s:Body/f:UploadGetByExtIDResponse/f:UploadGetByExtIDResult/b:FILE_TICKET_ENC", nsmgr);

            return fileTicketNode;
        }
    }
}
