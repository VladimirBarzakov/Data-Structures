using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

/// <summary>
/// The ThreadExecutor is the concrete implementation of the IScheduler.
/// You can send any class to the judge system as long as it implements
/// the IScheduler interface. The Tests do not contain any <e>Reflection</e>!
/// </summary>
public class ThreadExecutor : IScheduler
{
    int IScheduler.Count => this.Count();

    LinkedList<LinkedListNode<Task>> byInsertion;
    Dictionary<int, LinkedListNode<Task>> byId;
    Dictionary<Priority, Dictionary<int, LinkedListNode<Task>>> byPriority;
    OrderedDictionary<int, Dictionary<int, LinkedListNode<Task>>> byConsumption;
    int cycleOffset = 0;

    public class myIdCompararer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

    public ThreadExecutor()
    {
        this.byInsertion = new LinkedList<LinkedListNode<Task>>();
        this.byId = new Dictionary<int, LinkedListNode<Task>>();
        this.byPriority = new Dictionary<Priority, Dictionary<int, LinkedListNode<Task>>>();
        this.byConsumption = new OrderedDictionary<int, Dictionary<int, LinkedListNode<Task>>>();
    }

    public int Count()
    {
        return byId.Count;
    }


    public void ChangePriority(int id, Priority newPriority)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new ArgumentException();
        }
        
        this.byPriority[this.byId[id].Value.TaskPriority].Remove(id);
        this.byId[id].Value.TaskPriority = newPriority;
        if (!this.byPriority.ContainsKey(newPriority))
        {
            this.byPriority[newPriority] = new Dictionary<int, LinkedListNode<Task>>();
        }
        this.byPriority[newPriority].Add(id, this.byId[id]);
    }

    public bool Contains(Task task)
    {
        return this.byId.ContainsKey(task.Id);
    }

    public int Cycle(int cycles)
    {
        if (this.Count()==0 || cycles<=0)
        {
            throw new InvalidOperationException();
        }
        List<int> keys = new List<int>();
        int count = 0;
        foreach (int key in this.byConsumption.Keys)
        {
            if (key - cycleOffset - cycles <= 0)
            {
                foreach (LinkedListNode<Task> node in this.byConsumption[key].Values)
                {
                    this.byInsertion.Remove(node);
                    this.byPriority[node.Value.TaskPriority].Remove(node.Value.Id);
                    this.byId.Remove(node.Value.Id);
                    count++;
                }
                keys.Add(key);
            }
            else
            {
                break;
            }
        }
        foreach (int key in keys)
        {
            this.byConsumption.Remove(key);
        }

        this.cycleOffset += cycles;
        return count;
    }

    public void Execute(Task task)
    {
        if (this.Contains(task))
        {
            throw new ArgumentException();
        }
        LinkedListNode<Task> node = new LinkedListNode<Task>(task);
        this.byInsertion.AddLast(node);
        this.byId.Add(task.Id, node);
        if (!this.byPriority.ContainsKey(task.TaskPriority))
        {
            this.byPriority[task.TaskPriority] = new Dictionary<int, LinkedListNode<Task>>();
        }
        this.byPriority[task.TaskPriority][task.Id] = node;
        if (!this.byConsumption.ContainsKey(task.Consumption+cycleOffset))
        {
            this.byConsumption[task.Consumption+cycleOffset] = new Dictionary<int, LinkedListNode<Task>>();
        }
        this.byConsumption[task.Consumption][task.Id]=node;

    }

    public IEnumerable<Task> GetByConsumptionRange(int lo, int hi, bool inclusive)
    {
        return  this.byConsumption.Range(lo+cycleOffset,inclusive,hi+cycleOffset,inclusive).Values.SelectMany(x=>x.Values).Select(x=>x.Value)
            .OrderBy(x=>x.Consumption)
            .ThenByDescending(x=>x.TaskPriority);

    }

    public Task GetById(int id)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new ArgumentException();
        }
        return this.byId[id].Value;
    }

    public Task GetByIndex(int index)
    {
        if (index<0||index>=this.Count())
        {
            throw new ArgumentOutOfRangeException();
        }
        int count= -1;
        foreach (var item in this.byInsertion)
        {
            count++;
            if (count==index)
            {
                return item.Value;
            }
        }
        return null;
    }

    public IEnumerable<Task> GetByPriority(Priority type)
    {
        List<Task> result = new List<Task>();
        if (!this.byPriority.ContainsKey(type))
        {
            return result;
        }
        return this.byPriority[type].Values.Select(x=>x.Value).OrderByDescending(x=>x.Id);
    }

    public IEnumerable<Task> GetByPriorityAndMinimumConsumption(Priority priority, int lo)
    {
        List<Task> result = new List<Task>();
        if (!this.byPriority.ContainsKey(priority))
        {
            return result;
        }
        return this.byConsumption.RangeFrom(lo-cycleOffset, true).Values.SelectMany(x => x.Values)
            .Where(x => x.Value.TaskPriority == priority).Select(x => x.Value).OrderByDescending(x => x.Id);
    }


    public IEnumerator<Task> GetEnumerator()
    {
        foreach (var item in this.byInsertion)
        {
             yield return item.Value; 
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
