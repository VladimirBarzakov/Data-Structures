using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

public class RoyaleArena : IArena
{
    LinkedList<LinkedListNode<Battlecard>> byInsertion = new LinkedList<LinkedListNode<Battlecard>>();
    Dictionary<int, LinkedListNode<Battlecard>> byId = new Dictionary<int, LinkedListNode<Battlecard>>();
    Dictionary<CardType, Dictionary<int, LinkedListNode<Battlecard>>> byType = new Dictionary<CardType, Dictionary<int, LinkedListNode<Battlecard>>>();
    OrderedDictionary<double, Dictionary<int, LinkedListNode<Battlecard>>> byDamage = new OrderedDictionary<double, Dictionary<int, LinkedListNode<Battlecard>>>();
    OrderedDictionary<double, Dictionary<int, LinkedListNode<Battlecard>>> bySwag = new OrderedDictionary<double, Dictionary<int, LinkedListNode<Battlecard>>>();
    Dictionary<String, Dictionary<int, LinkedListNode<Battlecard>>> byName = new Dictionary<string, Dictionary<int, LinkedListNode<Battlecard>>>();

    public int Count => this.byId.Count;

    public void Add(Battlecard card)
    {
        if (this.Contains(card))
        {
            return;
        }
        LinkedListNode<Battlecard> node = new LinkedListNode<Battlecard>(card);
        this.byInsertion.AddLast(node);
        this.byId[card.Id] = node;
        if (!this.byType.ContainsKey(card.Type))
        {
            this.byType[card.Type] = new Dictionary<int, LinkedListNode<Battlecard>>();
        }
        this.byType[card.Type].Add(card.Id, node);
        if (!this.byDamage.ContainsKey(card.Damage))
        {
            this.byDamage[card.Damage] = new Dictionary<int, LinkedListNode<Battlecard>>();
        }
        this.byDamage[card.Damage][card.Id] = node;
        if (!this.bySwag.ContainsKey(card.Swag))
        {
            this.bySwag[card.Swag] = new Dictionary<int, LinkedListNode<Battlecard>>();
        }
        this.bySwag[card.Swag][card.Id] = node;
        if (!this.byName.ContainsKey(card.Name))
        {
            this.byName[card.Name] = new Dictionary<int, LinkedListNode<Battlecard>>();
        }
        this.byName[card.Name][card.Id] = node;
    }

    public void ChangeCardType(int id, CardType type)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new ArgumentException();
        }
        this.byType[this.byId[id].Value.Type].Remove(id);
        this.byId[id].Value.Type = type;
        if (!this.byType.ContainsKey(type))
        {
            this.byType[type] = new Dictionary<int, LinkedListNode<Battlecard>>();
        }
        this.byType[type][id] = this.byId[id];
    }

    public bool Contains(Battlecard card)
    {
        return this.byId.ContainsKey(card.Id);
    }

    public IEnumerable<Battlecard> FindFirstLeastSwag(int n)
    {
        if (n>this.Count)
        {
            throw new InvalidOperationException();
        }
        return this.bySwag.Values.SelectMany(x=>x.Values).Take(n).Select(x=>x.Value).OrderBy(x=>x.Swag).ThenBy(x=>x.Id);
    }

    public IEnumerable<Battlecard> GetAllByNameAndSwag()
    {
        List<Battlecard> result = new List<Battlecard>();
        foreach (var key in this.byName.Keys)
        {
            if (this.byName[key].Count == 0)
            {
                continue;
            }
            LinkedListNode<Battlecard> test = byName[key][this.byName[key].Keys.First()];
            foreach (var node in byName[key].Values)
            {
                if (test.Value.Swag<node.Value.Swag)
                {
                    test = node;
                }
            }
            result.Add(test.Value);
        }
        return result;
    }

    public IEnumerable<Battlecard> GetAllInSwagRange(double lo, double hi)
    {
        return this.bySwag.Range(lo,true,hi,true).Values
            .SelectMany(x=>x.Values).Select(x=>x.Value).OrderBy(x=>x.Swag);
    }

    public IEnumerable<Battlecard> GetByCardType(CardType type)
    {
        if (!this.byType.ContainsKey(type))
        {
            throw new InvalidOperationException();
        };
        return this.byType[type].Values.Select(x => x.Value).OrderByDescending(x => x.Damage).ThenBy(x => x.Id);
    }

    public IEnumerable<Battlecard> GetByCardTypeAndMaximumDamage(CardType type, double damage)
    { 
        if (!this.byType.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }
        List<Battlecard> result = new List<Battlecard>();
        result = this.byDamage.RangeTo(damage, true).SelectMany(x => x.Value)
            .Select(x => x.Value).Where(x => x.Value.Type == type).Select(x => x.Value)
            .OrderByDescending(x => x.Damage).ThenBy(x => x.Id).ToList();
        if (result.Count==0)
        {
            throw new InvalidOperationException();
        }
        return result;
    }

    public Battlecard GetById(int id)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new InvalidOperationException();
        }
        return this.byId[id].Value;
    }

    public IEnumerable<Battlecard> GetByNameAndSwagRange(string name, double lo, double hi)
    {
        List<Battlecard> result = new List<Battlecard>();
        result = this.bySwag.Range(lo, true, hi, false).Values.SelectMany(x => x.Values).Where(x => x.Value.Name == name).Select(x => x.Value).ToList();
        if (result.Count==0)
        {
            throw new InvalidOperationException();
        }
        return result.OrderByDescending(x => x.Swag).ThenBy(x => x.Id);
    }

    public IEnumerable<Battlecard> GetByNameOrderedBySwagDescending(string name)
    {
        if (!this.byName.ContainsKey(name) ||this.byName[name].Count==0)
        {
            throw new InvalidOperationException();
        }
        return this.byName[name].Values.OrderByDescending(x => x.Value.Swag).ThenBy(x => x.Value.Id).Select(x => x.Value);
    }

    public IEnumerable<Battlecard> GetByTypeAndDamageRangeOrderedByDamageThenById(CardType type, int lo, int hi)
    {
        if (!this.byType.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }
        List<Battlecard> result = new List<Battlecard>();
        result = this.byDamage.Range(lo,false,hi, true).SelectMany(x => x.Value)
            .Select(x => x.Value).Where(x => x.Value.Type == type).Select(x => x.Value)
            .OrderByDescending(x => x.Damage).ThenBy(x => x.Id).ToList();
        if (result.Count == 0)
        {
            throw new InvalidOperationException();
        }
        return result; ;
    }

    public IEnumerator<Battlecard> GetEnumerator()
    {
        foreach (var item in this.byInsertion)
        {
            yield return item.Value;
        }
    }

    public void RemoveById(int id)
    {
        if (!this.byId.ContainsKey(id))
        {
            throw new InvalidOperationException();
        }
        LinkedListNode<Battlecard> node = this.byId[id];
        this.bySwag[node.Value.Swag].Remove(id);
        this.byDamage[node.Value.Damage].Remove(id);
        this.byType[node.Value.Type].Remove(id);
        this.byName[node.Value.Name].Remove(id);
        this.byId.Remove(id);
        this.byInsertion.Remove(node);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
