// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Security.Cryptography;
using System.Text;

namespace IdentityModel
{
    /// <summary>
    /// A class that mimics the standard Random class in the .NET Framework - but uses a random number generator internally.
    /// </summary>
    public class CryptoRandom : Random
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private readonly byte[] _uint32Buffer = new byte[4];

        /// <summary>
        /// Creates a random key byte array.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static byte[] CreateRandomKey(int length)
        {
            var bytes = new byte[length];
            Rng.GetBytes(bytes);

            return bytes;
        }

        /// <summary>
        /// Creates a random key as base64 encoded string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string CreateRandomKeyString(int length)
        {
            var bytes = new byte[length];
            Rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Creates a URL safe unique identifier.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string CreateUniqueId(int length = 32)
        {
            var bytes = new byte[length];
            Rng.GetBytes(bytes);

            return ByteArrayToString(bytes);
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom"/> class.
        /// </summary>
        public CryptoRandom() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoRandom"/> class.
        /// </summary>
        /// <param name="ignoredSeed">seed (ignored)</param>
        public CryptoRandom(Int32 ignoredSeed) { }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than <see cref="F:System.Int32.MaxValue"/>.
        /// </returns>
        public override Int32 Next()
        {
            Rng.GetBytes(_uint32Buffer);
            return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. <paramref name="maxValue"/> must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than <paramref name="maxValue"/>; that is, the range of return values ordinarily includes zero but not <paramref name="maxValue"/>. However, if <paramref name="maxValue"/> equals zero, <paramref name="maxValue"/> is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="maxValue"/> is less than zero.
        /// </exception>
        public override Int32 Next(Int32 maxValue)
        {
            if (maxValue < 0) throw new ArgumentOutOfRangeException(nameof(maxValue));
            return Next(0, maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but not <paramref name="maxValue"/>. If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
        /// </exception>
        public override Int32 Next(Int32 minValue, Int32 maxValue)
        {
            if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue) return minValue;
            Int64 diff = maxValue - minValue;

            while (true)
            {
                Rng.GetBytes(_uint32Buffer);
                UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                Int64 max = (1 + (Int64)UInt32.MaxValue);
                Int64 remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (Int32)(minValue + (rand % diff));
                }
            }
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        public override double NextDouble()
        {
            Rng.GetBytes(_uint32Buffer);
            UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);
            return rand / (1.0 + UInt32.MaxValue);
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="buffer"/> is null.
        /// </exception>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            Rng.GetBytes(buffer);
        }
    }
}