using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;
using System.Linq;

public class Instock : IProductStock
{
    private List<Product> listProducts;
    private Dictionary<string, Product> hashList;
    private OrderedSet<Product> orderedByName;
    private OrderedDictionary<double, List<Product>> orderedByPrice;
    private OrderedDictionary<int, HashSet<string>> orderedByQuantity;

    public class myPriceCompararer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            return y.CompareTo(x);
        }
    }


    public Instock()
    {
        this.listProducts = new List<Product>();
        this.hashList = new Dictionary<string, Product>();
        this.orderedByName = new OrderedSet<Product>();
        this.orderedByPrice = new OrderedDictionary<double, List<Product>>(new myPriceCompararer());
        this.orderedByQuantity = new OrderedDictionary<int, HashSet<string>>();
    }

    public int Count => this.listProducts.Count;

    public void Add(Product product)
    {
        this.listProducts.Add(product);
        this.hashList.Add(product.Label, product);
        this.orderedByName.Add( product);
        if (!this.orderedByPrice.ContainsKey(product.Price))
        {
            this.orderedByPrice[product.Price] = new List<Product>();
        }
        this.orderedByPrice[product.Price].Add(product);
        if (!this.orderedByQuantity.ContainsKey(product.Quantity))
        {
            this.orderedByQuantity[product.Quantity] = new HashSet<string>();
        }
        this.orderedByQuantity[product.Quantity].Add(product.Label);
    }

    public void ChangeQuantity(string product, int quantity)
    {
        if (!this.hashList.ContainsKey(product))
        {
            throw new ArgumentException();
        }
        this.orderedByQuantity[this.hashList[product].Quantity].Remove(product);
        this.hashList[product].Quantity=quantity;
        if (!this.orderedByQuantity.ContainsKey(quantity))
        {
            this.orderedByQuantity[quantity] =new HashSet<string>();
        }
        this.orderedByQuantity[quantity].Add(product);
    }

    public bool Contains(Product product)
    {
        return this.hashList.ContainsKey(product.Label);
    }

    public Product Find(int index)
    {
        if (index< 0 || index>=this.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return this.listProducts[index];
    }

    public IEnumerable<Product> FindAllByPrice(double price)
    {
        if (this.orderedByPrice.ContainsKey(price))
        {
            return this.orderedByPrice[price];
        } else
        {
            return new List<Product>();
        }
        
    }

    public IEnumerable<Product> FindAllByQuantity(int quantity)
    {
        List<Product> result = new List<Product>();
        if (!this.orderedByQuantity.ContainsKey(quantity))
        {
            return result;
        }
        foreach (string name in this.orderedByQuantity[quantity])
        {
            result.Add(this.hashList[name]);
        };
        return result;
    }

    public IEnumerable<Product> FindAllInRange(double lo, double hi)
    {
        return this.orderedByPrice.Range(hi, true, lo, false).Select(x=>x.Value).SelectMany(x=>x);
    }

    public Product FindByLabel(string label)
    {
        if (!this.hashList.ContainsKey(label))
        {
            throw new ArgumentException();
        }
        return this.hashList[label];
    }

    public IEnumerable<Product> FindFirstByAlphabeticalOrder(int count)
    {
        if (count < 0 || count > this.Count)
        {
            throw new ArgumentException();
        }

        return this.orderedByName.Take(count);
    }

    public IEnumerable<Product> FindFirstMostExpensiveProducts(int count)
    {
        if (count < 0 || count > this.listProducts.Count)
        {
            throw new ArgumentException();
        }
        List<Product> list = new List<Product>(count);
        foreach (double price in this.orderedByPrice.Keys)
        {
            list.AddRange(this.orderedByPrice[price]);
            if (list.Count>=count)
            {
                break;
            }
        }
        return list.Take(count);
    }

    public IEnumerator<Product> GetEnumerator()
    {
        foreach(Product product in this.listProducts)
        {
            yield return product;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
