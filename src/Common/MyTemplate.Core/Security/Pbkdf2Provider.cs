using System;
using System.Security.Cryptography;
using System.Text;

namespace MyTemplate.Core.Security
{
    /// <summary>
    /// Modified based on https://github.com/aspnet/DataProtection/blob/8503e161d061fa230b00ba58d6801d7f906356ce/src/Microsoft.AspNetCore.Cryptography.KeyDerivation/PBKDF2/ManagedPbkdf2Provider.cs
    /// Only provide PBKDF2 with HMAC-SHA256
    /// </summary>
    internal sealed class Pbkdf2Provider
    {
        public byte[] DeriveKey(string password, byte[] salt, int iterationCount, int numBytesRequested)
        {
            // PBKDF2 is defined in NIST SP800-132, Sec. 5.3.
            // http://csrc.nist.gov/publications/nistpubs/800-132/nist-sp800-132.pdf

            byte[] retVal = new byte[numBytesRequested];
            int numBytesWritten = 0;
            int numBytesRemaining = numBytesRequested;

            // For each block index, U_0 := Salt || block_index
            byte[] saltWithBlockIndex = new byte[checked(salt.Length + sizeof(uint))];
            Buffer.BlockCopy(salt, 0, saltWithBlockIndex, 0, salt.Length);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var hashAlgorithm = new HMACSHA256(passwordBytes))
            {
                for (uint blockIndex = 1; numBytesRemaining > 0; blockIndex++)
                {
                    // write the block index out as big-endian
                    saltWithBlockIndex[saltWithBlockIndex.Length - 4] = (byte)(blockIndex >> 24);
                    saltWithBlockIndex[saltWithBlockIndex.Length - 3] = (byte)(blockIndex >> 16);
                    saltWithBlockIndex[saltWithBlockIndex.Length - 2] = (byte)(blockIndex >> 8);
                    saltWithBlockIndex[saltWithBlockIndex.Length - 1] = (byte)blockIndex;

                    // U_1 = PRF(U_0) = PRF(Salt || block_index)
                    // T_blockIndex = U_1
                    byte[] U_iter = hashAlgorithm.ComputeHash(saltWithBlockIndex); // this is U_1
                    byte[] T_blockIndex = U_iter;

                    for (int iter = 1; iter < iterationCount; iter++)
                    {
                        U_iter = hashAlgorithm.ComputeHash(U_iter);
                        XorBuffers(src: U_iter, dest: T_blockIndex);
                        // At this point, the 'U_iter' variable actually contains U_{iter+1} (due to indexing differences).
                    }

                    // At this point, we're done iterating on this block, so copy the transformed block into retVal.
                    int numBytesToCopy = Math.Min(numBytesRemaining, T_blockIndex.Length);
                    Buffer.BlockCopy(T_blockIndex, 0, retVal, numBytesWritten, numBytesToCopy);
                    numBytesWritten += numBytesToCopy;
                    numBytesRemaining -= numBytesToCopy;
                }
            }

            // retVal := T_1 || T_2 || ... || T_n, where T_n may be truncated to meet the desired output length
            return retVal;
        }


        private static void XorBuffers(byte[] src, byte[] dest)
        {
            // Note: dest buffer is mutated.
            for (int i = 0; i < src.Length; i++)
            {
                dest[i] ^= src[i];
            }
        }
    }
}
