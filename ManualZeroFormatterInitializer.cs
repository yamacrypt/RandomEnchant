namespace ZeroFormatter
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::ZeroFormatter.Formatters;
    using global::ZeroFormatter.Internal;
    using global::ZeroFormatter.Segments;
    using global::ZeroFormatter.Comparers;
    using global::RandomEnchant;
    using UnityEngine;

    public static partial class ZeroFormatterInitializer
    {
        static bool manualregistered = false;
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        
        public static void ManualRegister()
        {
            if(manualregistered)
                return ;
            manualregistered=true;
            Formatter.AppendFormatterResolver(t =>
            {
                
                /*if (t.IsSubclassOf(typeof(global::RandomEnchant.Status))||t==typeof(Status))
                {
                    //Type first = typeof(global::RandomEnchant.StatusModifierFormatter<,>).GetGenericArguments().First();//.MakeGenericType(t.GetGenericArguments().StartsWith());        
                    var formatter=typeof(global::RandomEnchant.StatusFormatter<,>).MakeGenericType(typeof(Formatters.DefaultResolver),t);
                    //Debug.Log(formatter.ToString());
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(IStatus))
                {
                    var formatter=typeof(global::RandomEnchant.IStatusFormatter<>).MakeGenericType(typeof(Formatters.DefaultResolver));
                
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(IList<IStatus>))
                {
                    var formatter=typeof(global::RandomEnchant.IStatusFormatter<>).MakeGenericType(typeof(Formatters.DefaultResolver));
                
                    return Activator.CreateInstance(formatter);
                }
                else if (t.IsSubclassOf(typeof(global::RandomEnchant.StatusHolder))||t==typeof(StatusHolder))
                {
                    //Type first = typeof(global::RandomEnchant.StatusModifierFormatter<,>).GetGenericArguments().First();//.MakeGenericType(t.GetGenericArguments().StartsWith());        
                    var formatter=typeof(global::RandomEnchant.StatusHolderFormatter<,>).MakeGenericType(typeof(Formatters.DefaultResolver),t);
                    //Debug.Log(formatter.ToString());
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(IStatusHolder))
                {
                    var formatter=typeof(global::RandomEnchant.IStatusHolderFormatter<>).MakeGenericType(typeof(Formatters.DefaultResolver));
                
                    return Activator.CreateInstance(formatter);
                }
                else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(global::RandomEnchant.StatusModifier<>))
                {
                    //Type first = typeof(global::RandomEnchant.StatusModifierFormatter<,>).GetGenericArguments().First();//.MakeGenericType(t.GetGenericArguments().StartsWith());        
                    //Debug.Log(first);
                    var formatter=typeof(global::RandomEnchant.StatusModifierFormatter<,>).MakeGenericType(t.GetGenericArguments().StartsWith(typeof(Formatters.DefaultResolver)));
                    //Debug.Log(formatter.ToString());
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(IStatusModifier<IStatus>))
                {
                    var formatter=typeof(global::RandomEnchant.IListFormatter<,>).MakeGenericType(t.GetGenericArguments().StartsWith(typeof(Formatters.DefaultResolver)));
                
                    return Activator.CreateInstance(formatter);
                }
                
                else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(global::RandomEnchant.Suffix<>))
                {
                    //Type first = typeof(global::RandomEnchant.StatusModifierFormatter<,>).GetGenericArguments().First();//.MakeGenericType(t.GetGenericArguments().StartsWith());        
                    //Debug.Log(first);
                    var formatter=typeof(global::RandomEnchant.SuffixFormatter<,>).MakeGenericType(t.GetGenericArguments().StartsWith(typeof(Formatters.DefaultResolver)));
                    //Debug.Log(formatter.ToString());
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(ISuffix<IStatus>))
                {
                    var formatter=typeof(global::RandomEnchant.ISuffixFormatter<>).MakeGenericType(typeof(Formatters.DefaultResolver));
                
                    return Activator.CreateInstance(formatter);
                }
                else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(global::RandomEnchant.Prefix<>))
                {
                    //Type first = typeof(global::RandomEnchant.StatusModifierFormatter<,>).GetGenericArguments().First();//.MakeGenericType(t.GetGenericArguments().StartsWith());        
                    //Debug.Log(first);
                    var formatter=typeof(global::RandomEnchant.PrefixFormatter<,>).MakeGenericType(t.GetGenericArguments().StartsWith(typeof(Formatters.DefaultResolver)));
                    //Debug.Log(formatter.ToString());
                    return Activator.CreateInstance(formatter);
                }
                else if(t==typeof(IPrefix<IStatus>))
                {
                    var formatter=typeof(global::RandomEnchant.IPrefixFormatter<>).MakeGenericType(typeof(Formatters.DefaultResolver));
                
                    return Activator.CreateInstance(formatter);
                }*/


                return null; // fallback to the next resolver
            });
            
        }
        public static T[] StartsWith<T>(this T[] array, T firstValue)
        {
            var dest = new T[array.Length + 1];
            dest[0] = firstValue;
            Array.Copy(array, 0, dest, 1, array.Length);
            return dest;
        }
    }
}