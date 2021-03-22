

namespace RandomEnchant{
    public interface IFigere<out T>   where T:IStatus{
        IStatusModifier<T> modifier{get;}
        FigereTier tier{get;}
        string text{get;}
    }
    public interface ISuffix<out T>:IFigere<T>   where T:IStatus{
    }
    public interface IPrefix<out T>:IFigere<T>   where T:IStatus{
    }
    public class Suffix<T> : Figere<T>, ISuffix<T> where T : Status
    {
        public Suffix(StatusModifierCore core, FigereTier tier) : base(core, tier)
        {
        }

        public Suffix(OperatorType type, int amount, FigereTier tier) : base(type, amount, tier)
        {
        }
    }
    public class Prefix<T> : Figere<T>, IPrefix<T> where T : Status
    {
        public Prefix(StatusModifierCore core, FigereTier tier) : base(core, tier)
        {
        }

        public Prefix(OperatorType type, int amount, FigereTier tier) : base(type, amount, tier)
        {
        }
    }
    /*public class Suffix:Suffix<Status> 
    {
        public Suffix(StatusModifier<Status> modifier,int frequency) : base(modifier,frequency)
        {
        }
    }*/
    public  abstract class Figere<T>:IFigere<T> where T:Status{
    internal readonly StatusModifier<T> _modifier;
    //public int frequency{get; }
    FigereTier _tier;

    public IStatusModifier<T> modifier => _modifier;

    public string text => _modifier.ToString();

    public FigereTier tier => _tier;

        public Figere(OperatorType type,int amount,FigereTier tier)
        {
            this._modifier = new StatusModifier<T>(type,amount);
            this._tier=tier;
           // this.frequency = frequency;
        }
        public Figere(StatusModifierCore core,FigereTier tier): this(core.type,core.amount,tier)
        {

        }


    }
}