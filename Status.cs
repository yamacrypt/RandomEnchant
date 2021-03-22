using System;
using System.Collections.Generic;
using MackySoft.Modiferty;
using Hexat;
using MackySoft.Modiferty.Modifiers;

namespace RandomEnchant{
    //[ZeroFormattable]
    //minimun unit Don't add more parameters
    abstract class OperatableModifiableInt : ModifiableInt, IModifier<int>
    {
        protected OperatableModifiableInt():base()
        {
        }

        protected OperatableModifiableInt(int baseValue) : base(baseValue)
        {
        }
        int m_Priority;
        public  int Priority {
			get => m_Priority;
			set => m_Priority = value;
		}

        public abstract int Evaluate(int value);
    }
    internal class IncreaseModifiableInt : OperatableModifiableInt
    {
 
        RoundingMethod m_RoundingMethod;

		public RoundingMethod RoundingMethod {
			get => m_RoundingMethod;
			set => m_RoundingMethod = value;
		}
        protected int res=0;
        public IncreaseModifiableInt(int baseValue) : base(baseValue)
        {
        }
        public virtual void Add(int add){
            res+=add;
        }
        public virtual void Remove(int add){
            res-=add;
        }

        public override int Evaluate(int value)
        {
           return ((float)value * (1f+(float)res/100f)).RoundToInt(m_RoundingMethod);
        }
    }
    internal class AddModifiableInt :  OperatableModifiableInt
    {

        protected int res=0;
        public AddModifiableInt(int baseValue) : base(baseValue)
        {
        }
         public virtual void Add(int add){
            res+=add;
        }
        public virtual void Remove(int add){
            res-=add;
        }

        public override int Evaluate(int value)
        {
            return value+res;//this.Evaluate();
        }
    }
    internal class MainModifiableInt :  OperatableModifiableInt,IMainModifiableInt
    {
         AddModifiableInt addModifier;
        IncreaseModifiableInt increaseModifier;

        public Func<int,int> _Evaluate{set;private get;}

        public int baseValue {get=>BaseValue;set{BaseValue=value;}}

        //public IModifier<int> modifier => this;

        public MainModifiableInt(int baseValue=0) : base(baseValue)
        {
            addModifier=new AddModifiableInt(0);
            increaseModifier=new IncreaseModifiableInt(0);
            this.AddModifier(addModifier);
            this.AddModifier(increaseModifier);
        }

        public MainModifiableInt()
        {
        }

        public void Add(IModifier<int> modifier,OperatorType operatortype){
            if(operatortype==OperatorType.Additive||operatortype==OperatorType.Subtractive)
                addModifier.Add(modifier.Evaluate(0));
            else if(operatortype==OperatorType.Increase||operatortype==OperatorType.Decrease)
                increaseModifier.Add(modifier.Evaluate(0));
            else if(operatortype==OperatorType.More||operatortype==OperatorType.Less||operatortype==OperatorType.Set)
                this.AddModifier(modifier);
            else 
                throw new Exception();
        }
        public void Remove(IModifier<int> modifier,OperatorType operatortype){
            if(operatortype==OperatorType.Additive||operatortype==OperatorType.Subtractive)
                addModifier.Remove(modifier.Evaluate(0));
            else if(operatortype==OperatorType.Increase||operatortype==OperatorType.Decrease)
                increaseModifier.Remove(modifier.Evaluate(0));
            else if(operatortype==OperatorType.More||operatortype==OperatorType.Less||operatortype==OperatorType.Set)
                this.RemoveModifier(modifier);
            else 
                throw new Exception();
        }

        public override int Evaluate(int value)
        {
            return value+_Evaluate(this.Evaluate());//value+this.Evaluate();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    internal interface IMainModifiableInt:IModifier<int>{
        int baseValue{get;set;}
        void Add(IModifier<int> modifier,OperatorType operatortype);
        void Remove(IModifier<int> modifier,OperatorType operatortype);
        int Evaluate();
        Func<int,int> _Evaluate{set;}
        //IModifier<int> modifier{get;}

    }
    public abstract class DefaultStatus : Status
    {
        public DefaultStatus(int baseValue) : base(baseValue)
        {
            main=new MainModifiableInt(baseValue);
           
        }
        MainModifiableInt main;//= new ModifiableInt(baseValue: 5);
        override internal IMainModifiableInt mainmodifier=>main;
        public abstract override string name{get;}

        public override int baseValue => main.baseValue;

        public override int Evaluate()
        {
            int res=  Math.Min(Math.Max(min() ,main.Evaluate()),max());//cachedValue.Value;
            return res;
        
        }

        internal override void apply(IModifier<int> modifier, OperatorType operatorType)
        {
            main.Add(modifier,operatorType);
            if (onChangedInvoker != null) 
                onChangedInvoker.Invoke(this, EventArgs.Empty);
        }

        internal override void remove(IModifier<int> modifier, OperatorType operatorType)
        {
            main.Remove(modifier,operatorType);
            if (onChangedInvoker != null) 
                onChangedInvoker.Invoke(this,  EventArgs.Empty);
        }
    }
    public  abstract class Status: IStatus{
        //ISecureValue<int> min,max;
        virtual public Func<int> min{internal get=>_min;set{_min=value;}}
        Func<int> _min,_max;
        virtual public Func<int> max{internal get=>_max;set{_max=value;}}
        public Status(int baseValue)
        {
            //main=new MainModifiableInt(baseValue);
            this._min=()=>0;//int.MinValue;
            this._max=()=>int.MaxValue;
            //this.min=SecureValues.Int(min);
            //this.max=SecureValues.Int(max);
        }
        abstract internal IMainModifiableInt mainmodifier{get;}
        //bool isChanged=true;

       // ISecureValue<int> cachedValue=SecureValues.Int(0);
        public  abstract int Evaluate();
       
        public abstract int baseValue{get;}
        public abstract string name{get;}
       // abstract protected string _name{get;}
        
        internal abstract void apply(IModifier<int> modifier,OperatorType operatorType);
        internal abstract void remove(IModifier<int> modifier,OperatorType operatorType);
        public static implicit operator int(Status st){
            return st.Evaluate();
        }

        /*public IModifierList<int> getAll()
{
   return variable.Modifiers;
}*/
        public event EventHandler<EventArgs> onChanged
        {
            add { onChangedInvoker += value;
             }
            remove { onChangedInvoker -= value; }
        }

        // Needed to overcome iOS AOT limitations
        internal EventHandler<EventArgs> onChangedInvoker;
    }
    

    public interface IStatus{
        string name{get;}
        int Evaluate();
        int baseValue{get;}
        Func<int> min{set;}
        Func<int> max{set;}
        //IModifierList<int> getAll();
       // ModifiableInt variable{get;}
        event EventHandler<EventArgs> onChanged;
        //void apply(IModifier<int> modifier);
        //void remove(IModifier<int> modifier);
        //int Evaluate{get;}
        //ModifiableInt variable{get;}
        //void apply(IModifier<int> modifier);
    }
   
    public abstract class StatusHolder:IStatusHolder{
        public bool savable=false;
        //public abstract Status getStatus(Type status);
        readonly Dictionary<Type,IStatus> _statusDictionary;
       public  IList<IStatusModifier<IStatus>> _modifierList=new List<IStatusModifier<IStatus>>();

        public IReadOnlyDictionary<Type, IStatus>  statusDictionary => _statusDictionary;

        public IReadOnlyList<IStatusModifier<IStatus>> permanentModifierList => (IReadOnlyList<IStatusModifier<IStatus>>)this._modifierList;

        public T getStatus<T>()where T:IStatus
        {
            //IStatus value=new IStatus();
            var res=statusDictionary[typeof(T)];

            return (T)res;

        }
        StatusHolder(IReadOnlyList<IStatus> statusList){
            //this._statusDictionary=statusDictionary;
            _statusDictionary=new Dictionary<Type, IStatus>();
            foreach (IStatus item in statusList)
            {
                Type type=item.GetType();
                if(!_statusDictionary.ContainsKey(type))
                    _statusDictionary.Add(type,item);
                else
                    throw new Exception($"key {type} is already included.");
               /* try{
                _statusDictionary.Add(item.GetType(),item);
                }
                catch{
                    throw new ArgumentException("failed to create instance.Make sure that this class has only one int argument");
                }*/
            }
            
           
            //UnityEngine.Debug.Log(modifierList.Count);
        }
        public StatusHolder(IReadOnlyList<IStatus> statusList,IList<IStatusModifier<IStatus>> list=null):this(statusList)
        {
            if(list==null)
                return;
            this._modifierList=list;
            //IReadOnlyList<IStatusModifier<IStatus>> list= (IReadOnlyList<IStatusModifier<IStatus>>)modifierList;
            foreach (var item in this._modifierList)
            {
                item.apply(this);
               
            }
        }
    }
    public interface IStatusHolder{
        IReadOnlyDictionary<Type,IStatus> statusDictionary{get;}
        IReadOnlyList<IStatusModifier<IStatus>> permanentModifierList{get;}
        T getStatus<T>()where T:IStatus;
    }
     public interface InApplicableHolder{
        T getStatus<T>()where T:IStatus;
        T getElement<T>()where T:IStatusHolder;
    }
    /* public interface IReadOnlyModifierApplicable{
        delegate IStatus get<in T>();
        IStatus getStatus<T>()where T:IStatus;
    }*/
   /* public interface IStatusHolder:ModifierApplicable{
        IReadOnlyDictionary<Type,IStatus> statusDictionary{get;}
       // T getStatus<T>()where T:IStatus;
        //IList<IStatusModifier<IStatus>> modifierList{get;}
    } */
    public abstract class ElementsHolder:IElementsHolder,InApplicableHolder{
         Dictionary<Type, IStatusHolder> _elementDictionary;
         Dictionary<Type, IStatus> _statusDictionary;

        ElementsHolder(IReadOnlyList<IStatusHolder> elementsList){
            //this._statusDictionary=statusDictionary;
            _elementDictionary=new Dictionary<Type, IStatusHolder>();
            _statusDictionary=new Dictionary<Type, IStatus>();
            foreach (IStatusHolder item in elementsList)
            {
                Type elementtype=item.GetType();
                if(!_elementDictionary.ContainsKey(elementtype))
                    _elementDictionary.Add(elementtype,item);
                else
                    throw new Exception($"key {elementtype} is already included.");
                foreach (var status in item.statusDictionary.Values)
                {
                    Type statusType=status.GetType();
                    if(!_statusDictionary.ContainsKey(statusType))
                        _statusDictionary.Add(statusType,status);
                    else
                        throw new Exception($"key {statusType} is already included.");
                }
               
               /* try{
                _statusDictionary.Add(item.GetType(),item);
                }
                catch{
                    throw new ArgumentException("failed to create instance.Make sure that this class has only one int argument");
                }*/
            }
            
           
            //UnityEngine.Debug.Log(modifierList.Count);
        }
        public ElementsHolder(IReadOnlyList<IStatusHolder> statusList,IList<IStatusModifier<IStatus>> list=null):this(statusList)
        {
            if(list==null)
                return ;
            this._modifierList=list;
            //IReadOnlyList<IStatusModifier<IStatus>> list= (IReadOnlyList<IStatusModifier<IStatus>>)modifierList;
            foreach (var item in this._modifierList)
            {
                item.apply(this);
               
            }
        }
        internal IList<IStatusModifier<IStatus>> _modifierList=new List<IStatusModifier<IStatus>>();
        //public IReadOnlyList<IStatusModifier<IStatus>> modifierList => (IReadOnlyList<IStatusModifier<IStatus>>)this._modifierList;

        public IReadOnlyDictionary<Type, IStatusHolder> elementDictionary => this._elementDictionary;

        public IReadOnlyDictionary<Type, IStatus> statusDictionary =>this._statusDictionary;

        //only output modifiers added to ElementHolder , not includes modifiers added to any statusHolders which this have.
        public IReadOnlyList<IStatusModifier<IStatus>> permanentModifierList =>  (IReadOnlyList<IStatusModifier<IStatus>>)this._modifierList;

        public T getElement<T>() where T : IStatusHolder
        {
            var res=_elementDictionary[typeof(T)];
            return (T)res;
        }

         public T getStatus<T>()where T:IStatus
        {
            //IStatus value=new IStatus();
            var res=_statusDictionary[typeof(T)];

            return (T)res;

        }
    }
     public interface IElementsHolder:IStatusHolder{
       // IReadOnlyDictionary<Type,IStatus> statusDictionary{get;}
       // T getStatus<T>()where T:IStatus;

        //IReadOnlyList<IStatusModifier<IStatus>> modifierList{get;}
        //T getStatus<T>()where T:IStatus;
        T getElement<T>()where T:IStatusHolder;
        IReadOnlyDictionary<Type,IStatusHolder> elementDictionary{get;}
        //IReadOnlyDictionary<Type,IStatus> statusDictionary{get;}
    }
}
