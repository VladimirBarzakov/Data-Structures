using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

public class Organization : IOrganization
{
    List<LinkedListNode<Person>> byInsertion;
    Dictionary<string, List<LinkedListNode<Person>>> byName;
    OrderedDictionary<int, List<LinkedListNode<Person>>> byNameSize;
    

    public Organization()
    {
        this.byInsertion = new List<LinkedListNode<Person>>();
        this.byName = new Dictionary<string, List<LinkedListNode<Person>>>();
        this.byNameSize = new OrderedDictionary<int, List<LinkedListNode<Person>>>();
    }

    public IEnumerator<Person> GetEnumerator()
    {
        foreach (var item in byInsertion)
        {
            yield return item.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count { get { return this.byInsertion.Count; } }

    public bool Contains(Person person)
    {
        if (this.byName.ContainsKey(person.Name))
        {
            foreach (var node in this.byName[person.Name])
            {
                if (person.Salary==node.Value.Salary)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public bool ContainsByName(string name)
    {
        if (this.byName.ContainsKey(name))
        {
            return true;
        }
        return false;
    }

    public void Add(Person person)
    {
        LinkedListNode<Person> node = new LinkedListNode<Person>(person);
        this.byInsertion.Add(node);
        if (!this.byName.ContainsKey(person.Name))
        {
            this.byName[person.Name] = new List<LinkedListNode<Person>>();
        }
        this.byName[person.Name].Add(node);
        if (!this.byNameSize.ContainsKey(person.Name.Length))
        {
            this.byNameSize[person.Name.Length] = new List<LinkedListNode<Person>>();
        }
        this.byNameSize[person.Name.Length].Add(node);


    }
    public Person GetAtIndex(int index)
    {
        if (index<0 || index>=this.Count)
        {
            throw new System.IndexOutOfRangeException();
        }
        return this.byInsertion[index].Value;
    }

    public IEnumerable<Person> GetByName(string name)
    {
        List<Person> result = new List<Person>();
        if (this.byName.ContainsKey(name))
        {
            result = new List<Person>(this.byName[name].Select(x => x.Value));
        }
        return result;
    }

    public IEnumerable<Person> FirstByInsertOrder(int count = 1)
    {
        return this.byInsertion.Take(count).Select(x => x.Value);
    }

    public IEnumerable<Person> SearchWithNameSize(int minLength, int maxLength)
    {
        return this.byNameSize.Range(minLength, true, maxLength, true)
            .SelectMany(x=>x.Value).Select(x=>x.Value);
    }

    public IEnumerable<Person> GetWithNameSize(int length)
    {
        if (!this.byNameSize.ContainsKey(length))
        {
            throw new System.ArgumentException();
        }
        return this.byNameSize[length].Select(x=>x.Value);
    }

    public IEnumerable<Person> PeopleByInsertOrder()
    {
        foreach (var item in byInsertion)
        {
            yield return item.Value;
        }
    }
}