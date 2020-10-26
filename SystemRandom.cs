namespace DxRandom
{
    using System;

    /// <summary>
    ///     Implementation dependent upon .Net's Random class.
    /// </summary>
    public sealed class SystemRandom : AbstractRandom
    {
        private readonly Random _random;

        public SystemRandom()
        {
            _random = new Random();
        }

        public SystemRandom(int seed)
        {
            _random = new Random(seed);
        }

        public override uint NextUint()
        {
            return unchecked((uint)_random.Next(int.MinValue, int.MaxValue));
        }
    }
}
