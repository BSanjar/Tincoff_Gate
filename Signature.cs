using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Tincoff_Gate
{
    public class Signature
    {
        //public string GetSign(string originatorSignature)
        //{
        //    try
        //    {
        //        string path = Directory.GetCurrentDirectory();
        //        string PrivateKeyPath = path+ "\\Keys\\private.pem";

        //        byte[] messageAsByte = Encoding.ASCII.GetBytes(originatorSignature);


        //        RSACryptoServiceProvider RSA = GetPrivateKeyFromPemFile(PrivateKeyPath);


        //        RSAParameters rsaParams = RSA.ExportParameters(true);

        //        RSACng RSACng = new RSACng();
        //        RSACng.ImportParameters(rsaParams);

        //        byte[] signatureByte = RSACng.SignData(messageAsByte, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        //        var t = BitConverter.ToString(signatureByte).Replace("-", string.Empty);
        //        return t;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }           
        //}


        //public string GetSign(string originatorSignature)
        //{
        //    try
        //    {
        //        // encoding my privateKey from string to byte[] by using DecodeOpenSSLPrivateKey function from OpenSSLKey source code
        //        byte[] pemprivatekey = DecodeOpenSSLPrivateKey(privateKey);

        //        // enconding my string to sign in byte[]
        //        byte[] byteSign = Encoding.ASCII.GetBytes(Sign);

        //        // using DecodeRSAPrivateKey function from OpenSSLKey source code to get the RSACryptoServiceProvider with all needed parameters
        //        var rsa = DecodeRSAPrivateKey(pemprivatekey);

        //        // Signing my string with previously get RSACryptoServiceProvider in SHA256
        //        var byteRSA = rsa.SignData(byteSign, CryptoConfig.MapNameToOID("SHA256"));

        //        // As required by docs converting the signed string to base64
        //        string Signature = Convert.ToBase64String(byteRSA);
        //        return t;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}




        //public bool Verify(string text, string originatorSignature)

        //{
        //    string path = Directory.GetCurrentDirectory();
        //    string PrivateKeyPath = path + "\\Keys\\private.key";

        //    byte[] messageAsByte = Encoding.ASCII.GetBytes(originatorSignature);

        //    X509Certificate2 cert = new X509Certificate2(path);

        //    // Note:

        //    // If we want to use the client cert in an ASP.NET app, we may use something like this instead:

        //    // X509Certificate2 cert = new X509Certificate2(Request.ClientCertificate.Certificate);


        //    // Get its associated CSP and public key

        //    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;


        //    // Hash the data

        //    SHA1Managed sha1 = new SHA1Managed();

        //    UnicodeEncoding encoding = new UnicodeEncoding();

        //    byte[] data = encoding.GetBytes(text);

        //    byte[] hash = sha1.ComputeHash(data);


        //    // Verify the signature with the hash

        //    return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), messageAsByte);

        //}
        //public bool CheckSign(string text, string originatorSignature)
        //{
        //    try
        //    {
        //        string path = Directory.GetCurrentDirectory();
        //        string PrivateKeyPath = path + "\\Keys\\public_platform.pem";
        //        RSACryptoServiceProvider csp = GetPrivateKeyFromPemFile(PrivateKeyPath);


        //        SHA1Managed sha1 = new SHA1Managed();

        //        UnicodeEncoding encoding = new UnicodeEncoding();
        //        byte[] messageAsByte = Encoding.ASCII.GetBytes(originatorSignature);

        //        byte[] data = encoding.GetBytes(text);

        //        byte[] hash = sha1.ComputeHash(data);


        //        // Verify the signature with the hash

        //        return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), messageAsByte);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        //public bool CheckSign2(string text, string originatorSignature)
        //{
        //    try
        //    {
        //        string path = Directory.GetCurrentDirectory();
        //        string PrivateKeyPath = path + "\\Keys\\public_platform.key";
        //        // Access Personal (MY) certificate store of current user

        //        X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //        my.Open(OpenFlags.ReadOnly);
        //        // Find the certificate we'll use to sign

        //        RSACryptoServiceProvider csp = null;

        //        foreach (X509Certificate2 cert in my.Certificates)

        //        {

        //            if (cert.Subject.Contains(originatorSignature))

        //            {

        //                // We found it.

        //                // Get its associated CSP and private key

        //                csp = (RSACryptoServiceProvider)cert.PrivateKey;

        //            }

        //        }

        //        if (csp == null)

        //        {

        //            throw new Exception("No valid cert was found");

        //        }


        //        // Hash the data

        //        SHA1Managed sha1 = new SHA1Managed();

        //        UnicodeEncoding encoding = new UnicodeEncoding();

        //        byte[] data = encoding.GetBytes(text);

        //        byte[] hash = sha1.ComputeHash(data);


        //        // Sign the hash
        //        byte[] signatureByte = RSACng.SignData(messageAsByte, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        //        var t = BitConverter.ToString(signatureByte).Replace("-", string.Empty);

        //        return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public static RSACryptoServiceProvider GetPrivateKeyFromPemFile(string filePath)
        //{
        //    using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(filePath)))
        //    {
        //        //PemReader pr = new PemReader(privateKeyTextReader).ReadObject(); 

        //        AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

        //        RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
        //        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
        //        csp.ImportParameters(rsaParams);
        //        return csp;
        //    }
        //}

        //public static RSACryptoServiceProvider GetPrivateKeyFromPemFile2(string filePath)
        //{
        //    using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(filePath)))
        //    {
        //        //PemReader pr = new PemReader(privateKeyTextReader).ReadObject(); 

        //        AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

        //        RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Public);
        //        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
        //        csp.ImportParameters(rsaParams);
        //        return csp;
        //    }
        //}


        private RSAParameters _privateKey;
        private RSAParameters _publicKey;
        public Signature()
        {

            string path = Directory.GetCurrentDirectory();
            //string PrivateKeyPath = path+ "\\Keys\\private.pem";
            string public_pem = path + "\\Keys\\public_platform.key";
            string private_pem = path + "\\Keys\\private.key";


            var pub = Signature.GetPublicKeyFromPemFile(public_pem);
            var pri = Signature.GetPrivateKeyFromPemFile(private_pem);


            _publicKey = pub.ExportParameters(false);
            _privateKey = pri.ExportParameters(true);
        }


        public byte[] GetHash(string plaintext)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
        }

        public string SignData(string text)
        {
            //byte[] bytes_in_for_digesting = Encoding.UTF8.GetBytes(for_digesting);
            byte[] hashOfDataToSign = GetHash(text);
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(_privateKey);
                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");
                var sign = rsaFormatter.CreateSignature(hashOfDataToSign);
                string sign_str = System.Convert.ToBase64String(sign);
                return sign_str;
            }
        }


        


        public bool VerifySignature(string text, string  signature_str)
        {
            byte[] signature = System.Convert.FromBase64String(signature_str);
            byte[] hashOfDataToSign = GetHash(text);


            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(_publicKey);
                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                return rsaDeformatter.VerifySignature(hashOfDataToSign, signature);
            }
        }


        public static RSACryptoServiceProvider GetPrivateKeyFromPemFile(string filePath)
        {
            using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(filePath)))
            {
                AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
                csp.ImportParameters(rsaParams);
                return csp;
            }
        }

        public static RSACryptoServiceProvider GetPublicKeyFromPemFile(String filePath)
        {
            using (TextReader publicKeyTextReader = new StringReader(File.ReadAllText(filePath)))
            {
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)new PemReader(publicKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKeyParam);

                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
                csp.ImportParameters(rsaParams);
                return csp;
            }
        }

    }
}
