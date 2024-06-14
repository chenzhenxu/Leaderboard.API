using System.Xml.Linq;

namespace Leaderboard.API.DataStructures
{
    public class SkipListNode:IComparable<SkipListNode>
    {
        private long _value;

        private decimal _score;

        private SkipListNode _backward;

        private SkipListLevel[] _skipListLevels;

        public SkipListNode(long value,decimal score,int level)
        {
            if (level < 0) 
                throw new ArgumentOutOfRangeException("Invalid level.");

            Value = value;
            Score = score;
            SkipListLevels = Enumerable
                                 .Range(1, level)
                                 .Select(i => new SkipListLevel())
                                 .ToArray(); ;
        }

        public virtual long Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        public virtual decimal Score
        {
            get { return _score; }
            private set { _score = value; }
        }

        public virtual SkipListLevel[] SkipListLevels
        {
            get { return _skipListLevels; }
            private set { _skipListLevels = value; }
        }

        public virtual SkipListNode Backward
        {
            get { return _backward; }
            set { _backward = value; }
        }

        public virtual int Level
        {
            get { return SkipListLevels.Length; }
        }

        public virtual int CompareTo(SkipListNode other)
        {
            var scoreCompareResult = this.Score.CompareTo(other.Score);
            if (scoreCompareResult == 0)
            {
                return -(this.Value.CompareTo(other.Value));
            }
            else
            {
                return scoreCompareResult;
            }
        }

        
    }

    public class SkipListLevel
    {
        private SkipListNode _forward = default;
        public SkipListNode Forward { get => _forward; set => _forward = value; }
        //The distance  to the next node in this level
        public int Span { get; set; }

    }
}
