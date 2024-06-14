using Leaderboard.API.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Text;

namespace Leaderboard.API.DataStructures
{
    public class SkipList : ICollection<Customer>, IEnumerable<Customer>
    {
        private static readonly object AddAndRemoveLock = new object();
        private int _count { get; set; }
        private int _currentMaxLevel { get; set; }
        private Random _randomizer { get; set; }
        private SkipListNode _firstNode { get; set; }


        private const int MaxLevel = 32;
        private const double Probability = 0.5;

        private int _getNextLevel()
        {
            int lvl = 1;
            while (_randomizer.NextDouble() < Probability && lvl <= _currentMaxLevel && lvl < MaxLevel)
            {
                ++lvl;
            }
            return lvl;
        }

        public SkipList()
        {
            _count = 0;
            _currentMaxLevel = 1;
            _randomizer = new Random();
            _firstNode = new SkipListNode(default, default, MaxLevel);

            for (int i = 0; i < MaxLevel; i++)
            {
                _firstNode.SkipListLevels[i].Forward = _firstNode;
            }
        }

        public SkipListNode Root
        {
            get { return _firstNode; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public int Count
        {
            get { return _count; }
        }

        public int Level
        {
            get { return _currentMaxLevel; }
        }

        public Customer this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(Customer item)
        {
            var current = _firstNode;
            var toBeUpdated = new SkipListNode[MaxLevel];
            int[] rank = new int[MaxLevel];

            lock (AddAndRemoveLock)
            {

                for (int i = _currentMaxLevel - 1; i >= 0; i--)
                {
                    rank[i] = i == _currentMaxLevel - 1 ? 0 : rank[i + 1];
                    while (current.SkipListLevels[i].Forward != _firstNode && current.SkipListLevels[i].Forward.CompareToExtend(item.CustomerId, item.Score) > 0)
                    {
                        rank[i] = rank[i] + current.SkipListLevels[i].Span;
                        current = current.SkipListLevels[i].Forward;

                    }
                    toBeUpdated[i] = current;
                }

                current = current.SkipListLevels[0].Forward;


                int level = _getNextLevel();
                if (level > _currentMaxLevel)
                {
                    for (int i = _currentMaxLevel; i < level; i++)
                    {
                        rank[i] = 0;
                        toBeUpdated[i] = _firstNode;
                        toBeUpdated[i].SkipListLevels[i].Span = _count;
                    }
                    _currentMaxLevel = level;
                }

                var newNode = new SkipListNode(item.CustomerId, item.Score, level);

                Console.WriteLine($"new node level:{level}");
                for (int i = 0; i < level; i++)
                {
                    newNode.SkipListLevels[i].Forward = toBeUpdated[i].SkipListLevels[i].Forward;
                    toBeUpdated[i].SkipListLevels[i].Forward = newNode;

                    newNode.SkipListLevels[i].Span = toBeUpdated[i].SkipListLevels[i].Span - (rank[0] - rank[i]);
                    toBeUpdated[i].SkipListLevels[i].Span = (rank[0] - rank[i]) + 1;
                }

                for (int i = level; i < _currentMaxLevel; i++)
                {
                    toBeUpdated[i].SkipListLevels[i].Span++;
                }

                newNode.Backward = toBeUpdated[0];
                current.Backward = newNode;

                _count++;
            }
        }

        public bool Remove(Customer item)
        {
            long deleted;
            decimal deletedscore;
            return Remove(item, out deleted, out deletedscore);
        }

        public bool Remove(Customer item, out long deleteditem, out decimal deletedscore)
        {
            var current = _firstNode;
            var toBeUpdated = new SkipListNode[MaxLevel];

            lock (AddAndRemoveLock)
            {
                for (int i = _currentMaxLevel - 1; i >= 0; i--)
                {
                    while (current.SkipListLevels[i].Forward != _firstNode && current.SkipListLevels[i].Forward.CompareToExtend(item.CustomerId, item.Score) > 0)
                    {
                        current = current.SkipListLevels[i].Forward;
                    }
                    toBeUpdated[i] = current;
                }

                current = current.SkipListLevels[0].Forward;

                if (current.CompareToExtend(item.CustomerId, item.Score) != 0)
                {
                    deleteditem = default(long);
                    deletedscore = default;
                    return false;
                }

                for (int i = 0; i < _currentMaxLevel; i++)
                {
                    if (toBeUpdated[i].SkipListLevels[i].Forward == current)
                    {
                        toBeUpdated[i].SkipListLevels[i].Span += current.SkipListLevels[i].Span - 1;
                        toBeUpdated[i].SkipListLevels[i].Forward = current.SkipListLevels[i].Forward;

                    }
                    else
                    {
                        toBeUpdated[i].SkipListLevels[i].Span -= 1;
                    }
                }
                current.Backward = toBeUpdated[0];

                --_count;

                while (_currentMaxLevel > 1 && _firstNode.SkipListLevels[_currentMaxLevel - 1].Forward == _firstNode)
                    _currentMaxLevel--;
            }

            deleteditem = current.Value;
            deletedscore = current.Score;
            return true;

        }

        public bool Contains(Customer item)
        {
            long itemOut;
            decimal scoreOut;
            return Find(item, out itemOut, out scoreOut);

        }

        public bool Find(Customer item, out long result, out decimal deletedscore)
        {
            var current = _firstNode;

            for (int i = _currentMaxLevel - 1; i >= 0; i--)
                while (current.SkipListLevels[i].Forward != _firstNode && current.SkipListLevels[i].Forward.CompareToExtend(item.CustomerId, item.Score) > 0)
                    current = current.SkipListLevels[i].Forward;

            current = current.SkipListLevels[0].Forward;

            if (current.CompareToExtend(item.CustomerId, item.Score) == 0)
            {
                result = current.Value;
                deletedscore = current.Score;
                return true;
            }

            result = default(long);
            deletedscore = default;
            return false;

        }

        public Customer Peek()
        {
            Customer peek;
            if (!TryPeek(out peek))
            {
                throw new InvalidOperationException("SkipList is empty.");
            }

            return peek;
        }

        public bool TryPeek(out Customer result)
        {
            if (IsEmpty)
            {
                result = default(Customer);
                return false;
            }
            result = new Customer
            {
                CustomerId = _firstNode.SkipListLevels[0].Forward.Value,
                Score = _firstNode.SkipListLevels[0].Forward.Score
            };
            return true;
        }

        public IEnumerator<Customer> GetEnumerator()
        {
            var node = _firstNode;
            while (node.SkipListLevels[0].Forward != null && node.SkipListLevels[0].Forward != _firstNode)
            {
                node = node.SkipListLevels[0].Forward;
                yield return new Customer
                {
                    CustomerId = node.SkipListLevels[0].Forward.Value,
                    Score = node.SkipListLevels[0].Forward.Score
                }; ;
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public void CopyTo(Customer[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (array.Length == 0 || arrayIndex >= array.Length || arrayIndex < 0)
                throw new IndexOutOfRangeException();

            var enumarator = this.GetEnumerator();

            for (int i = arrayIndex; i < array.Length; i++)
            {
                if (enumarator.MoveNext())
                    array[i] = enumarator.Current;
                else
                    break;

            }


        }
        public List<Customer> GetRange(int start, int length)
        {
            var rangeCustomers = new List<Customer>();
            var current = _firstNode;
            int tranversed = 0;

            for (int i = _currentMaxLevel - 1; i >= 0; i--)
            {
                while (current.SkipListLevels[i].Forward != _firstNode && (tranversed + (current.SkipListLevels[i].Span) <= start))
                {
                    tranversed += current.SkipListLevels[i].Span;
                    current = current.SkipListLevels[i].Forward;
                }
                if (tranversed == start)
                {


                    while (current != _firstNode && length > 0)
                    {
                        rangeCustomers.Add(new Customer { CustomerId = current.Value, Score = current.Score });
                        current = current.SkipListLevels[0].Forward;
                        length--;
                    }
                    return rangeCustomers;
                }
            }

            return rangeCustomers;
        }

        public List<Customer> GetRange(Customer customer, int high, int low, out int firstRank)
        {
            var rangeCustomers = new List<Customer>();
            firstRank = 0;

            var current = _firstNode;

            for (int i = _currentMaxLevel - 1; i >= 0; i--)
            {
                while (current.SkipListLevels[i].Forward != _firstNode && current.SkipListLevels[i].Forward.CompareToExtend(customer.CustomerId, customer.Score) > 0)
                {
                    firstRank += current.SkipListLevels[i].Span;
                    current = current.SkipListLevels[i].Forward;
                }
            }

            current = current.SkipListLevels[0].Forward;
            firstRank++;

            if (current.CompareToExtend(customer.CustomerId, customer.Score) == 0)
            {
                //add high customer
                var highNode = current;
                while (high > 0 && highNode.Backward != _firstNode)
                {
                    highNode = highNode.Backward;
                    high--;
                    firstRank--;

                    rangeCustomers.Add(new Customer
                    {
                        CustomerId = highNode.Value,
                        Score = highNode.Score
                    });

                }
                rangeCustomers.Reverse();

                //add current customer
                rangeCustomers.Add(customer);

                // add low nodes
                var lowNode = current;
                while (low > 0 && lowNode != _firstNode)
                {
                    lowNode = lowNode.SkipListLevels[0].Forward;
                    low--;

                    rangeCustomers.Add(new Customer
                    {
                        CustomerId = lowNode.Value,
                        Score = lowNode.Score
                    });

                }
            }


            return rangeCustomers;
        }

        public void Clear()
        {
            _count = 0;
            _currentMaxLevel = 1;
            _randomizer = new Random();
            _firstNode = new SkipListNode(default(long), default(decimal), MaxLevel);

            for (int i = 0; i < MaxLevel; i++)
            {
                _firstNode.SkipListLevels[i].Forward = _firstNode;
            }

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var current = _firstNode;

            sb.Append($"Skip List (Level {_currentMaxLevel}) ,count {_count}\n");

            //print header node
            sb.Append($"c:{current.Value} s:{current.Score} l:{_currentMaxLevel}");

            int firstNodeLevel = 0;
            while (firstNodeLevel < _currentMaxLevel)
            {
                sb.Append("\t");
                firstNodeLevel++;
            }
            sb.Append($"L{_currentMaxLevel} Span{current.SkipListLevels[_currentMaxLevel - 1].Span}");
            sb.Append("\n");

            //print other nodes

            while (current.SkipListLevels[0].Forward != _firstNode)
            {
                current = current.SkipListLevels[0].Forward;

                sb.Append($"c:{current.Value} s:{current.Score} l:{current.Level}");
                int level = 0;
                while (level < current.Level)
                {
                    sb.Append("\t");
                    level++;
                }
                sb.Append($"L{current.Level} Span{current.SkipListLevels[current.Level - 1].Span}");
                sb.Append("\n");

            }

            sb.Append("\n\n");

            return sb.ToString();

        }
    }
}

