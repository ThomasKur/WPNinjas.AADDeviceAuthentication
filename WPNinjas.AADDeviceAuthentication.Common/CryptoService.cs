using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WPNinjas.AADDeviceAuthentication.Common
{
    public class CryptoService
    {
        public static string Sign(RSA privateKey, string content)
        {
            SHA256Managed sha1 = new SHA256Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(content);
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            byte[] signature = privateKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        public static bool Verify(RSA publicKey, string content, string signature)
        {
            SHA256Managed sha1 = new SHA256Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(content);
            byte[] hash = sha1.ComputeHash(data);
            return publicKey.VerifyHash(hash, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public static bool Verify(byte[] publicKeyBytes, string thumbprint, string azureADSecurityId, string content, string signature)
        {
            SHA256Managed sha1 = new SHA256Managed();
            // Verifiy cert
            azureADSecurityId = Base64StringDecodeUnicode(azureADSecurityId);
            string verifyValue = "X509:<SHA1-TP-PUBKEY>" + thumbprint + Base64StringEncode(sha1.ComputeHash(publicKeyBytes));
            if (!azureADSecurityId.Equals(verifyValue))
            {
                throw new Exception("Verification with Azure AD records failed");
            }
            // extract the modulus and exponent based on the key data
            byte[] exponentData = new byte[3];
            byte[] modulusData = new byte[256];
            Array.Copy(publicKeyBytes, publicKeyBytes.Length - exponentData.Length, exponentData, 0, exponentData.Length);
            Array.Copy(publicKeyBytes, 9, modulusData, 0, modulusData.Length);


            // import the public key data (base RSA - works)
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(dwKeySize: 2048);
            RSAParameters rsaParam = rsa.ExportParameters(false);
            rsaParam.Modulus = modulusData;
            rsaParam.Exponent = exponentData;
            rsa.ImportParameters(rsaParam);

            return Verify(rsa, content, signature);
        }

        public static string Encrypt(string textToEncrypt, RSA publicKey){
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(textToEncrypt);
            byte[] encryptedData = publicKey.EncryptValue(data);
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedText, RSA privateKey)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedText);
            byte[] data = privateKey.DecryptValue(encryptedData);
            UnicodeEncoding encoding = new UnicodeEncoding();
            string text = encoding.GetString(data);
            return text;
        }

        private static string Base64StringDecodeUnicode(string encodedString)
        {
            var bytes = Convert.FromBase64String(encodedString);

            var decodedString = Encoding.Unicode.GetString(bytes);
            return decodedString;
        }

        private static string Base64StringEncode(byte[] originalByte)
        {

            var encodedString = Convert.ToBase64String(originalByte);

            return encodedString;
        }
    }
}
