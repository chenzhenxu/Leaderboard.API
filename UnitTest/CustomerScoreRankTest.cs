using Leaderboard.API.Services;

namespace UnitTest
{
    public class CustomerScoreRankTest
    {
        [Fact]
        public void UpdateCustomerScoreSimple()
        {
            var customerScoreRank = new CustomerScoreRank();
            long firstCustomerId = 1;
            long secondCustomerId = 2;

            customerScoreRank.UpdateCustomerScore(firstCustomerId, 100);
            customerScoreRank.UpdateCustomerScore(firstCustomerId, -10);
            customerScoreRank.UpdateCustomerScore(firstCustomerId, 20);
            customerScoreRank.UpdateCustomerScore(secondCustomerId, 9);

            Assert.Equal(customerScoreRank.LeaderboardCustomers.FirstOrDefault(x => x.CustomerId == 1)?.Score, 100 - 10 + 20);
            Assert.Equal(customerScoreRank.LeaderboardCustomers.FirstOrDefault(x => x.CustomerId == 2)?.Score, 9);

        }


        [Fact]
        public void GetRankResultsByStartAndEnd()
        {
            var customerScoreRank = new CustomerScoreRank();
            long firstCustomerId = 1;
            long secondCustomerId = 2;
            int start = 1;
            int end = 100;


            customerScoreRank.UpdateCustomerScore(firstCustomerId, 100);
            customerScoreRank.UpdateCustomerScore(firstCustomerId, -10);
            customerScoreRank.UpdateCustomerScore(secondCustomerId, 9);

            var rankResults = customerScoreRank.GetRankResults(start, end);

            Assert.Equal(rankResults.FirstOrDefault(x => x.CustomerId == 1)?.Rank, 1);
            Assert.Equal(rankResults?.Count, 2);

        }

        [Fact]
        public void GetRankResultsByCustomerId()
        {
            var customerScoreRank = new CustomerScoreRank();
            long firstCustomerId = 1;
            long secondCustomerId = 2;
            int high = 1;
            int low = 3;

            customerScoreRank.UpdateCustomerScore(firstCustomerId, 100);
            customerScoreRank.UpdateCustomerScore(firstCustomerId, 20);
            customerScoreRank.UpdateCustomerScore(secondCustomerId, 9);

            var rankResults = customerScoreRank.GetRankResults(firstCustomerId, high, low);

            Assert.Equal(rankResults.FirstOrDefault(x => x.CustomerId == 1)?.Rank, 1);
            Assert.Equal(rankResults?.Count, 2);

        }

        [Fact]
        public void ComplexTest ()
        {
            var customerScoreRank = new CustomerScoreRank();
            long firstCustomerId = 1;
            long secondCustomerId = 2;
            int high = 1;
            int low = 3;

            customerScoreRank.UpdateCustomerScore(firstCustomerId, 100);
            customerScoreRank.UpdateCustomerScore(firstCustomerId, 20);
            customerScoreRank.UpdateCustomerScore(secondCustomerId, 9);

            var rankResults = customerScoreRank.GetRankResults(firstCustomerId, high, low);

            Assert.Equal(rankResults.FirstOrDefault(x => x.CustomerId == 1)?.Rank, 1);
            Assert.Equal(rankResults?.Count, 2);

        }
    }
}