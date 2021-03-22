using System;
using Hexat;
using MackySoft.Modiferty;

namespace RandomEnchant{
    class StateAddModifableInt : AddModifiableInt
    {
        internal Func<int> min{get;set;}
     internal Func<int> max{get;set;}

        public StateAddModifableInt(int baseValue) : base(baseValue)
        {
            this.min=()=>0;//int.MinValue;
            this.max=()=>int.MaxValue;
        }

        public override void Add(int add)
        {
            base.Add(add);
            res=Math.Min(Math.Max(min() ,res),max());
        }

        public override int Evaluate(int value)
        {
            return base.Evaluate(value);
        }

 

        public override void Remove(int add)
        {
            base.Remove(add);
           res=Math.Min(Math.Max(min() ,res),max());
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    class StateIncreaseModifiableInt : IncreaseModifiableInt
    {
       
     
        internal Func<int> min{get;set;}
     internal Func<int> max{get;set;}
        public StateIncreaseModifiableInt(int baseValue) : base(baseValue)
        {
          this.min=()=>0;//int.MinValue;
            this.max=()=>int.MaxValue;
        }

        public override void Add(int add)
        {
            base.Add(add);
           res=Math.Min(Math.Max(min() ,res),max());
        }
        public override int Evaluate(int value)
        {
            return base.Evaluate(value);
        }

        public override void Remove(int add)
        {
            base.Remove(add);
            res=Math.Min(Math.Max(min() ,res),max());
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    class StateMainModifiableInt :  OperatableModifiableInt,IMainModifiableInt
    {
        StateAddModifableInt addModifier;
        StateIncreaseModifiableInt increaseModifier;

        public Func<int,int> _Evaluate{set;private get;}
        public int baseValue {get=>BaseValue;set{BaseValue=value;}}


        public IModifier<int> modifier => this.modifier;
        internal Func<int> min{private get=>this._min;
            set{
                this._min=value;
                addModifier.min=value;
                increaseModifier.min=value;
            }
        }
        internal Func<int> max{private get=>this._max;set{this._max=value;addModifier.max=value;increaseModifier.max=value;}}
        Func<int> _min,_max;
        public StateMainModifiableInt(int baseValue=0) : base(baseValue)
        {
            addModifier=new  StateAddModifableInt(0);
            increaseModifier=new StateIncreaseModifiableInt(0);
            this.min=()=>0;//int.MinValue;
            this.max=()=>int.MaxValue;
            this.AddModifier(addModifier);
            this.AddModifier(increaseModifier);
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

    }
    public abstract class StateStatus : Status
    {
        StateMainModifiableInt main;
        public StateStatus(int baseValue) : base(baseValue)
        {
           main=new StateMainModifiableInt();
        }

        public override int baseValue => main.baseValue;
        public override Func<int> min { 
            set{
                base.min=value;
                main.min=value;
                }
            }
        public override Func<int> max {
             set{
                 base.max=value;
                 main.max=value;
                 }
            }

        public abstract override string name{get;}

        internal override IMainModifiableInt mainmodifier => main;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int Evaluate()
        {
           return main.Evaluate();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
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
}