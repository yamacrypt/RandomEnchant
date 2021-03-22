namespace RandomEnchant{
    public abstract class CalcStatus : DefaultStatus
    {
        
        public CalcStatus(int baseValue) : base(baseValue)
        {
        }
        public int newBaseValue{set{mainmodifier.baseValue=value;}}
        public int Evaluate(int baseValue){
             mainmodifier.baseValue=baseValue;
             return mainmodifier.Evaluate();
        }

      public override abstract string name{get;}
    }
}