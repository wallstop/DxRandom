﻿namespace DxRandom
{
    using System;
    using System.Collections.Generic;

    public interface IRandom
    {
        int Next();
        int Next(int max);
        int Next(int min, int max);

        uint NextUint();
        uint NextUint(uint max);
        uint NextUint(uint min, uint max);

        short NextShort();
        short NextShort(short max);
        short NextShort(short min, short max);

        byte NextByte();
        byte NextByte(byte max);
        byte NextByte(byte min, byte max);

        long NextLong();
        long NextLong(long max);
        long NextLong(long min, long max);

        ulong NextUlong();
        ulong NextUlong(ulong max);
        ulong NextUlong(ulong min, ulong max);

        bool NextBool();

        void NextBytes(byte[] buffer);

        float NextFloat();
        float NextFloat(float max);
        float NextFloat(float min, float max);

        double NextDouble();
        double NextDouble(double max);
        double NextDouble(double min, double max);

        T Next<T>(IEnumerable<T> enumerable);
        T Next<T>(ICollection<T> collection);
        T Next<T>(IList<T> list);

        T Next<T>() where T : struct, Enum;
        T NextCachedEnum<T>() where T : struct, Enum;
    }
}
