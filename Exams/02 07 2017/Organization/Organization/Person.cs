public class Person
{
    public Person(string name, double salary)
    {
        this.Name = name;
        this.Salary = salary;
    }

    public string Name { get; set; }
    public double Salary { get; set; }

    public int CompareTo(Person other)
    {
        int comp = this.Name.CompareTo(other.Name);
        if (comp==0)
        {
            return this.Salary.CompareTo(other.Salary);
        }
        return comp;
    }
} 

