using System.Xml.Linq;

namespace Leaderboard.API.DataStructures
{
    public class SkipListNode:IComparable<SkipListNode>
    {
        private long _value;

        private decimal _score;

        private SkipListNode _backward;

        private SkipListNode[] _forwards;

        public SkipListNode(long value,decimal score,int level)
        {
            if (level < 0) 
                throw new ArgumentOutOfRangeException("Invalid level.");

            Value = value;
            Score = score;
            Forwards = new SkipListNode[level];
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

        public virtual SkipListNode[] Forwards
        {
            get { return _forwards; }
            private set { _forwards = value; }
        }

        public virtual SkipListNode Backward
        {
            get { return _backward; }
            set { _backward = value; }
        }

        public virtual int Level
        {
            get { return Forwards.Length; }
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
}
