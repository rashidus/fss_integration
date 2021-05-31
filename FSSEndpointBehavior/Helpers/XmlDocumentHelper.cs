using System;
using System.Xml;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using GostCryptography.Base;
using GostCryptography.Xml;

namespace FSSExtensions
{    
    internal class FSSSignedXml : SignedXml
    {
        internal FSSSignedXml(XmlDocument document) : base(document)
        {

        }
        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
            manager.AddNamespace("wsu", Xmlns.wsu);
            var el = document.SelectSingleNode($"//*[@wsu:Id=\"{idValue}\"]", manager);
            return el as XmlElement;
        }
    }

    internal static class XmlDocumentHelper
    {
        #region ЭЛН 2.0 расшифровка
        public static XmlDocument Decrypt(XmlDocument encryptedXmlDocument, X509Certificate2 insurerCertificate)
        {
            XmlNodeList encDataNodes = encryptedXmlDocument.DocumentElement.GetElementsByTagName("xenc:EncryptedData");
            encryptedXmlDocument = new XmlDocument();
            encryptedXmlDocument.LoadXml(encDataNodes.Item(0).OuterXml);

            if (insurerCertificate != null)
            {
                XmlNodeList certificatesNodes = encryptedXmlDocument.DocumentElement.GetElementsByTagName("ds:X509Certificate");
                XmlElement xml = new KeyInfoX509Data(insurerCertificate).GetXml();
                certificatesNodes.Item(0).FirstChild.Value = xml.FirstChild.FirstChild.Value;
            }

            GostCryptography.Xml.GostEncryptedXml gostEncryptedXml = new GostCryptography.Xml.GostEncryptedXml(encryptedXmlDocument);
            XmlElement documentElement = encryptedXmlDocument.DocumentElement;
            if (documentElement != null)
            {
                EncryptedData encryptedData = new EncryptedData();
                encryptedData.LoadXml(documentElement);
                SymmetricAlgorithm decryptionKey = GetDecryptionKey(encryptedData);
                if (decryptionKey != null)
                {
                    byte[] decryptedData = gostEncryptedXml.DecryptData(encryptedData, decryptionKey);
                    gostEncryptedXml.ReplaceData(documentElement, decryptedData);
                }
            }
            return encryptedXmlDocument;
        }

        private static SymmetricAlgorithm GetDecryptionKey(EncryptedData encryptedData)
        {
            SymmetricAlgorithm symmetricAlgorithm = (SymmetricAlgorithm)null;
            foreach (object obj1 in encryptedData.KeyInfo)
            {
                if (obj1 is KeyInfoEncryptedKey)
                {
                    EncryptedKey encryptedKey = ((KeyInfoEncryptedKey)obj1).EncryptedKey;
                    if (encryptedKey != null)
                    {
                        foreach (object obj2 in encryptedKey.KeyInfo)
                        {
                            if (obj2 is KeyInfoX509Data)
                            {
                                ArrayList certificates = ((KeyInfoX509Data)obj2).Certificates;
                                X509Certificate2 certificate = (X509Certificate2)null;
                                foreach (X509Certificate2 x509Certificate2 in certificates)
                                {
                                    //certificate = GostCertificatesDispatcher.FindCertificate(x509Certificate2.SerialNumber); //TODO
                                    certificate = SecurityUtils.GetCertificateFromStoreCore(StoreName.My, StoreLocation.CurrentUser, X509FindType.FindBySerialNumber, x509Certificate2.SerialNumber);
                                    if (certificate != null)
                                    {
                                        break;
                                    }

                                }
                                if (System.Security.Cryptography.X509Certificates.X509CertificateHelper.GetPrivateKeyAlgorithm(certificate) is GostCryptography.Base.GostAsymmetricAlgorithm privateKeyAlgorithm7)
                                {
                                    symmetricAlgorithm = GostCryptography.Xml.GostEncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, privateKeyAlgorithm7);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return symmetricAlgorithm;
        }
        #endregion

        #region ЭЛН 2.0 шифрование
        public static XmlDocument Encrypt(XmlDocument xmlDocument, X509Certificate2 fssCertificate, X509Certificate2 insurerCertificate)
        {
            XmlDocument xmlAppendedSenderCert = AppendInsurerCertificate(xmlDocument, insurerCertificate);

            GostCryptography.Config.GostCryptoConfig.ProviderType = GostCryptography.Base.ProviderType.CryptoPro;
            GostCryptography.Config.GostCryptoConfig.ProviderType_2012_512 = GostCryptography.Base.ProviderType.CryptoPro_2012_512;
            GostCryptography.Config.GostCryptoConfig.ProviderType_2012_1024 = GostCryptography.Base.ProviderType.CryptoPro_2012_1024;
            GostCryptography.Xml.GostEncryptedXml gostEncryptedXml = new GostCryptography.Xml.GostEncryptedXml(GostCryptography.Base.ProviderType.CryptoPro);
            XmlElement documentElement = xmlAppendedSenderCert.DocumentElement;
            if (documentElement != null)
            {
                EncryptedData encryptedData = new EncryptedData();
                encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
                encryptedData.KeyInfo = new KeyInfo();

                encryptedData.EncryptionMethod = new EncryptionMethod("urn:ietf:params:xml:ns:cpxmlsec:algorithms:gost28147");
                using (GostCryptography.Gost_28147_89.Gost_28147_89_SymmetricAlgorithm symmetricAlgorithm_orig = new GostCryptography.Gost_28147_89.Gost_28147_89_SymmetricAlgorithm(GostCryptography.Base.ProviderType.CryptoPro_2012_512))
                {
                    byte[] numArray = gostEncryptedXml.EncryptData(documentElement, symmetricAlgorithm_orig, false);
                    byte[] cipherValue = GostEncryptedXml.EncryptKey(symmetricAlgorithm_orig, (GostAsymmetricAlgorithm)fssCertificate.GetPublicKeyAlgorithm());
                    EncryptedKey encryptedKey = new EncryptedKey();
                    encryptedKey.CipherData = new CipherData(cipherValue);
                    encryptedKey.EncryptionMethod = new EncryptionMethod("urn:ietf:params:xml:ns:cpxmlsec:algorithms:transport-gost2001");
                    encryptedKey.KeyInfo.AddClause((KeyInfoClause)new KeyInfoX509Data(fssCertificate));
                    encryptedData.KeyInfo.AddClause((KeyInfoClause)new KeyInfoEncryptedKey(encryptedKey));
                    encryptedData.CipherData.CipherValue = numArray;
                }

                GostEncryptedXml.ReplaceElement(documentElement, encryptedData, false);
            }
            string result = "<?xml version=\"1.0\"?>" +
                            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                            "<soapenv:Header/>" +
                            "<soapenv:Body>" +
                            $"{xmlAppendedSenderCert.DocumentElement.OuterXml}" +
                            "</soapenv:Body>" +
                            "</soapenv:Envelope>";
            XmlDocument newXmlDocument = new XmlDocument();
            newXmlDocument.LoadXml(result);

            return newXmlDocument;
        }

        private static XmlNamespaceManager GetNameTable(XmlDocument x)
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(x.NameTable);
            namespaceManager.AddNamespace("soap-env", Xmlns.soapenv);
            namespaceManager.AddNamespace("xenc", Xmlns.xmlenc);
            namespaceManager.AddNamespace("s", Xmlns.soapenv);
            namespaceManager.AddNamespace("soap", Xmlns.soapenv);
            namespaceManager.AddNamespace("soapenv", Xmlns.soapenv);
            namespaceManager.AddNamespace("wsse", Xmlns.wsse);
            namespaceManager.AddNamespace("ns1", Xmlns.eln);
            namespaceManager.AddNamespace("eln", Xmlns.eln);
            namespaceManager.AddNamespace("ds", Xmlns.ds);
            return namespaceManager;
        }

        private static XmlDocument AppendInsurerCertificate(XmlDocument xmlDocument, X509Certificate2 insurerCertificate)
        {
            XmlNamespaceManager nameTable = GetNameTable(xmlDocument);
            XmlNode node = xmlDocument.CreateNode("element", "ds:X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
            node.InnerText = Convert.ToBase64String(insurerCertificate.RawData);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("/soapenv:Envelope/soapenv:Header", nameTable);
            xmlNode.AppendChild(node);
            return xmlDocument;
        }
        #endregion

        public static void Sign(XmlDocument document, X509Certificate2 certificate, string wsseReferencePostfix, string referenceUri)
        {
            XmlElement xmlDigitalSignature = GenerateSecurityToken(document, certificate, wsseReferencePostfix, referenceUri);

            XmlNodeList nodeList = document.GetElementsByTagName("Header", Xmlns.soapenv);

            if (nodeList != null && nodeList.Count == 1)
            {
                XmlElement security = document.CreateElement("wsse", "Security", Xmlns.wsse);
                security.SetAttribute("actor", Xmlns.soapenv, $"{Xmlns.wsseReferenceURI}{wsseReferencePostfix}");
                security.SetAttribute("xmlns:wsu", Xmlns.wsu);
                security.SetAttribute("xmlns:ds", Xmlns.ds);
                nodeList[0].AppendChild(security);

                XmlElement binarySecurityToken = document.CreateElement("wsse", "BinarySecurityToken", Xmlns.wsse);
                binarySecurityToken.SetAttribute("EncodingType", Xmlns.encodingType);
                binarySecurityToken.SetAttribute("ValueType", Xmlns.valueType);
                binarySecurityToken.SetAttribute("Id", Xmlns.wsu, $"{Xmlns.wsseReferenceURI}{wsseReferencePostfix}");
                binarySecurityToken.InnerText = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
                security.AppendChild(binarySecurityToken);

                security.AppendChild(xmlDigitalSignature);
            }
        }

        private static XmlElement GenerateSecurityToken(XmlDocument document, X509Certificate2 certificate, string wsseReferencePostfix, string referenceUri)
        {
            XmlElement keyReference = document.CreateElement("wsse", "Reference", Xmlns.wsse);
            keyReference.SetAttribute("URI", $"#{Xmlns.wsseReferenceURI}{wsseReferencePostfix}");

            XmlElement keySecurityTokenReference = document.CreateElement("wsse", "SecurityTokenReference", Xmlns.wsse);
            keySecurityTokenReference.AppendChild(keyReference);

            KeyInfoNode keyInfoData = new KeyInfoNode(keySecurityTokenReference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(keyInfoData);

            Reference reference = new Reference
            {
                Uri = $"#{referenceUri}",
                DigestMethod = Xmlns.digestMethod
            };

            reference.AddTransform(new XmlDsigExcC14NTransform());

            FSSSignedXml signedXml = new FSSSignedXml(document)
            //GostSignedXml signedXml = new GostSignedXml(document)
            {
                SigningKey = certificate.GetPrivateKeyAlgorithm(), //TODO
                KeyInfo = keyInfo,
            };
            signedXml.AddReference(reference);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
#pragma warning disable CS0612
            signedXml.SignedInfo.SignatureMethod = GetSignatureMethod(certificate);
#pragma warning restore CS0612
            signedXml.ComputeSignature();

            return signedXml.GetXml();
        }

        private static string GetSignatureMethod(X509Certificate2 certificate)
        {
            // Имя алгоритма вычисляем динамически, чтобы сделать код теста универсальным

            using (var publicKey = (GostCryptography.Base.GostAsymmetricAlgorithm)certificate.GetPublicKeyAlgorithm()) //TODO
            {
                return publicKey.SignatureAlgorithm;
            }
        }
    }
    //internal class FssMessage
    //{
    //    public X509Certificate2 InsurerCertificate { get; set; }
    //    public X509Certificate2 ChiefCertificate { get; set; }
    //    public X509Certificate2 AccountantCertificate { get; set; }
    //    public X509Certificate2 FssCertificate { get; set; }
    //    public XmlDocument Message { get; set; }
    //    public string RegNum { get; set; }

    //    public static FssMessage getInstance(XmlDocument message)
    //    {
    //        XmlNodeList nodeList = message.GetElementsByTagName("getPrivateLNDataRequest", Xmlns.fssWsdl);

    //        if (nodeList != null && nodeList.Count == 1)
    //        {
    //            return new FSSMessageGet(message);
    //        }
    //        nodeList = message.GetElementsByTagName("prParseReestrFileRequest", Xmlns.fssWsdl);

    //        if (nodeList != null && nodeList.Count == 1)
    //        {
    //            return new FSSMessageSet(message);
    //        }

    //        throw new Exception("Неизвестный тип сообщения");

    //    }
    //    public FssMessage(XmlDocument message)
    //    {
    //        Message = message;
    //    }

    //    public virtual XmlDocument PrepareMessage()
    //    {
    //        return Message;
    //    }

    //    protected virtual void PrepareNamespaces(XmlDocument document)
    //    {
    //        XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);

    //        ns.AddNamespace("soapenv", Xmlns.soapenv);
    //        ns.AddNamespace("ds", Xmlns.ds);
    //        ns.AddNamespace("wsse", Xmlns.wsse);
    //        ns.AddNamespace("wsu", Xmlns.wsu);
    //        ns.AddNamespace("xsd", Xmlns.xsd);
    //        ns.AddNamespace("xsi", Xmlns.xsi);
    //        ns.AddNamespace("xenc", Xmlns.xmlenc);
    //        ns.AddNamespace("sch", Xmlns.sch);
    //    }

    //    public virtual void AddSecurityToken(XmlDocument document, X509Certificate2 certificate, string wsseReferencePostfix, string referenceUri)
    //    {
    //        XmlElement xmlDigitalSignature = GenerateSecurityToken(document, certificate, wsseReferencePostfix, referenceUri);

    //        XmlNodeList nodeList = document.GetElementsByTagName("Header", Xmlns.soapenv);

    //        if (nodeList != null && nodeList.Count == 1)
    //        {
    //            XmlElement security = document.CreateElement("wsse", "Security", Xmlns.wsse);
    //            security.SetAttribute("actor", Xmlns.soapenv, $"{Xmlns.wsseReferenceURI}{wsseReferencePostfix}");
    //            security.SetAttribute("xmlns:wsu", Xmlns.wsu);
    //            security.SetAttribute("xmlns:ds", Xmlns.ds);
    //            nodeList[0].AppendChild(security);

    //            XmlElement binarySecurityToken = document.CreateElement("wsse", "BinarySecurityToken", Xmlns.wsse);
    //            binarySecurityToken.SetAttribute("EncodingType", Xmlns.encodingType);
    //            binarySecurityToken.SetAttribute("ValueType", Xmlns.valueType);
    //            binarySecurityToken.SetAttribute("Id", Xmlns.wsu, $"{Xmlns.wsseReferenceURI}{wsseReferencePostfix}");
    //            binarySecurityToken.InnerText = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
    //            security.AppendChild(binarySecurityToken);

    //            security.AppendChild(xmlDigitalSignature);
    //        }
    //    }

    //    protected virtual void InitRegNum()
    //    {
    //        XmlNodeList nodeList = Message.GetElementsByTagName("regNum", Xmlns.fssWsdl);

    //        if (nodeList != null && nodeList.Count == 1)
    //        {
    //            XmlElement regNumElement = nodeList[0] as XmlElement;

    //            if (String.IsNullOrEmpty(RegNum))
    //            {
    //                RegNum = regNumElement.InnerText;
    //            }
    //        }
    //    }
    //}

    //internal class FSSMessageGet : FssMessage
    //{
    //    public FSSMessageGet(XmlDocument message) : base(message)
    //    {
    //    }

    //    public override XmlDocument PrepareMessage()
    //    {
    //        InitRegNum();
    //        PrepareNamespaces(Message);
    //        PrepareBody(Message, $"REGNO_{RegNum}");
    //        AddSecurityToken(Message, InsurerCertificate, $"insurer/{RegNum}", $"REGNO_{RegNum}");
    //        return Message;
    //    }
    //    protected void PrepareBody(XmlDocument document, string referenceUri)
    //    {
    //        XmlNodeList bodyNode = document.GetElementsByTagName("Body", Xmlns.soapenv);

    //        if (bodyNode != null && bodyNode.Count == 1)
    //        {
    //            XmlElement body = bodyNode[0] as XmlElement;
    //            body.SetAttribute("xmlns:wsu", Xmlns.wsu);
    //            body.SetAttribute("Id", Xmlns.wsu, referenceUri);
    //        }
    //    }
    //}

    //internal class FSSMessageSet : FssMessage
    //{
    //    public FSSMessageSet(XmlDocument message) : base(message)
    //    {
    //    }
    //    public override XmlDocument PrepareMessage()
    //    {
    //        InitRegNum();
    //        PrepareNamespaces(Message);

    //        XmlNodeList rowNode = Message.GetElementsByTagName("row", Xmlns.fssWsdl);

    //        foreach (XmlElement row in rowNode)
    //        {
    //            XmlNodeList lnCode = row.GetElementsByTagName("lnCode", Xmlns.fssWsdl);

    //            if (lnCode != null && lnCode.Count == 1)
    //            {
    //                XmlElement lnCodeTag = lnCode[0] as XmlElement;
    //                string lnNum = lnCodeTag.InnerText;

    //                if (!String.IsNullOrEmpty(lnNum))
    //                {
    //                    row.RemoveAllAttributes();

    //                    row.SetAttribute("xmlns:wsu", Xmlns.wsu);
    //                    row.SetAttribute("Id", Xmlns.wsu, $"ELN_{lnNum}");

    //                    AddSecurityToken(Message, InsurerCertificate, $"insurer/{RegNum}/{lnNum}", $"ELN_{lnNum}");
    //                    AddSecurityToken(Message, ChiefCertificate, $"chief/{RegNum}/{lnNum}", $"ELN_{lnNum}");
    //                    AddSecurityToken(Message, AccountantCertificate, $"accountant/{RegNum}/{lnNum}", $"ELN_{lnNum}");
    //                }
    //            }
    //        }
    //        return Message;
    //    }
    //}
}
