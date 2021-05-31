using System.IO;
using System.Xml;
using System.ServiceModel.Channels;

namespace FSSExtensions
{
    public class FSSMessage
    {
        protected WcfEndpointBehaviorBase endpointBehavior;

        bool isReply;

        public XmlDocument XmlDocument { get; protected set; }

        public FSSMessage(Message requestOrReplyMessage, WcfEndpointBehaviorBase _endpointBehavior)
        {
            endpointBehavior = _endpointBehavior;
            this.XmlDocument = Message2XmlDocument(requestOrReplyMessage);
        }

        public virtual void PrepareBeforeSendRequest()
        {
            this.Sign();
            if (endpointBehavior.UseEncryption)
            {
                this.Encrypt();
            }
        }

        public virtual void PrepareAfterReceiveReply()
        {
            if (endpointBehavior.UseEncryption)
            {
                this.Decrypt();
            }
        }

        static internal XmlDocument Message2XmlDocument(Message message)
        {            
            //MemoryStream messageStream = new MemoryStream();
            XmlDocument xmlDocument = new XmlDocument();

            //XmlWriter writer = XmlWriter.Create(messageStream);
            //message.WriteMessage(writer);
            //writer.Flush();
            //messageStream.Position = 0;

            xmlDocument.LoadXml(message.ToString());

            return xmlDocument;
        }

        static internal Message XmlDocument2Message(XmlDocument xmlDocument, MessageVersion version)
        {
            MemoryStream messageStream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(messageStream);
            xmlDocument.WriteTo(writer);
            writer.Flush();
            messageStream.Position = 0;
            XmlReader reader = XmlReader.Create(messageStream);
            Message message = Message.CreateMessage(reader, int.MaxValue, version);

            return message;
        }

        public virtual void Sign() { }
        public virtual void Encrypt() { }
        public virtual void Decrypt() { }
    }
}
