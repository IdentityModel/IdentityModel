#if NETSTANDARD1_6

using System.Security.Cryptography;

namespace Hawk.Middleware
{
    public class HashAlgoirthmFactory
    {
        public static HashAlgorithm Create(string hashAlgorithmName)
        {
            if (hashAlgorithmName == HashAlgorithmName.MD5.Name)
            {
                return MD5.Create();
            }
            else if (hashAlgorithmName == "hmac" + HashAlgorithmName.MD5.Name)
            {
                return new HMACMD5();
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA1.Name)
            {
                return SHA1.Create();
            }
            else if (hashAlgorithmName == "hmac" + HashAlgorithmName.SHA1.Name)
            {
                return new HMACSHA1();
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA256.Name)
            {
                return SHA256.Create();
            }
            else if (hashAlgorithmName == "hmac" + HashAlgorithmName.SHA256.Name)
            {
                return new HMACSHA256(); 
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA384.Name)
            {
                return SHA384.Create();
            }
            else if (hashAlgorithmName == "hmac" + HashAlgorithmName.SHA384.Name)
            {
                return new HMACSHA384();
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA512.Name)
            {
                return SHA512.Create();
            }
            else if (hashAlgorithmName == "hmac" + HashAlgorithmName.SHA512.Name)
            {
                return new HMACSHA512(); 
            }

            return null;

        }
    }
}
#endif