namespace DxRandom
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class PcgRandom : AbstractRandom, IEquatable<PcgRandom>, IComparable, IComparable<PcgRandom>
    {
        [DataMember(Name = "Increment")]
        private ulong _increment;
        [DataMember(Name = "State")]
        private ulong _state;

        public PcgRandom()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            _state = BitConverter.ToUInt64(guidArray, 0);
            _increment = BitConverter.ToUInt64(guidArray, sizeof(ulong));
        }

        public PcgRandom(ulong increment, ulong state)
        {
            _increment = increment;
            _state = state;
        }

        public override uint NextUint()
        {
            ulong oldState = _state;
            _state = oldState * 6364136223846793005UL + _increment;
            uint xorShifted = unchecked((uint)(((oldState >> 18) ^ oldState) >> 27));
            int rot = unchecked((int)(oldState >> 59));
            return (xorShifted >> rot) | (xorShifted << (-rot & 31));
        }

        public bool Equals(PcgRandom other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return _increment == other._increment && _state == other._state;
        }

        public int CompareTo(PcgRandom other)
        {
            if (ReferenceEquals(other, null))
            {
                return -1;
            }

            if (_increment == other._increment)
            {
                if (_state == other._state)
                {
                    return 0;
                }
                if (_state < other._state)
                {
                    return -1;
                }

                return 1;
            }

            if (_increment < other._increment)
            {
                return -1;
            }

            return 1;
        }

        public override bool Equals(object other)
        {
            return Equals(other as PcgRandom);
        }

        public override int GetHashCode()
        {
            return unchecked(-2128831035 * _increment.GetHashCode() + 16777619 * _state.GetHashCode());
        }

        public override string ToString()
        {
            return $"{{{{Increment}}: {_increment}, {{State}}: {_state}}}";
        }

        public int CompareTo(object other)
        {
            return CompareTo(other as PcgRandom);
        }
    }
}
