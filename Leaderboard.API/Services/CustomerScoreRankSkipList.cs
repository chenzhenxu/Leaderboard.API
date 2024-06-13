using Leaderboard.API.DataStructures;
using Leaderboard.API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Leaderboard.API.Services
{
    public class CustomerScoreRankSkipList
    {
        //all the customers and their scores
        public Dictionary<long, decimal> CustomerScores = new Dictionary<long, decimal>();
        // Customers participating in the leaderboard
        public SkipList LeaderboardCustomers { get; set; } = new SkipList();

        public decimal UpdateCustomerScore(long customerId, decimal score)
        {

            decimal newScore;
            decimal oldScore;
            Customer oldCustomer = null;

            //update all customer dic with new score
            if (!CustomerScores.ContainsKey(customerId))
            {
                CustomerScores.Add(customerId, score);
                oldScore = 0;
                newScore = score;
            }
            else
            {
                oldScore = CustomerScores[customerId];
                oldCustomer = new Customer { CustomerId = customerId, Score = oldScore };

                newScore = CustomerScores[customerId] + score;
                CustomerScores[customerId] = newScore;
            }

            //if new score >0 then update the leaderboard Customers
            if (newScore > 0)
            {
                var newCustomer = new Customer { CustomerId = customerId, Score = newScore };
                //Check if LeaderboardCustomer exists
                if (oldCustomer == null)
                {
                    LeaderboardCustomers.Add(newCustomer);
                }
                else
                {
                    LeaderboardCustomers.Remove(oldCustomer);

                    LeaderboardCustomers.Add(newCustomer);
                }
            }
            else
            {
                //if new newScore <= 0 and oldScore>0 then remove the customer from the leaderboard
                if (oldScore > 0 && oldCustomer != null)
                {
                    LeaderboardCustomers.Remove(oldCustomer);
                }
            }

            Console.WriteLine(JsonSerializer.Serialize(CustomerScores));
            Console.WriteLine(LeaderboardCustomers.ToString());
            return newScore;
        }


        public List<LeaderBoardItem> GetRankResults(int start, int end)
        {
            var leaderBoardItems = new List<LeaderBoardItem>();

            if (start < 1)
            {
                start = 1;
            }
            if (start > LeaderboardCustomers.Count)
            {
                return leaderBoardItems;
            }

            int length;
            if (end >= LeaderboardCustomers.Count)
            {
                length = LeaderboardCustomers.Count - start + 1;
            }
            else
            {
                length = end - start;
            }

            var customersResults = LeaderboardCustomers.GetRange(start, length);

            var rank = start;
            foreach (var customer in customersResults)
            {
                leaderBoardItems.Add(new LeaderBoardItem { CustomerId = customer.CustomerId, Score = customer.Score, Rank = rank });
                rank++;
            }

            Console.WriteLine(LeaderboardCustomers.ToString());
            return leaderBoardItems;
        }

        public List<LeaderBoardItem> GetRankResults(long customerId, int high, int low)
        {
            var leaderBoardItems = new List<LeaderBoardItem>();
            if (CustomerScores.ContainsKey(customerId))
            {
                var score = CustomerScores[customerId];
                var currentCustomer = new Customer { CustomerId = customerId, Score = score };
                if (score > 0)
                {
                    var customersResults = LeaderboardCustomers.GetRange(currentCustomer, high, low, out int highestCustomerRank);
                    foreach (var customer in customersResults)
                    {
                        leaderBoardItems.Add(new LeaderBoardItem { CustomerId = customer.CustomerId, Score = customer.Score, Rank = highestCustomerRank });
                        highestCustomerRank++;
                    }
                }

            }

            Console.WriteLine(LeaderboardCustomers.ToString());
            return leaderBoardItems;
        }
    }
}

