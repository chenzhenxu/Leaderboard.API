using Leaderboard.API.Models;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Text;
using System.Xml.Linq;

namespace Leaderboard.API.DataStructures
{
    public class SkipList : ICollection<Customer>, IEnumerable<Customer>
    {
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
                _firstNode.Forwards[i] = _firstNode;
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

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].CompareToExtend(item.CustomerId, item.Score) > 0)
                {
                    current = current.Forwards[i];
                }
                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];


            int level = _getNextLevel();
            if (level > _currentMaxLevel)
            {
                for (int i = _currentMaxLevel; i < level; ++i)
                {
                    toBeUpdated[i] = _firstNode;
                }
                _currentMaxLevel = level;
            }

            var newNode = new SkipListNode(item.CustomerId, item.Score, level);

            Console.WriteLine($"new node level:{level}");
            for (int i = 0; i < level; ++i)
            {
                newNode.Forwards[i] = toBeUpdated[i].Forwards[i];
                toBeUpdated[i].Forwards[i] = newNode;

            }

            newNode.Backward = toBeUpdated[0];
            current.Backward = newNode;

            _count++;
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

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].CompareToExtend(item.CustomerId, item.Score) > 0)
                {
                    current = current.Forwards[i];
                }
                toBeUpdated[i] = current;
            }

            current = current.Forwards[0];

            if (current.CompareToExtend(item.CustomerId, item.Score) != 0)
            {
                deleteditem = default(long);
                deletedscore = default;
                return false;
            }

            for (int i = 0; i < _currentMaxLevel; ++i)
            {
                if (toBeUpdated[i].Forwards[i] == current)
                {
                    toBeUpdated[i].Forwards[i] = current.Forwards[i];
                }
            }
            current.Backward = toBeUpdated[0];

            --_count;

            while (_currentMaxLevel > 1 && _firstNode.Forwards[_currentMaxLevel - 1] == _firstNode)
                _currentMaxLevel--;

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

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
                while (current.Forwards[i] != _firstNode && current.Forwards[i].CompareToExtend(item.CustomerId, item.Score) > 0)
                    current = current.Forwards[i];

            current = current.Forwards[0];

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
                CustomerId = _firstNode.Forwards[0].Value,
                Score = _firstNode.Forwards[0].Score
            };
            return true;
        }

        public IEnumerator<Customer> GetEnumerator()
        {
            var node = _firstNode;
            while (node.Forwards[0] != null && node.Forwards[0] != _firstNode)
            {
                node = node.Forwards[0];
                yield return new Customer
                {
                    CustomerId = node.Forwards[0].Value,
                    Score = node.Forwards[0].Score
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
            var node = _firstNode;

            for (int i = 0; i < _count; i++)
            {

                if (node.Forwards[0] != null && node.Forwards[0] != _firstNode)
                {
                    node = node.Forwards[0];
                    if (i + 1 >= start && (i < (start + length)))
                        rangeCustomers.Add(new Customer { CustomerId = node.Value, Score = node.Score });
                }
                else
                    break;

            }
            return rangeCustomers;
        }

        public List<Customer> GetRange(Customer customer, int high, int low, out int firstRank)
        {
            var rangeCustomers = new List<Customer>();
            firstRank = 0;

            var current = _firstNode;

            for (int i = _currentMaxLevel - 1; i >= 0; --i)
            {
                while (current.Forwards[i] != _firstNode && current.Forwards[i].CompareToExtend(customer.CustomerId, customer.Score) > 0)
                {
                    current = current.Forwards[i];
                    firstRank++;
                }
            }

            current = current.Forwards[0];
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

                //add current customer
                rangeCustomers.Add(customer);

                // add low nodes
                var lowNode = current;
                while (low > 0 && lowNode.Forwards[0] != _firstNode)
                {
                    lowNode = lowNode.Forwards[0];
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

            for (int i = 0; i < MaxLevel; ++i)
            {
                _firstNode.Forwards[i] = _firstNode;
            }

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var current = _firstNode;

            sb.Append($"Skip List (Level {_currentMaxLevel}) ,count {_count}\n");
            
            while (current.Forwards[0] != _firstNode)
            {
                current = current.Forwards[0];

                sb.Append($"c:{current.Value} s:{current.Score} l:{current.Level}");
                int level = 0;
                while (level < current.Level)
                {
                    sb.Append("\t");
                    level++;
                }
                sb.Append($"L{current.Level}");
                sb.Append("\n");
                
            }           

            sb.Append("\n\n");

            return sb.ToString();

        }
    }
}

