using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.Modiferty;
using MackySoft.Modiferty.Modifiers;
using System.Linq;
using ZeroFormatter.Formatters;
using ZeroFormatter;

namespace RandomEnchant{
    public enum OperatorType {
            Additive = 0,
            Increase=5,
            Subtractive = 1,
            Decrease=6,
            More = 2,
            Less = 3,
            Set = 4
    }
    public static class Extenders
    {
        public static string ToString(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.Additive:
                    return "+";
                case OperatorType.Subtractive:
                    return "-";
                case OperatorType.More:
                    return "*";
            }
            // other ones, just use the base method
            return operatorType.ToString();
        }
    }
    public class StatusModifier<T> :IStatusModifier<T> 
        where T:Status{
        internal readonly IModifier<int> _modifier;
        readonly OperatorType _type;
        //public IModifier<int> modifier=>_modifier;
        public OperatorType type=>_type;
        public int amount =>_amount;
        readonly int _amount;

        /*public static  StatusModifier<T> Additive(int amount){
            return new StatusModifier<T>(OperatorType.Additive,amount);
        }
        public static StatusModifier<T> Substractive(int amount){
            return new StatusModifier<T>(OperatorType.Subtractive,amount);
        }
        public static StatusModifier<T> More(int amount){
            return new StatusModifier<T>(OperatorType.More,amount);
        }*/
        
        public override string ToString(){
            string status=((Status)Activator.CreateInstance(typeof(T),args:0)).name;
            switch (this._type)
            {
                case OperatorType.Additive:
                    return $"{status} +{_amount}";
                case OperatorType.Subtractive:
                    return $"{status} -{_amount}";
                case OperatorType.More:
                    return $"{status} +{_amount}%";
                default:
                    throw new NotImplementedException();
            }
        }
        IModifier<int> getModifier(OperatorType type,int amount){
            if(amount<0)
                throw new Exception($"amount must be non-negative OperatorType {type},int {amount} ");
            IModifier<int> _mod;
            switch(type){
            case OperatorType.Additive  : _mod=new AdditiveModifierInt((int)amount);((OperatorModifierBase<int>)_mod).Priority=2;break;
            case OperatorType.Increase  : _mod=new AdditiveModifierInt((int)amount);((OperatorModifierBase<int>)_mod).Priority=2;break;
            case OperatorType.Subtractive : _mod=new SubtractiveModifierInt((int)amount);((OperatorModifierBase<int>)_mod).Priority=1;break;
            case OperatorType.Decrease : _mod=new SubtractiveModifierInt((int)amount);((OperatorModifierBase<int>)_mod).Priority=1;break;
            case OperatorType.More : _mod=new MultiplyModifierInt(1+amount/100f);break;
            case OperatorType.Less : _mod=new MultiplyModifierInt(1-amount/100f);break;
            case OperatorType.Set : _mod=new SetModifierInt((int)amount);((OperatorModifierBase<int>)_mod).Priority=10;break;
            default:throw new System.Exception() ;      
            }
            new MultiplyModifierInt(()=>1+amount/100f);
            return _mod;
        }
        public  StatusModifier(OperatorType type,int amount){
            _modifier=getModifier(type,amount);
            this._type = type;
            this._amount = amount;
        }
        public  StatusModifier(StatusModifierCore core):this(core.type,core.amount)
        {
        
        }
        public StatusModifier(Status status,Func<int,int> _Evaluate,OperatorType type ){
            if(type==OperatorType.Additive||type==OperatorType.Increase)
                status.mainmodifier._Evaluate=_Evaluate;
            else if(type==OperatorType.Subtractive||type==OperatorType.Decrease)
                status.mainmodifier._Evaluate=(int value)=>-_Evaluate(value);
            else
                throw new Exception();
            LinkOnChanged=(Status target)=>target.onChangedInvoker+=status.onChangedInvoker;

            _modifier=status.mainmodifier;
    
            this._type=type;            
        }
        Action<Status> LinkOnChanged;  
        public  virtual void apply(IStatusHolder holder)
        {
            //UnityEngine.Debug.Log(this.ToString());
           // holder._modifierList.Add(this);
            (holder.getStatus<T>()).apply(_modifier,type);
            if(LinkOnChanged!=null){
                LinkOnChanged(holder.getStatus<T>());
                LinkOnChanged=null;
            }
           // status.variable.Modifiers.Add(modifier);
            // parameter.variable.Modifiers.Add(modifier);
        }
        /*public  void add(StatusHolder holder)
        {
            holder._modifierList.Add(this);
            holder.savable=true;
           // ((Status)holder.getStatus<T>()).apply(_modifier);
           // status.variable.Modifiers.Add(modifier);
            // parameter.variable.Modifiers.Add(modifier);
        }*/
         public  virtual void remove(IStatusHolder holder)
        {
            (holder.getStatus<T>()).remove(_modifier,type);
            //holder._modifierList.Remove(this);
           // status.variable.Modifiers.Add(modifier);
            // parameter.variable.Modifiers.Add(modifier);
        }
    }
    public class PermanentStatusModifier<T> : StatusModifier<T>
    where T:Status
    {
        /* public static new PermanentStatusModifier<T> Additive(int amount){
            return new PermanentStatusModifier<T>(OperatorType.Additive,amount);
        }
        public static new PermanentStatusModifier<T> Substractive(int amount){
            return new PermanentStatusModifier<T>(OperatorType.Subtractive,amount);
        }
        public static new PermanentStatusModifier<T> More(int amount){
            return new PermanentStatusModifier<T>(OperatorType.More,amount);
        }*/

        public PermanentStatusModifier(StatusModifierCore core) : base(core)
        {
        }

        public PermanentStatusModifier(OperatorType type, int amount) : base(type, amount)
        {
        }

        public override void apply(IStatusHolder holder)
        {
            
            if(holder as ElementsHolder !=null)
                ((ElementsHolder)holder)._modifierList.Add(this);
            else if(holder as StatusHolder!=null)
                ((StatusHolder)holder)._modifierList.Add(this);
            else
                throw new Exception();
            base.apply(holder);
        }

        public override void remove(IStatusHolder holder)
        {
            
            if(holder as StatusHolder!=null)
                ((StatusHolder)holder)._modifierList.Remove(this);
            else if(holder as ElementsHolder !=null)
                ((ElementsHolder)holder)._modifierList.Remove(this);
            else
                throw new Exception();
            base.remove(holder);
        }

     
    }
    public interface IStatusModifier<out T> 
        where T:IStatus{
        void apply(IStatusHolder holder);
       //void add(StatusHolder holder);
        void remove(IStatusHolder holder);
        //IModifier<int> modifier{get;}
        OperatorType type{get;}
        int amount{get;}

    }
 

}