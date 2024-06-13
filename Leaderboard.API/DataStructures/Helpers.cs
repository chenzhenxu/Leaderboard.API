namespace Leaderboard.API.DataStructures
{
    public static class Helpers
    {
        public static int CompareToExtend(this SkipListNode skipListNode, long value, decimal score) 
        {
            var scoreCompareResult = skipListNode.Score.CompareTo(score);
            if (scoreCompareResult == 0)
            {
                return -(skipListNode.Value.CompareTo(value));
            }
            else
            {
                return scoreCompareResult;
            }

        }
    }
}
