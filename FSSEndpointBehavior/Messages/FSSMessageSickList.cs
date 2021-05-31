using System;
using System.Xml;
using System.ServiceModel.Channels;
using FSSExtensions.SickList;

namespace FSSExtensions
{
    public class FSSMessageSickList : FSSMessage
    {
        protected string RegNum { get; set; }
        public FSSMessageSickList(Message _message, SickList.EndpointBehaviorSickList _endpoinBehavior) : base(_message, _endpoinBehavior) { }

        protected virtual void PrepareNamespaces()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(XmlDocument.NameTable);

            ns.AddNamespace("soapenv", Xmlns.soapenv);
            ns.AddNamespace("ds", Xmlns.ds);
            ns.AddNamespace("wsse", Xmlns.wsse);
            ns.AddNamespace("wsu", Xmlns.wsu);
            ns.AddNamespace("xsd", Xmlns.xsd);
            ns.AddNamespace("xsi", Xmlns.xsi);
            ns.AddNamespace("xenc", Xmlns.xmlenc);
            ns.AddNamespace("sch", Xmlns.sch);
        }

        protected virtual void InitRegNum()
        {
            RegNum = ((EndpointBehaviorSickList)endpointBehavior).RegNum;

            if (String.IsNullOrEmpty(RegNum))
            {
                XmlNodeList nodeList = XmlDocument.GetElementsByTagName("regNum", Xmlns.fssWsdl);

                if (nodeList != null && nodeList.Count == 1)
                {
                    XmlElement regNumElement = nodeList[0] as XmlElement;

                    RegNum = regNumElement.InnerText;
                }
            }            
        }

        public override void Sign()
        {
            InitRegNum();
            PrepareNamespaces();
        }

        public override void Encrypt()
        {
            XmlDocument = XmlDocumentHelper.Encrypt(XmlDocument, endpointBehavior.FSSCertificate, endpointBehavior.InsurerCertificate);
        }

        public override void Decrypt()
        {
            XmlDocument = XmlDocumentHelper.Decrypt(XmlDocument, endpointBehavior.InsurerCertificate);
        }
    }

    public class FSSMessageSickListSet : FSSMessageSickList
    {
        public FSSMessageSickListSet(Message _message, SickList.EndpointBehaviorSickList _endpoinBehavior) : base(_message, _endpoinBehavior) { }

        public override void Sign()
        {
            base.Sign();

            XmlNodeList rowNode = XmlDocument.GetElementsByTagName("row", Xmlns.fssWsdl);

            foreach (XmlElement row in rowNode)
            {
                XmlNodeList lnCode = row.GetElementsByTagName("lnCode", Xmlns.fssWsdl);

                if (lnCode != null && lnCode.Count == 1)
                {
                    XmlElement lnCodeTag = lnCode[0] as XmlElement;
                    string lnNum = lnCodeTag.InnerText;

                    if (!String.IsNullOrEmpty(lnNum))
                    {
                        row.RemoveAllAttributes();

                        row.SetAttribute("xmlns:wsu", Xmlns.wsu);
                        row.SetAttribute("Id", Xmlns.wsu, $"ELN_{lnNum}");

                        XmlDocumentHelper.Sign(XmlDocument, endpointBehavior.InsurerCertificate, $"insurer/{RegNum}/{lnNum}", $"ELN_{lnNum}");
                        XmlDocumentHelper.Sign(XmlDocument, (endpointBehavior as EndpointBehaviorSickList)?.ChiefCertificate, $"chief/{RegNum}/{lnNum}", $"ELN_{lnNum}"); //TODO
                        XmlDocumentHelper.Sign(XmlDocument, (endpointBehavior as EndpointBehaviorSickList).AccountantCertificate, $"accountant/{RegNum}/{lnNum}", $"ELN_{lnNum}");
                    }
                }
            }
        }
    }

    public class FSSMessageSickListGet : FSSMessageSickList
    {
        public FSSMessageSickListGet(Message _message, SickList.EndpointBehaviorSickList _endpoinBehavior) : base(_message, _endpoinBehavior) { }

        protected void PrepareBody(string referenceUri)
        {
            XmlNodeList bodyNode = XmlDocument.GetElementsByTagName("Body", Xmlns.soapenv);

            if (bodyNode != null && bodyNode.Count == 1)
            {
                XmlElement body = bodyNode[0] as XmlElement;
                body.SetAttribute("xmlns:wsu", Xmlns.wsu);
                body.SetAttribute("Id", Xmlns.wsu, referenceUri);
            }
        }

        public override void Sign()
        {
            base.Sign();
            PrepareBody($"REGNO_{RegNum}");
            XmlDocumentHelper.Sign(XmlDocument, endpointBehavior.InsurerCertificate, $"insurer/{RegNum}", $"REGNO_{RegNum}");
        }
    }
}
