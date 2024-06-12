namespace Leaderboard.API.Models
{
    public class LeaderBoardItem
    {
        public long CustomerId { get; set; }

        public decimal Score { get; set; }

        public int Rank { get; set; }
    }
}
