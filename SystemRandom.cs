namespace DxRandom
{
    using System;

    public sealed class SystemRandom : AbstractRandom
    {
        private readonly Random _random;

        public override uint NextUint()
        {
            return unchecked((uint)_random.Next(int.MinValue, int.MaxValue));
        }
    }
}
