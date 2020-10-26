namespace DxRandom
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public abstract class AbstractRandom : IRandom
    {
        private static readonly ConcurrentDictionary<Type, Array> EnumTypeCache = new ConcurrentDictionary<Type, Array>();

        protected const uint HalfwayUint = uint.MaxValue / 2;
        protected const uint IntMax = int.MaxValue + 1U;
        protected const double MagicDouble = 4.6566128752458E-10;
        protected const float MagicFloat = 5.960465E-008F;

        protected AbstractRandom() { }

        public int Next()
        {
            unchecked
            {
                return (int)NextUint();
            }
        }

        public int Next(int max)
        {
            if (max <= 0)
            {
                throw new ArgumentException($"Max {max} cannot be less-than or equal-to 0");
            }

            return unchecked((int)NextUint(unchecked((uint)max)));
        }

        public int Next(int min, int max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to max {max}");
            }

            uint range = unchecked((uint)(max - min));
            return unchecked((int)(NextUint(range) + min));
        }

        // Internal sampler
        public abstract uint NextUint();

        public uint NextUint(uint max)
        {
            /*
                https://github.com/libevent/libevent/blob/3807a30b03ab42f2f503f2db62b1ef5876e2be80/arc4random.c#L531

                http://cs.stackexchange.com/questions/570/generating-uniformly-distributed-random-numbers-using-a-coin
                Generates a uniform random number within the bound, avoiding modulo bias
            */
            uint threshold = unchecked((uint)((0x100000000UL - max) % max));
            while (true)
            {
                uint randomValue = NextUint();
                if (threshold <= randomValue)
                {
                    return randomValue % max;
                }
            }
        }

        public uint NextUint(uint min, uint max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to max {max}");
            }

            return min + NextUint(max - min);
        }

        public short NextShort()
        {
            return NextShort(short.MaxValue);
        }

        public short NextShort(short max)
        {
            return NextShort(0, max);
        }

        public short NextShort(short min, short max)
        {
            return unchecked((short)Next(min, max));
        }

        public byte NextByte()
        {
            return NextByte(byte.MaxValue);
        }

        public byte NextByte(byte max)
        {
            return NextByte(0, max);
        }

        public byte NextByte(byte min, byte max)
        {
            return unchecked((byte)Next(min, max));
        }

        public long NextLong()
        {
            uint upper = NextUint();
            uint lower = NextUint();
            // Mix things up a little
            if (NextBool())
            {
                return unchecked((long)((ulong)upper << 32) | lower);
            }
            return unchecked((long)((ulong)lower << 32) | upper);
        }

        public long NextLong(long max)
        {
            if (max <= 0)
            {
                throw new ArgumentException($"Max {max} cannot be less-than or equal-to 0");
            }

            long withinRange;
            while (max < (withinRange = NextLong()))
            {
                // Hot-loop
            }
            return withinRange;
        }

        public long NextLong(long min, long max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to Max {max}");
            }

            return min + NextLong(max - min);
        }

        public ulong NextUlong()
        {
            return unchecked((ulong)NextLong());
        }

        public ulong NextUlong(ulong max)
        {
            return unchecked((ulong)NextLong(unchecked((long)max)));
        }

        public ulong NextUlong(ulong min, ulong max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to max {max}");
            }

            return unchecked((ulong)NextLong(unchecked((long)min), unchecked((long)max)));
        }

        public bool NextBool()
        {
            return NextUint() < HalfwayUint;
        }

        public void NextBytes(byte[] buffer)
        {
            if (ReferenceEquals(buffer, null))
            {
                throw new ArgumentException(nameof(buffer));
            }

            const byte sizeOfInt = 4; // May differ on some platforms

            // See how many ints we can slap into it.
            int chunks = buffer.Length / sizeOfInt;
            byte spare = unchecked((byte)(buffer.Length - (chunks * sizeOfInt)));
            for (int i = 0; i < chunks; ++i)
            {
                int offset = i * chunks;
                int random = Next();
                buffer[offset] = unchecked((byte)(random & 0xFF000000));
                buffer[offset + 1] = unchecked((byte)(random & 0x00FF0000));
                buffer[offset + 2] = unchecked((byte)(random & 0x0000FF00));
                buffer[offset + 3] = unchecked((byte)(random & 0x000000FF));
            }

            {
                /*
                    This could be implemented more optimally by generating a single int and
                    bit shifting along the position, but that is too much for me right now.
                 */
                for (byte i = 0; i < spare; ++i)
                {
                    buffer[^i] = unchecked((byte)Next());
                }
            }
        }

        public double NextDouble()
        {
            return NextUint() * MagicDouble;
        }

        public double NextDouble(double max)
        {
            if (max <= 0)
            {
                throw new ArgumentException($"Max {max} cannot be less-than or equal-to 0");
            }

            return NextDouble() * max;
        }

        public double NextDouble(double min, double max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to max {max}");
            }

            double range = max - min;
            return min + NextDouble(range);
        }

        public float NextFloat()
        {
            uint floatAsInt = NextUint();
            return (floatAsInt >> 8) * MagicFloat;
        }

        public float NextFloat(float max)
        {
            if (max <= 0)
            {
                throw new ArgumentException($"{max} cannot be less-than or equal-to 0");
            }

            return NextFloat() * max;
        }

        public float NextFloat(float min, float max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"Min {min} cannot be larger-than or equal-to max {max}");
            }

            return min + NextFloat(max - min);
        }

        public T Next<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is ICollection<T> collection)
            {
                return Next(collection);
            }

            return Next(enumerable.ToList());
        }

        public T Next<T>(ICollection<T> collection)
        {
            if (collection is IList<T> list)
            {
                return Next(list);
            }

            int count = collection.Count;
            if (count <= 0)
            {
                return default;
            }
            int index = Next(count);
            int i = 0;
            foreach (T element in collection)
            {
                if (i++ == index)
                {
                    return element;
                }
            }

            // Should never happen
            return default;
        }

        public T Next<T>(IList<T> list)
        {
            if (ReferenceEquals(list, null))
            {
                throw new ArgumentNullException(nameof(list));
            }

            int index = Next(list.Count);
            return list[index];
        }

        public T Next<T>() where T : struct, Enum
        {
            Type enumType = typeof(T);
            T[] enumValues;
            if (EnumTypeCache.TryGetValue(enumType, out Array enumArray))
            {
                enumValues = (T[])enumArray;
            }
            else
            {
                enumValues = (T[])Enum.GetValues(typeof(T));
            }

            return enumValues.Length == 0 ? default : enumValues[Next(enumValues.Length)];
        }

        public T NextCachedEnum<T>() where T : struct, Enum
        {
            Type enumType = typeof(T);
            T[] enumValues = (T[])EnumTypeCache.GetOrAdd(enumType, _ => Enum.GetValues(typeof(T)));

            return enumValues.Length == 0 ? default : enumValues[Next(enumValues.Length)];
        }
    }
}
