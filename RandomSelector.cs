using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace RandomEnchant{
public class WeightedRandomSelector<Item>:RandomSelector<Item> where Item:IRandom
{
    public WeightedRandomSelector(IReadOnlyCollection<Item> inputs) : base(inputs)
    {
         this.temps= (new List<Item>(inputs)).Select(ele=>new Temp(ele,ele.frequency)).ToList();

    }

   /* public override List<Item> getItem(int takeCount){
        int[] range=new int[temps.Count];
        if(temps.Count==0)
            return new List<Item>();
       int sum=0;
        {
            int i=0;
            foreach (Temp item in temps)
            {
                
                range[i]=sum;
                //Debug.Log(sum);
                sum+=item.frequency;
                i++;
            }
        }
        
        List<Item> resList=new List<Item>();
        for(int i=0;i<takeCount;i++){
            var randomByte = new byte[4];
            using(var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomByte);
            }
            int res=Math.Abs(System.BitConverter.ToInt32(randomByte,0))%sum+1;
            Debug.Log(res);
            int index = UpperBound<int>(range, res )-1;
            //Debug.Log(index);
            if(index<0 || index>=temps.Count)
                continue;
            resList.Add(temps.ElementAt(index));
            int decreasedAmount=temps.ElementAt(index).frequency;
            sum-=decreasedAmount;
            for(int pos=0;pos<temps.Count;pos++){
                if(pos>=index){
                    range[pos]-=decreasedAmount;
                }
            }
        }
        return resList;
    }*/
}
public class RandomSelector<Item>
{
    
    protected IReadOnlyCollection<Temp> temps;
    protected class Temp{
        public Item item;
        public int frequency;

            public Temp( Item item,int frequency)
            {
                this.frequency = frequency;
                this.item = item;
            }
        }
    public RandomSelector(IReadOnlyCollection<Item> inputs)
    {
        this.temps= (new List<Item>(inputs)).Select(ele=>new Temp(ele,1)).ToList();

    }
    public Item getItem(){
        return getItem(1).First();
    }
        
    public virtual List<Item> getItem(int takeCount){
        int[] range=new int[temps.Count];
        if(temps.Count==0)
            return new List<Item>();
        int sum=0;
        {
            int i=0;
            foreach (Temp item in temps)
            {
                range[i]=sum;
                //Debug.Log(sum);
                sum+=item.frequency;
                i++;
            }
        }
        List<Item> resList=new List<Item>();
        for(int i=0;i<takeCount;i++){
            //Debug.Log(string.Join(", ", range));
            var randomByte = new byte[4];
            using(var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomByte);
            }
            int res=Math.Abs(System.BitConverter.ToInt32(randomByte,0))%sum;
            int index = UpperBound<int>(range, res );
            //Debug.Log(index);
            if(index-1<0 || index-1>=temps.Count)
                continue;
            resList.Add(temps.ElementAt(index-1).item);
            int decreasedAmount=temps.ElementAt(index-1).frequency;
            sum-=decreasedAmount;
            for(int pos=0;pos<temps.Count;pos++){
                if(pos>=index){
                    range[pos]-=decreasedAmount;
                }
            }
        }
        return resList;
    }
    public int UpperBound<T>(T[] a, T v)
    { 
        return UpperBound(a, v, Comparer<T>.Default);
    }

    public  int UpperBound<T>(T[] a, T v, Comparer<T> cmp)
    {
        var l = 0;
        var r = a.Length - 1;
        while (l <= r)
        {
            var mid = l + (r - l) / 2;
            var res = cmp.Compare(a[mid], v);
            if (res <= 0) l = mid + 1;
            else r = mid - 1;
        }
        return l;
    }
}

public interface IRandom{
    int frequency{get;}
}
public interface IGroup{
    int[] group{get;}
}
}