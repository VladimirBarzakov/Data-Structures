using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

public class Chainblock : IChainblock
{

    public class myAmountCompararer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            return y.CompareTo(x);
        }
    }


    private Dictionary<int, Transaction> byId;
    private Dictionary<TransactionStatus, Dictionary<int, Transaction>> byStatusAndId;

    private int count;

    public Chainblock()
    {
        this.byId = new Dictionary<int, Transaction>();
        this.byStatusAndId = new Dictionary<TransactionStatus, Dictionary<int, Transaction>>();
    }

    public int Count => count;

    public void Add(Transaction tx)
    {
        this.byId.Add(tx.Id, tx);
        if (!this.byStatusAndId.ContainsKey(tx.Status))
        {
            this.byStatusAndId[tx.Status] = new Dictionary<int, Transaction>();
        }
        this.byStatusAndId[tx.Status].Add(tx.Id, tx);


        this.count++;
    }

    public void ChangeTransactionStatus(int id, TransactionStatus newStatus)
    {
        if (!this.Contains(id))
        {
            throw new ArgumentException();
        }
        if (!this.byStatusAndId.ContainsKey(newStatus))
        {
            this.byStatusAndId[newStatus] = new Dictionary<int, Transaction>();
        }
        this.byStatusAndId[this.byId[id].Status].Remove(id);
        this.byId[id].Status = newStatus;
        this.byStatusAndId[newStatus].Add(id, this.byId[id]);
    }

    public bool Contains(Transaction tx)
    {
        return this.byId.ContainsKey(tx.Id);
    }

    public bool Contains(int id)
    {
        return this.byId.ContainsKey(id);
    }

    public IEnumerable<Transaction> GetAllInAmountRange(double lo, double hi)
    {
        //List<int> range =  this.byAmount.Range(hi, true, lo, true).SelectMany(x=>x.Value).ToList();
        //List<Transaction> res = new List<Transaction>();
        //foreach (int key in range)
        //{
        //    res.Add( this.byId[key]);
        //}
        //return res;
        return this.byId.Values.Where(x => x.Amount >= lo && x.Amount <= hi);
    }

    public IEnumerable<Transaction> GetAllOrderedByAmountDescendingThenById()
    {
        
        return this.byId.Values.OrderByDescending(x => x.Amount).ThenBy(x => x.Id);
        
    }

    public IEnumerable<string> GetAllReceiversWithTransactionStatus(TransactionStatus status)
    {
        if (!this.byStatusAndId.ContainsKey(status) || this.byStatusAndId[status].Count == 0)
        {
            throw new InvalidOperationException();
        }
        return this.byStatusAndId[status].Values.OrderByDescending(x => x.Amount).Select(x => x.To);
    }

    public IEnumerable<string> GetAllSendersWithTransactionStatus(TransactionStatus status)
    {
        if (!this.byStatusAndId.ContainsKey(status) || this.byStatusAndId[status].Count==0)
        {
            throw new InvalidOperationException();
        }
        return this.byStatusAndId[status].Values.OrderByDescending(x => x.Amount).Select(x => x.From);
    }

    public Transaction GetById(int id)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new InvalidOperationException();
        }
        return this.byId[id];
    }

    public IEnumerable<Transaction> GetByReceiverAndAmountRange(string receiver, double lo, double hi)
    {
        List<Transaction> res = this.byId.Values.Where(x => (x.Amount >= lo && x.Amount < hi) && x.To == receiver)
            .OrderByDescending(x => x.Amount).ThenBy(x=>x.Id).ToList();
        if (res.Count==0)
        {
            throw new InvalidOperationException();
        }
        return res;
    }

    public IEnumerable<Transaction> GetByReceiverOrderedByAmountThenById(string receiver)
    {
        List<Transaction> res = this.byId.Values.Where(x =>  x.To == receiver)
             .OrderByDescending(x => x.Amount).ThenBy(x=>x.Id).ToList();
        if (res.Count == 0)
        {
            throw new InvalidOperationException();
        }
        return res;
    }

    public IEnumerable<Transaction> GetBySenderAndMinimumAmountDescending(string sender, double amount)
    {
        List<Transaction> res = this.byId.Values.Where(x => (x.Amount > amount) && x.From == sender)
            .OrderByDescending(x => x.Amount).ToList();
        if (res.Count == 0)
        {
            throw new InvalidOperationException();
        }
        return res;
    }

    public IEnumerable<Transaction> GetBySenderOrderedByAmountDescending(string sender)
    {
        List<Transaction> res = this.byId.Values.Where(x => x.From == sender)
             .OrderByDescending(x => x.Amount).ToList();
        if (res.Count == 0)
        {
            throw new InvalidOperationException();
        }
        return res;
    }

    public IEnumerable<Transaction> GetByTransactionStatus(TransactionStatus status)
    {
        if(!this.byStatusAndId.ContainsKey(status))
        {
            throw new InvalidOperationException();
        }
        return this.byStatusAndId[status].Values.OrderByDescending(x => x.Amount);
    }

    public IEnumerable<Transaction> GetByTransactionStatusAndMaximumAmount(TransactionStatus status, double amount)
    {
        List<Transaction> result = new List<Transaction>();
        if (!this.byStatusAndId.ContainsKey(status))
        {
            return result;
        }
        result = this.byStatusAndId[status].Values.Where(x => x.Amount <= amount)
            .OrderByDescending(x => x.Amount).ToList();
        return result;
    }

    public IEnumerator<Transaction> GetEnumerator()
    {
        foreach (int id in this.byId.Keys)
        {
            yield return this.byId[id];
        }
    }

    public void RemoveTransactionById(int id)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new IndexOutOfRangeException();
        }
        Transaction tr = this.byId[id];
        this.byStatusAndId[tr.Status].Remove(id);
        this.byId.Remove(id);

        this.count--;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

