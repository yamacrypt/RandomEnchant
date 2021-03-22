using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroFormatter;
using ZeroFormatter.Formatters;
using System.Reflection;
using ZeroFormatter.Internal;

namespace RandomEnchant{

    public interface IFormattable<T>{
    string  DataType{get;set;}
    byte[] Data { get; set; }
    bool Set(T t);
    T Load();
   // bool Set(FormattableHolder holder);
    
    }
    [ZeroFormattable]
    public class FormattableMaster
    {
        [Index(0)]
        public virtual string FormattableName{get;set;}
        [Index(1)]
        public virtual  string  DataType{get;set;}
        [Index(2)]
        public virtual byte[] Data { get; set; }
        static Dictionary<Type,Type[]> CacheInterfaceDictionary=new Dictionary<Type, Type[]>();
        public T Load<T> (){
            IFormattable<T> formattable= (IFormattable<T>)Activator.CreateInstance(TypeUtils.StringToType(FormattableName));
            formattable.Data=Data;
            formattable.DataType=DataType;
            return formattable.Load();
        }
        public bool Set<T>(T t){
            //var res=Activator.CreateInstance(type,ItemRank.magic);
            Type[] arr;
            if(!CacheInterfaceDictionary.ContainsKey(typeof(T))){
                arr=InterfaceUtils.GetInterfaces<IFormattable<T>>();
                if(arr.Length!=1)
                    throw new Exception();
                CacheInterfaceDictionary[typeof(T)]=arr;
            }
            else
                arr=CacheInterfaceDictionary[typeof(T)];;
            //if(arr.Length!=1)
            //    throw new Exception();
            var target=arr.First();
            IFormattable<T> formattable= (IFormattable<T>)Activator.CreateInstance(target);
            formattable.Set(t);
            //return new FormattableMaster{
            FormattableName= TypeUtils.TypeToString(formattable.GetType());
            DataType=formattable.DataType;
            Data=formattable.Data;
            return true;

            //};
            
        }
    }
    public interface IZeroFormattable{
       object Deserialize(byte[] Data);
       byte[] Serialize(object obj);
    }
    [ZeroFormattable]
    public class FormattableStatus:IFormattable<IStatus>{
        [Index(0)]
        public virtual string  DataType{get;set;}
        [Index(1)]
        public virtual byte[] Data { get; set; }
        public bool Set(IStatus data)
        {
            bool ret = true;
            Data = ZeroFormatterSerializer.Serialize(data.baseValue);
            Type type=data.GetType();
            if(type.IsSubclassOf(typeof(Status)))
                
            DataType=TypeUtils.TypeToString(type);//$"{type}, {type.Assembly.GetName().Name}";
            //type=data.GetType();
            return ret;
        }

        public IStatus Load()
        {
            int baseValue=ZeroFormatterSerializer.Deserialize<int>(Data);
            IStatus res= (IStatus)Activator.CreateInstance(TypeUtils.StringToType(DataType),args:baseValue);
            return res;
        }
    }
    [ZeroFormattable]
    public class StatusModifierCore{
            [Index(0)]
            public virtual OperatorType type{get;set;}

            [Index(1)]
            public virtual int amount{get;set;}
            

    }

     [ZeroFormattable]
    public class FormattableStatusModifier :IFormattable<IStatusModifier<IStatus>>  {
        [Index(0)]
        public virtual string  DataType{get;set;}
        [Index(1)]
        public virtual byte[] Data { get; set; }
        public bool Set(IStatusModifier<IStatus> data)
        {
            bool ret = true;
            Data = ZeroFormatterSerializer.Serialize(new StatusModifierCore{type=data.type,amount=data.amount});
            //Type type=data.GetType();
            DataType=TypeUtils.TypeToString(data.GetType().GenericTypeArguments.First());
            //DataType=TypeUtils.TypeToString(type);
            //type=data.GetType();
            return ret;
        }
        

        public IStatusModifier<IStatus> Load()
        {  
            Type template = typeof(StatusModifier<>);
            Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            StatusModifierCore core=ZeroFormatterSerializer.Deserialize<StatusModifierCore>(Data);
            return (IStatusModifier<IStatus>)Activator.CreateInstance(type,args:core);
            //return res;
        }
       /* public dynamic LoadDynamic()
        {
            StatusModifierCore core=ZeroFormatterSerializer.Deserialize<StatusModifierCore>(Data);
            return Activator.CreateInstance(Type.GetType(DataType),core.type,core.amount);
        }*/
    }
    [ZeroFormattable]
    public class FigereCore{
        [Index(0)]
        public virtual StatusModifierCore statusModifierCore{get;set;}
        [Index(1)]
        public virtual FigereTier tier{get;set;}
    }

    [ZeroFormattable]
    public class FormattableSuffix : IFormattable<ISuffix<IStatus>>
    {
        [Index(0)]
        public virtual string DataType {get;set;}
        [Index(1)]
        public  virtual byte[] Data { get;set;}
        public ISuffix<IStatus> Load()
        {
            var core= ZeroFormatterSerializer.Deserialize<FigereCore>(Data);
            Type template = typeof(Suffix<>);
            Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            //Debug.Log(type);
            try
            {
            return (ISuffix<IStatus>)Activator.CreateInstance( type,core.statusModifierCore,core.tier);
            }
            catch (System.Exception)
            {
                
                return (ISuffix<IStatus>)Activator.CreateInstance( type,OperatorType.Additive,5,FigereTier.magic);
            }
        }

        public bool Set(ISuffix<IStatus> t)
        {
            var statusModifierCore=new StatusModifierCore{type=t.modifier.type,amount=t.modifier.amount};
            Data=ZeroFormatterSerializer.Serialize(new FigereCore{statusModifierCore=statusModifierCore,tier=t.tier});
            DataType=TypeUtils.TypeToString(t.GetType().GenericTypeArguments.First());
            return true;
        }
    }
    [ZeroFormattable]
    public class FormattablePrefix : IFormattable<IPrefix<IStatus>>
    {
        [Index(0)]
        public virtual string DataType {get;set;}
        [Index(1)]
        public  virtual byte[] Data { get;set;}
        public IPrefix<IStatus> Load()
        {
            var core= ZeroFormatterSerializer.Deserialize<FigereCore>(this.Data);
            Type template = typeof(Prefix<>);
            Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            //Debug.Log(type);
            try
            {
                return (IPrefix<IStatus>)Activator.CreateInstance(type,core.statusModifierCore,core.tier);
            }
            catch (System.Exception)
            {
                
                return (IPrefix<IStatus>)Activator.CreateInstance(type,OperatorType.Additive,5,FigereTier.magic);
            }
            
        }

        public bool Set(IPrefix<IStatus> t)
        {
            var statusModifierCore=new StatusModifierCore{type=t.modifier.type,amount=t.modifier.amount};
            this.Data=ZeroFormatterSerializer.Serialize(new FigereCore{statusModifierCore=statusModifierCore,tier=t.tier});
            //Debug.Log(TypeUtils.TypeToSTring(t.GetType()));
            this.DataType=TypeUtils.TypeToString(t.GetType().GenericTypeArguments.First());
            return true;
        }
    }
   [ZeroFormattable]
    public class ItemCore
    {
        [Index(0)]
        public virtual ItemRank rank{get;set;}
        [Index(1)]
        public virtual IList<FormattablePrefix> prefixList{get; set;}
        [Index(2)]
        public virtual IList<FormattableSuffix> suffixList{get; set;}
       

    }
    [ZeroFormattable]
    public class FormattableItem : IFormattable<IBaseItem>
    {
        [Index(0)]
        public virtual string DataType { get; set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }

        public IBaseItem Load()
        {
            //Debug.Log("ItemLoad");
            var core=ZeroFormatterSerializer.Deserialize<ItemCore>(Data);
            return (IBaseItem)Activator.CreateInstance(TypeUtils.StringToType(DataType),
            core.rank,
            core.prefixList.Select(ele=>{var res=ele.Load();return res;}).ToList<IPrefix<IStatus>>(),
            core.suffixList.Select(ele=>{var res=ele.Load();return res;}).ToList<ISuffix<IStatus>>()
            );
        }

        public bool Set(IBaseItem t)
        {
            //Debug.Log("ItemSet");
            var itemCore=new ItemCore{
                rank=t.rank,
                prefixList= t.prefixList.Select(ele=>{var res=new FormattablePrefix();res.Set(ele);return res;}).ToList(),
                suffixList= t.suffixList.Select(ele=>{var res=new FormattableSuffix();res.Set(ele);return res;}).ToList()
                };
            Data=ZeroFormatterSerializer.Serialize(itemCore);
            DataType=TypeUtils.TypeToString(t.GetType());
            return true;
        }

    }
    
    [ZeroFormattable]
    public class ItemListCore{
        [Index(0)]
        public virtual IList<FormattableItem> list{get;set;}

       /* public ItemListCore(IList<FormattableItem> list)
        {
            this.list = list;
        }*/
    }
   
    [ZeroFormattable]
    public class FormattableItemList : IFormattable<IList<IBaseItem>>
    {
        [Index(0)]
        public virtual string DataType { get; set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }

        public bool Set(IList<IBaseItem> t)
        {
            Data=ZeroFormatterSerializer.Serialize(
            new ItemListCore(){
                list=(IList<FormattableItem>)t.Select(ele=>{var res=new FormattableItem();res.Set(ele);return res;}).ToList()
            }
            );
            DataType="not used";
            //Type template = typeof(List<>);
            //Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            //Type type=t.GetType().GetGenericArguments().First();
            //DataType=TypeUtils.TypeToSTring(type);
            return true;
        }

        public IList<IBaseItem> Load()
        {
            var core=ZeroFormatterSerializer.Deserialize<ItemListCore>(Data);
            //Type template = typeof(List<>);
            //Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            
            return core.list.Select(ele=>ele.Load()).ToList();
        }

    }
    [ZeroFormattable]
    public class StatusListCore{
        [Index(0)]
        public virtual IList<FormattableStatus> list{get;set;}

    }
   
    [ZeroFormattable]
    public class FormattableStatusList : IFormattable<IList<IStatus>>
    {
        [Index(0)]
        public virtual string DataType { get; set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }

        public bool Set(IList<IStatus> t)
        {
            Data=ZeroFormatterSerializer.Serialize(
            new StatusListCore(){
                list=
                (IList<FormattableStatus>)t.Select(ele=>{var res=new FormattableStatus();res.Set(ele);return res;}).ToList()
            }
            );
            DataType="not used";
            //Type template = typeof(List<>);
            //Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            //Type type=t.GetType().GetGenericArguments().First();
            //DataType=TypeUtils.TypeToSTring(type);
            return true;
        }

        public IList<IStatus> Load()
        {
            //var core=ZeroFormatterSerializer.Deserialize<StatusListCore>(Data);
            //Type template = typeof(List<>);
            //Type type=template.MakeGenericType(TypeUtils.StringToType(DataType));
            
            return ZeroFormatterSerializer.Deserialize<StatusListCore>(Data).list.Select(ele=>ele.Load()).ToList();
        }

    }
    [ZeroFormattable]
    public class ModifierListCore
    {
        [Index(0)]
        public virtual IList<FormattableStatusModifier> list{get;set;}
    }
    
    [ZeroFormattable]
    public class FormattableModifierList : IFormattable<IList<IStatusModifier<IStatus>>>
    {
        [Index(0)]
        public virtual string DataType { get; set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }


        public bool Set(IList<IStatusModifier<IStatus>> t)
        {
             Data=ZeroFormatterSerializer.Serialize(
                 new ModifierListCore{
                list=(IList<FormattableStatusModifier>)t.Select(ele=>{var res=new FormattableStatusModifier();res.Set(ele);return res;}).ToList()
                }
            );
            DataType="not used";
            return true;
        }

        public IList<IStatusModifier<IStatus>> Load()
        {
            return ZeroFormatterSerializer.Deserialize<ModifierListCore>(Data).list.Select(ele=>ele.Load()).ToList();
        }

    }
    [ZeroFormattable]
    public class StatusHolderCore{
        [Index(0)]
        public virtual IList<FormattableStatus> statusList{get;set;}
        [Index(1)]
        public virtual FormattableModifierList modifierList{get;set;}
    }
    [ZeroFormattable]
    public class FormattableStatusHolder : IFormattable<IStatusHolder>
    {
        [Index(0)]
        public virtual string DataType { get ;set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }


        public IStatusHolder Load()
        {
            var core=ZeroFormatterSerializer.Deserialize<StatusHolderCore>(Data);
            //var a=core.modifierList.Load();
            return (IStatusHolder)Activator.CreateInstance(TypeUtils.StringToType(DataType),
                core.statusList.Select(
                    ele=>ele.Load()//(TypeUtils.StringToType(ele.Key),ele.Value.Load())
                    ).ToList(),
                core.modifierList.Load()
            );
        }

        public bool Set(IStatusHolder t)
        {
            FormattableModifierList formmatable=new FormattableModifierList();
            formmatable.Set(t.permanentModifierList.ToList());
            var core=new StatusHolderCore{
                statusList=t.statusDictionary.Select(ele=>
                {
                    FormattableStatus status=new FormattableStatus();
                    status.Set(ele.Value);
                    return status;
                }).ToList(),
               modifierList=formmatable
                //.ToDictionary(ele=>ele.Item1,ele=>ele.status)
            };
            Data=ZeroFormatterSerializer.Serialize(core);
            DataType=TypeUtils.TypeToString(t.GetType());
            return true;
        }
    }
     [ZeroFormattable]
    public class StatusHolderListCore
    {
        [Index(0)]
        public virtual IList<FormattableStatusHolder> list{get;set;}
       
    }
     [ZeroFormattable]
    public class FormattableStatusHolderIList : IFormattable<IList<IStatusHolder>>
    {
        [Index(0)]
        public virtual string DataType { get; set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }


       public bool Set(IList<IStatusHolder> t)
        {
             Data=ZeroFormatterSerializer.Serialize(
                 new StatusHolderListCore{
                list=(IList<FormattableStatusHolder>)t.Select(ele=>{var res=new FormattableStatusHolder();res.Set(ele);return res;}).ToList()
                 }
            );
            DataType="not used";
            return true;
        }

        public IList<IStatusHolder> Load()
        {
            return ZeroFormatterSerializer.Deserialize<StatusHolderListCore>(Data).list.Select(ele=>ele.Load()).ToList();
        }
    }
     [ZeroFormattable]
    public class ElementsHolderCore{
        [Index(0)]
        public virtual IList<FormattableStatusHolder> statusList{get;set;}
        [Index(1)]
        public virtual FormattableModifierList modifierList{get;set;}
    }
    [ZeroFormattable]
    public class FormattableElementsHolder : IFormattable<IElementsHolder>
    {
        [Index(0)]
        public virtual string DataType { get ;set; }
        [Index(1)]
        public virtual byte[] Data { get ;set; }


        public IElementsHolder Load()
        {
            var core=ZeroFormatterSerializer.Deserialize<ElementsHolderCore>(Data);
            return (IElementsHolder)Activator.CreateInstance(TypeUtils.StringToType(DataType),
                core.statusList.Select(
                    ele=>ele.Load()//(TypeUtils.StringToType(ele.Key),ele.Value.Load())
                    ).ToList(),
                core.modifierList.Load()
            );
        }

        public bool Set(IElementsHolder t)
        {
            FormattableModifierList formmatable=new FormattableModifierList();
           
            formmatable.Set(t.permanentModifierList.ToList());
            var core=new ElementsHolderCore{
                statusList=t.elementDictionary.Select(ele=>
                {
                    FormattableStatusHolder status=new FormattableStatusHolder();
                    status.Set(ele.Value);
                    return status;
                }).ToList(),
                modifierList=formmatable
                //.ToDictionary(ele=>ele.Item1,ele=>ele.status)
            };
            Data=ZeroFormatterSerializer.Serialize(core);
            DataType=TypeUtils.TypeToString(t.GetType());
            return true;
        }
    }

}