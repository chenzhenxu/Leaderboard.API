using Leaderboard.API.Models;

namespace Leaderboard.API.Services
{
    public class CustomerScoreRank
    {
        //all the customers and their scores
        public Dictionary<long, decimal> CustomerScores = new Dictionary<long, decimal>();
        // Customers participating in the leaderboard
        public List<Customer> LeaderboardCustomers { get; set; } = new List<Customer>();        

        public Dictionary<long, int> LeaderboardCustomerIndexs = new Dictionary<long, int>();


        public decimal UpdateCustomerScore(long customerId, decimal score)
        {

            decimal newScore;
            decimal oldScore; 

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
                newScore = CustomerScores[customerId] + score;
                CustomerScores[customerId] = newScore;
            }

            //if new score >0 then update the leaderboard Customers
            if (newScore > 0)
            {
                //Check if LeaderboardCustomer Indexs exists
                if (!LeaderboardCustomerIndexs.ContainsKey(customerId))
                {      

                    var newCustomer = new Customer { CustomerId = customerId, Score = newScore };
                    var customerIndex = LeaderboardCustomers.BinarySearch(newCustomer);
                    customerIndex = ~customerIndex;

                    LeaderboardCustomers.Insert(customerIndex, newCustomer);
                    LeaderboardCustomerIndexs.Add(customerId, customerIndex);                    
                }
                else
                {
                    var currentCustomerIndex = LeaderboardCustomerIndexs[customerId];
                    var oldCustomer = LeaderboardCustomers[currentCustomerIndex];
                    var newCustomer = new Customer { CustomerId = oldCustomer.CustomerId, Score = oldCustomer.Score + score };
                    LeaderboardCustomers.RemoveAt(currentCustomerIndex);

                    var newCustomerIndex = LeaderboardCustomers.BinarySearch(newCustomer);
                    newCustomerIndex = ~newCustomerIndex;

                    LeaderboardCustomers.Insert(newCustomerIndex, newCustomer);
                    LeaderboardCustomerIndexs[customerId] = newCustomerIndex;
                    
                }
            }
            else
            {
                //if new newScore <= 0 and oldScore>0 then remove the customer from the leaderboard
                if (oldScore > 0)
                {
                    var currentCustomerIndex = LeaderboardCustomerIndexs[customerId];
                    var oldCustomer = LeaderboardCustomers[currentCustomerIndex];                    
                    LeaderboardCustomers.RemoveAt(currentCustomerIndex);
                }    
            }
            return newScore;
        }


        public List<LeaderBoardItem> GetRankResults(int start, int end)
        {
            var leaderBoardItems = new List<LeaderBoardItem>();

            if (start < 0)
            {
                start = 0;
            }
            int length;
            if (end >= LeaderboardCustomers.Count)
            {
                length = LeaderboardCustomers.Count - start + 1;
            }
            else
            {
                length = end - start + 1;
            }
            int index = start - 1;
            var customersResults = LeaderboardCustomers.GetRange(start - 1, length);
            foreach (var customer in customersResults)
            {
                leaderBoardItems.Add(new LeaderBoardItem { CustomerId = customer.CustomerId, Score = customer.Score, Rank = index + 1 });
                index++;
            }

            return leaderBoardItems;
        }

        public List<LeaderBoardItem> GetRankResults(long customerId, int high, int low)
        {
            var leaderBoardItems = new List<LeaderBoardItem>();
            if (!LeaderboardCustomerIndexs.ContainsKey(customerId))
            {
                return leaderBoardItems;
            }

            var currentCustomerIndex = LeaderboardCustomerIndexs[customerId];
            int lowCustomerIndex;
            int highCustomerIndex;

            if (currentCustomerIndex - high < 0)
            {
                highCustomerIndex = 0;
            }
            else
            {
                highCustomerIndex = currentCustomerIndex - high;
            }
            if (currentCustomerIndex + 1 + low >= LeaderboardCustomers.Count)
            {
                lowCustomerIndex = LeaderboardCustomers.Count - 1;

            }
            else
            {
                lowCustomerIndex = currentCustomerIndex + 1 + low;
            }

            var customersResults = LeaderboardCustomers.GetRange(highCustomerIndex, lowCustomerIndex - highCustomerIndex + 1);
            foreach (var customer in customersResults)
            {
                highCustomerIndex++;
                leaderBoardItems.Add(new LeaderBoardItem { CustomerId = customer.CustomerId, Score = customer.Score, Rank = highCustomerIndex });
            }

            return leaderBoardItems;
        }
    }
}

