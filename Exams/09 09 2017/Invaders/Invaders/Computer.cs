using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

public class Computer : IComputer
{
    LinkedList<LinkedListNode<Invader>> byInsertion;
    OrderedDictionary<int, List<LinkedListNode<Invader>>> byDistance;

    private int energy { get; set; }

    public class myCompararer : IComparer<LinkedListNode<Invader>>
    {
        public int Compare(LinkedListNode<Invader> x, LinkedListNode<Invader> y)
        {
            return y.Value.Damage.CompareTo(x.Value.Damage);
        }
    }

    public class myKeyComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }
    }

    public Computer(int energy)
    {
        if (energy<0)
        {
            throw new ArgumentException();
        }
        this.energy = energy;
        this.byInsertion = new LinkedList<LinkedListNode<Invader>>();
        this.byDistance = new OrderedDictionary<int, List<LinkedListNode<Invader>>>(new myKeyComparer());
    }

    public int Energy
    {
        get
        {
            if (this.energy<0)
            {
                this.energy = 0;
            }
            return this.energy;
        }
        set {
            this.energy = this.Energy;
        }
    }

    public void Skip(int turns)
    {
        List<int> keys = new List<int>(this.byDistance.Keys);
        foreach (int key in keys)
        {
            if (key-turns<=0)
            {
                foreach (LinkedListNode<Invader> node in this.byDistance[key])
                {
                    if (!node.Value.isDeleted)
                    {
                        this.energy -= node.Value.Damage;
                        node.Value.isDeleted = true;
                    }                 
                }
                this.byDistance.Remove(key);
                continue;
            }
            if (!this.byDistance.ContainsKey(key-turns))
            {
                this.byDistance[key - turns] = new List<LinkedListNode<Invader>>(this.byDistance[key]);
            }
            foreach (LinkedListNode<Invader> node in this.byDistance[key-turns])
            {
                if (!node.Value.isDeleted)
                {
                    node.Value.Distance = key - turns;
                }
            }
            this.byDistance.Remove(key);
        }
    }

    public void AddInvader(Invader invader)
    {
        LinkedListNode<Invader> node = new LinkedListNode<Invader>(invader);
        this.byInsertion.AddLast(node);
        if (!this.byDistance.ContainsKey(invader.Distance))
        {
            this.byDistance[invader.Distance] = new List<LinkedListNode<Invader>>();
        }
        this.byDistance[invader.Distance].Add(node);
    }

    public void DestroyHighestPriorityTargets(int count)
    {
        if (count==0)
        {
            return;
        }
        foreach (int key in this.byDistance.Keys)
        {
            this.byDistance[key].Sort(new myCompararer());
            foreach (var item in this.byDistance[key])
            {
                if (item.Value.isDeleted)
                {
                    continue;
                }
                item.Value.isDeleted = true;
                count--;
                if (count == 0)
                {
                    return;
                }
            }
        }

    }

    public void DestroyTargetsInRadius(int radius)
    {
        foreach (var item in this.byInsertion)
        {
            if (item.Value.Distance<=radius)
            {
                item.Value.isDeleted = true;
            }
        }
        //for (int i = 0; i <= radius; i++)
        //{
        //    if (!this.byDistance.ContainsKey(i) || this.byDistance[i].Count==0)
        //    {
        //        continue;
        //    }
        //    for (int k = byDistance[i].Count - 1; k >= 0; k--)
        //    {
        //        this.byInsertion.Remove(this.byDistance[i][k]);
        //        this.byDistance[i].RemoveAt(k);
        //    }
        //
        //}
    }

    public IEnumerable<Invader> Invaders()
    {
        foreach (LinkedListNode<Invader> node in this.byInsertion)
        {
            if (!node.Value.isDeleted)
            {
                yield return node.Value;
            }
            
        }
    }
}
