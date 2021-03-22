using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomEnchant{
    public enum FigereTier{
    normal=1,magic=2,rare=3,unique=4 //enum value modify figere power
    }
    public enum ItemRank{
        normal=10,magic=5,rare=3,unique=1 //enum value modify item rarity frequency 
    }
    /*public abstract class BaseItemList{
        IList<
    }*/
    public interface IBaseItem {
        List<IPrefix<IStatus>> prefixList{get;}
        List<ISuffix<IStatus>> suffixList{get;}    
        ItemRank rank{get;}
        void apply(IStatusHolder holder);
        void remove(IStatusHolder holder);
        string name{get;}
        string toText();
    }
    public  abstract class BaseItem:IBaseItem{
        protected List<IPrefix<IStatus>> _prefixList=new List<IPrefix<IStatus>>();
        protected List<ISuffix<IStatus>> _suffixList=new List<ISuffix<IStatus>>();
        protected ItemRank _rank;
        public List<IPrefix<IStatus>> prefixList => _prefixList;

        public List<ISuffix<IStatus>> suffixList =>_suffixList;
        public ItemRank rank => _rank;
        protected string _name="Item";
        public string name => _name;

        protected BaseItem(ItemRank rank)
        {
            this._rank=rank;
        }
        protected BaseItem(ItemRank rank, List<IPrefix<IStatus>> prefixList, List<ISuffix<IStatus>> suffixList) 
        {
            this._prefixList=prefixList;
            this._suffixList=suffixList;
            this._rank=rank;
        }
       
        public void apply(IStatusHolder holder){
            if(_suffixList!=null)
            foreach (var item in _suffixList)
            {
                //Debug.Log(item.text);
                item.modifier.apply(holder);
            }
            if(_prefixList!=null)
            foreach (var item in _prefixList)
            {
                  item.modifier.apply(holder);
            }
          
        }
        public void remove(IStatusHolder holder){
            if(_suffixList!=null)
            foreach (var item in _suffixList)
            {
                //Debug.Log(item.text);
                item.modifier.remove(holder);
            }
            if(_prefixList!=null)
            foreach (var item in _prefixList)
            {
                  item.modifier.remove(holder);
            }
          
        }


        public string toText()
        {
            string res="";
            if(_suffixList!=null)
             foreach (var item in _suffixList)
            {
                res+=item.text+'\n';
                
            }
            if(_prefixList!=null)
            foreach (var item in _prefixList)
            {
                res+=item.text+'\n';
            }
            return res;
        }
        /* public BaseItem LoadMonsterData()
{
    switch(ItemName)
    {
        case DataTypeVersion.MonsterDataV1:
            return ZeroFormatterSerializer.Deserialize<MonsterDataV1>(Data);
        case DataTypeVersion.MonsterDataV2:
            return ZeroFormatterSerializer.Deserialize<MonsterDataV2>(Data);
        default:
            return null;
    }
}*/
    }
}