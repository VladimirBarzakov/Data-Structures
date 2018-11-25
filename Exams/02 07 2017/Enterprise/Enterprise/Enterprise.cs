using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

public class Enterprise : IEnterprise
{
    LinkedList<LinkedListNode<Employee>> byInsertion;
    Dictionary<Guid, LinkedListNode<Employee>> byId;
    Dictionary<Position, LinkedList<LinkedListNode<Employee>>> byPosition;
    OrderedDictionary<double, LinkedList<LinkedListNode<Employee>>> bySalary;

    public Enterprise()
    {
        this.byInsertion = new LinkedList<LinkedListNode<Employee>>();
        this.byId = new Dictionary<Guid, LinkedListNode<Employee>>();
        this.byPosition = new Dictionary<Position, LinkedList<LinkedListNode<Employee>>>();
        this.bySalary = new OrderedDictionary<double, LinkedList<LinkedListNode<Employee>>>();

    }

    public int Count => this.byInsertion.Count;

    public void Add(Employee employee)
    {
        if (this.Contains(employee))
        {
            return;
        }
        LinkedListNode<Employee> node = new LinkedListNode<Employee>(employee);
        this.byInsertion.AddLast(node);
        this.byId[employee.Id] = node;
        if (!this.byPosition.ContainsKey(employee.Position))
        {
            this.byPosition[employee.Position] = new LinkedList<LinkedListNode<Employee>>();
        }
        this.byPosition[employee.Position].AddLast(node);
        if (!this.bySalary.ContainsKey(employee.Salary))
        {
            this.bySalary[employee.Salary] = new LinkedList<LinkedListNode<Employee>>();
        }
        this.bySalary[employee.Salary].AddLast(node);
    }

    public IEnumerable<Employee> AllWithPositionAndMinSalary(Position position, double minSalary)
    {
        List<Employee> result = this.bySalary.RangeFrom(minSalary, true)
            .SelectMany(x=>x.Value).Where(x=>x.Value.Position==position).Select(x=>x.Value).ToList();
        return result;
    }

    public bool Change(Guid guid, Employee employee)
    {
        if (this.Contains(guid))
        {
            this.byId[guid].Value = employee;
            return true;
        }
        return false;
    }

    public bool Contains(Guid guid)
    {
        if (this.byId.ContainsKey(guid))
        {
            return true;
        }
        return false;
    }

    public bool Contains(Employee employee)
    {
        return this.Contains(employee.Id);
    }

    public bool Fire(Guid guid)
    {
        if (!this.Contains(guid))
        {
            return false;
        }
        LinkedListNode<Employee> node = this.byId[guid];
        this.bySalary[node.Value.Salary].Remove(node);
        this.byPosition[node.Value.Position].Remove(node);
        this.byInsertion.Remove(node);
        this.byId.Remove(guid);

        return true;
    }

    public Employee GetByGuid(Guid guid)
    {
        if (!this.Contains(guid))
        {
            throw new ArgumentException();
        }
        return this.byId[guid].Value;
    }

    public IEnumerable<Employee> GetByPosition(Position position)
    {
        if (!this.byPosition.ContainsKey(position))
        {
            return new List<Employee>();
        }
        return this.byPosition[position].Select(x=>x.Value);
    }

    public IEnumerable<Employee> GetBySalary(double minSalary)
    {
        if (this.Count==0 ||  this.bySalary.Keys.OrderByDescending(x=>x).First()>minSalary)
        {
            throw new InvalidOperationException();
        }
        return this.bySalary.RangeFrom(minSalary, true).SelectMany(x=>x.Value).Select(x=>x.Value);
    }

    public IEnumerable<Employee> GetBySalaryAndPosition(double salary, Position position)
    {
        List<Employee> result = this.bySalary[salary].Where(x=>x.Value.Position==position).Select(x=>x.Value).ToList();
        if (!result.Any())
        {
            throw new InvalidOperationException();
        }
        return result;
    }

    public IEnumerator<Employee> GetEnumerator()
    {
        foreach (var node in this.byInsertion)
        {
            yield return node.Value;
        }
    }

    public Position PositionByGuid(Guid guid)
    {
        if (!this.Contains(guid))
        {
            throw new InvalidOperationException();
        }
        return this.byId[guid].Value.Position;
    }

    public bool RaiseSalary(int months, int percent)
    {
        bool foundEmployeeToRaise = false;
        foreach (var item in this.byId.Values)
        {
            
        };
        return foundEmployeeToRaise;
    }

    public IEnumerable<Employee> SearchByFirstName(string firstName)
    {
        return this.byId.Values.Where(x=>x.Value.FirstName.Equals(firstName)).Select(x=>x.Value);
    }

    public IEnumerable<Employee> SearchByNameAndPosition(string firstName, string lastName, Position position)
    {
        List<Employee> result =  this.byPosition[position].Where(x=>x.Value.FirstName
        .Equals(firstName) && lastName.Equals(x.Value.FirstName)).Select(x=>x.Value).ToList();
        if (result.Count==0)
        {
            return new List<Employee>();
        }
        return result;
    }

    public IEnumerable<Employee> SearchByPosition(IEnumerable<Position> positions)
    {
        List<Employee> result = new List<Employee>();
        foreach (var position in positions)
        {
            if (!this.byPosition.ContainsKey(position))
            {
                continue;
            }
            foreach (var item in this.byPosition[position])
            {
                result.Add(item.Value);
            }
        }
        return result;
    }

    public IEnumerable<Employee> SearchBySalary(double minSalary, double maxSalary)
    {
        List<Employee> result = this.bySalary.Range(minSalary, true, maxSalary, true)
            .SelectMany(x => x.Value).Select(x => x.Value).ToList();
        if (!result.Any())
        {
            return new List<Employee>();
        }
        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator(); ;
    }
}

