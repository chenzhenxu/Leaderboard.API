using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Leaderboard.API.Models
{
    public class Customer : IEquatable<Customer>, IComparable<Customer>
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CustomerId { get; set; }

        public decimal Score { get; set; }

        public int CompareTo(Customer? other)
        {
            if (other == null) return 1;
            var scoreCompareResult = this.Score.CompareTo(other.Score);
            if (scoreCompareResult == 0)
            {
                return -(this.CustomerId.CompareTo(other.CustomerId));
            }
            else
            {
                return -scoreCompareResult;
            }
        }

        public bool Equals(Customer? other)
        {
            if (other == null) return false;
            return (this.CustomerId.Equals(other.CustomerId));
        }
    }
}
