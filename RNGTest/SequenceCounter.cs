namespace RNGTest
{
    public enum SequenceType
    {
        GreaterThen,
        LessThen
    }
    class SequenceCounter
    {
        public uint MaxLength = 0;
        public uint CurrentCount = 0;
        public uint Threshold;
        public SequenceType Type;
        public SequenceCounter(SequenceType type, uint threshold)
        {
            Threshold = threshold;
            Type = type;
        }
        public void Add(uint value)
        {
            if (Compare(value))
            {
                CurrentCount += 1;
                if(CurrentCount > MaxLength)
                {
                    MaxLength = CurrentCount;
                }
            } else
            {
                CurrentCount = 0;
            }
        }
        bool Compare(uint value)
        {
            if (Type == SequenceType.GreaterThen)
            {
                return value > Threshold;
            } else
            {
                return value < Threshold;
            }
        }
    }
}
