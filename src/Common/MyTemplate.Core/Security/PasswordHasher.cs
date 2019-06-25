using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace MyTemplate.Core.Security
{
    /// <summary>
    /// Keep aligning with AspNetCore KeyDerivationPrf 
    /// https://github.com/aspnet/DataProtection/blob/8503e161d061fa230b00ba58d6801d7f906356ce/src/Microsoft.AspNetCore.Cryptography.KeyDerivation/KeyDerivationPrf.cs
    /// </summary>
    enum KeyDerivationPrf
    {
        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-1 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA1,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-256 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA256,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-512 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA512,
    }

    /// <summary>
    /// Modified based on https://github.com/aspnet/Identity/blob/dev/src/Microsoft.AspNetCore.Identity/PasswordHasher.cs
    /// In order to be compatible with ASP.NET Identity V2, use HMAC-SHA1 crypto, may change to SHA256 in future
    /// </summary>
    public class SimplePasswordHasher
    {
        /* =======================
         * HASHED PASSWORD FORMATS
         * =======================
         * 
         * PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10001 iterations.
         * Format: { 0x01, prf (UInt32), iter count (UInt32), salt length (UInt32), salt, subkey }
         * (All UInt32s are stored big-endian.)
         */

        private readonly RandomNumberGenerator _rng;
        private readonly Pbkdf2Provider _pbkdf2Provider;
        private const int _pbkdf2IterCount = 10001; // 10001 iterations
        private const int _pbkdf2SubkeyLength = 256 / 8; // 256 bits
        private const int _saltSize = 128 / 8; // 128 bits        


        /// <summary>
        /// Creates a new instance of <see cref="PasswordHasher{TUser}"/>.
        /// </summary>
        /// <param name="optionsAccessor">The options for this instance.</param>
        public SimplePasswordHasher()
        {
            _rng = RandomNumberGenerator.Create();
            _pbkdf2Provider = new Pbkdf2Provider();
        }

        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/>.</returns>
        public virtual string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Convert.ToBase64String(HashPassword(password, _rng, _pbkdf2Provider));
        }

        private byte[] HashPassword(string password, RandomNumberGenerator rng, Pbkdf2Provider pbkdf2Provider)
        {
            return HashPasswordV3(password,
                iterCount: _pbkdf2IterCount,
                saltSize: _saltSize,
                numBytesRequested: _pbkdf2SubkeyLength,
                rng: rng,
                pbkdf2Provider: pbkdf2Provider);
        }

        private static byte[] HashPasswordV3(string password, int iterCount, int saltSize, int numBytesRequested, RandomNumberGenerator rng, Pbkdf2Provider pbkdf2Provider)
        {
            // Produce a version 2 (see comment above) text hash.
            byte[] salt = new byte[_saltSize];
            rng.GetBytes(salt);
            byte[] subkey = pbkdf2Provider.DeriveKey(password, salt, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA256);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison.</param>
        /// <returns>A <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.</returns>
        /// <remarks>Implementations of this method should be time consistent.</remarks>
        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            if (decodedHashedPassword[0] == 0x01)
            {
                if (VerifyHashedPassword(decodedHashedPassword, providedPassword, _pbkdf2Provider))
                {
                    // If this hasher was configured with a higher iteration count, change the entry now.
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false; // unknown format marker
            }
        }

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password, Pbkdf2Provider pbkdf2Provider)
        {
            try
            {
                // Read header information
                //KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                var iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Lower iteration count will be denied.
                if (iterCount < _pbkdf2IterCount)
                {
                    return false;
                }
                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = pbkdf2Provider.DeriveKey(password, salt, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
