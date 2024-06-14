using Leaderboard.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class  CustomerScoreRankSkipListTest
    {
        [Fact]
        public void UpdateCustomerScoreSimple()
        {
            var customerScoreRankSkipList = new CustomerScoreRankSkipList();            

            customerScoreRankSkipList.UpdateCustomerScore(10000, 100);
            customerScoreRankSkipList.UpdateCustomerScore(10001, -10);
            customerScoreRankSkipList.UpdateCustomerScore(2, 20);
            customerScoreRankSkipList.UpdateCustomerScore(33, 9);
            customerScoreRankSkipList.UpdateCustomerScore(100, 8);
            customerScoreRankSkipList.UpdateCustomerScore(1234, 56);
            customerScoreRankSkipList.UpdateCustomerScore(123, 75);
            customerScoreRankSkipList.UpdateCustomerScore(123400, 89);
            customerScoreRankSkipList.UpdateCustomerScore(20430, 200);
            customerScoreRankSkipList.UpdateCustomerScore(994, 2);


            Assert.Equal(customerScoreRankSkipList.LeaderboardCustomers?.Count, 9);
            Assert.Equal(customerScoreRankSkipList.CustomerScores?.Count, 10);

        }

        [Fact]
        public void GetRankResultsByStartAndEnd()
        {
            var customerScoreRankSkipList = new CustomerScoreRankSkipList();

            customerScoreRankSkipList.UpdateCustomerScore(10000, 100);
            customerScoreRankSkipList.UpdateCustomerScore(10001, -10);
            customerScoreRankSkipList.UpdateCustomerScore(2, 20);
            customerScoreRankSkipList.UpdateCustomerScore(33, 9);
            customerScoreRankSkipList.UpdateCustomerScore(100, 8);
            customerScoreRankSkipList.UpdateCustomerScore(1234, 56);
            customerScoreRankSkipList.UpdateCustomerScore(123, 75);
            customerScoreRankSkipList.UpdateCustomerScore(123400, 89);
            customerScoreRankSkipList.UpdateCustomerScore(20430, 200);
            customerScoreRankSkipList.UpdateCustomerScore(994, 2);

            var rankResults = customerScoreRankSkipList.GetRankResults(1, 10);

            Assert.Equal(rankResults.FirstOrDefault(x => x.CustomerId == 20430)?.Rank, 1);
            Assert.Equal(rankResults?.Count, 9);

        }

        [Fact]
        public void GetRankResultsByCustomerId()
        {
            var customerScoreRankSkipList = new CustomerScoreRankSkipList();

            customerScoreRankSkipList.UpdateCustomerScore(10000, 100);
            customerScoreRankSkipList.UpdateCustomerScore(10001, -10);
            customerScoreRankSkipList.UpdateCustomerScore(2, 20);
            customerScoreRankSkipList.UpdateCustomerScore(33, 9);
            customerScoreRankSkipList.UpdateCustomerScore(100, 8);
            customerScoreRankSkipList.UpdateCustomerScore(1234, 56);
            customerScoreRankSkipList.UpdateCustomerScore(123, 75);
            customerScoreRankSkipList.UpdateCustomerScore(123400, 89);
            customerScoreRankSkipList.UpdateCustomerScore(20430, 200);
            customerScoreRankSkipList.UpdateCustomerScore(994, 2);

            var rankResults = customerScoreRankSkipList.GetRankResults(10000, 2, 7);

            Assert.Equal(rankResults.FirstOrDefault(x => x.CustomerId == 10000)?.Rank, 2);
            Assert.Equal(rankResults?.Count, 9);

        }


    }
}
