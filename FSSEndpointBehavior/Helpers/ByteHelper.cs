using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

namespace FSSExtensions
{
    class ByteHelper
    {
        public static byte[] Sign(byte[] unsignedFile, X509Certificate2 cert)
        {
            // Создание объекта для подписи сообщения
            var signedCms = new GostCryptography.Pkcs.GostSignedCms(new ContentInfo(unsignedFile));

            // Создание объект с информацией о подписчике
            var signer = new CmsSigner(cert);

            // Включение информации только о конечном сертификате (только для теста)
            signer.IncludeOption = X509IncludeOption.EndCertOnly;

            // Создание подписи для сообщения CMS/PKCS#7
            signedCms.ComputeSignature(signer);

            // Создание сообщения CMS/PKCS#7
            return signedCms.Encode();
        }
        public static byte[] Encrypt(byte[] unencrypedFile, X509Certificate2 cert)
        {
            // Создание объекта для шифрования сообщения
            var envelopedCms = new EnvelopedCms(new ContentInfo(unencrypedFile));

            // Создание объект с информацией о получателе
            var recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, cert);

            // Шифрование сообщения CMS/PKCS#7
            envelopedCms.Encrypt(recipient);

            // Создание сообщения CMS/PKCS#7
            return envelopedCms.Encode();
        }
        public static byte[] SignAndEncrypt(byte[] file, X509Certificate2 certForSign, X509Certificate2 certForEncrypt)
        {
            return Encrypt(Sign(file, certForSign), certForEncrypt);
        }
        public static byte[] Decrypt(byte[] encryptedFile)
        {
            EnvelopedCms envelopedCms = new EnvelopedCms();

            // Чтение сообщения CMS/PKCS#7
            envelopedCms.Decode(encryptedFile);

            // Расшифровка сообщения CMS/PKCS#7
            envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);

            byte[] decryptedFile = envelopedCms.ContentInfo.Content;

            return decryptedFile;
        }
    }
}
