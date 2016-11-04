using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Plus.Communication.Encryption.KeyExchange
{
    public class Randomizer
    {
        private static Random rand = new Random();

        public static Random GetRandom
        {
            get
            {
                return rand;
            }
        }

        public static int Next()
        {
            return rand.Next();
        }

        public static int Next(int max)
        {
            return rand.Next(max);
        }

        public static int Next(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static double NextDouble()
        {
            return rand.NextDouble();
        }

        public static byte NextByte()
        {
            return (byte)Next(0, 255);
        }

        public static byte NextByte(int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(0, max);
        }

        public static byte NextByte(int min, int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(Math.Min(min, max), max);
        }

        public static void NextBytes(byte[] toparse)
        {
            rand.NextBytes(toparse);
        }
    }

    public class DiffieHellman
    {
        public readonly int BITLENGTH = 32;

        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }

        private BigInteger PrivateKey;
        public BigInteger PublicKey { get; private set; }

        public DiffieHellman()
        {
            this.Initialize();
        }

        public DiffieHellman(int b)
        {
            this.BITLENGTH = b;

            this.Initialize();
        }

        public DiffieHellman(BigInteger prime, BigInteger generator)
        {
            this.Prime = prime;
            this.Generator = generator;

            this.Initialize(true);
        }

        private void Initialize(bool ignoreBaseKeys = false)
        {
            this.PublicKey = 0;

            Random rand = new Random();
            while (this.PublicKey == 0)
            {
                if (!ignoreBaseKeys)
                {
                    this.Prime = BigInteger.genPseudoPrime(BITLENGTH, 10, rand);
                    this.Generator = BigInteger.genPseudoPrime(BITLENGTH, 10, rand);
                }

                byte[] bytes = new byte[this.BITLENGTH / 8];
                Randomizer.NextBytes(bytes);
                this.PrivateKey = new BigInteger(bytes);

                if (this.Generator > this.Prime)
                {
                    BigInteger temp = this.Prime;
                    this.Prime = this.Generator;
                    this.Generator = temp;
                }

                this.PublicKey = this.Generator.modPow(this.PrivateKey, this.Prime);

                if (!ignoreBaseKeys)
                {
                    break;
                }
            }
        }

        public BigInteger CalculateSharedKey(BigInteger m)
        {
            return m.modPow(this.PrivateKey, this.Prime);
        }
    }
}

