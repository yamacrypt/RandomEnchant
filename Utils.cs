using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RandomEnchant{
  public class TypeUtils{
        public static  string TypeToString(Type type){
            //return type.ToString();
            if(!type.IsGenericType)
                return $"{type.ToString().Split('+')[0]}, {type.Assembly.GetName().Name}";
            return "";
            //return $"{type}, {type.GenericTypeArguments.First().Assembly.GetName().Name}";
            throw new Exception();
            //ype.
        }
        public static Type StringToType(string s,bool isGeneric=false){
            if(!isGeneric)
                return Type.GetType(s);
            throw new Exception();
        }
    }
    public static class InterfaceUtils
    {
        /// <summary>
        /// call class内の指定されたインターフェイスが実装されているすべての Type を返します
        /// </summary>
        public static Type[] GetInterfaces<T>()
        {
            /*Type caller=new StackTrace().GetFrame(1).GetMethod().ReflectedType;
            UnityEngine.Debug.Log(caller);
            MethodInfo[] members = caller.GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly);
            UnityEngine.Debug.Log(string.Join(",",members.Select(ele=>ele.GetType())));
            return members.Select(method=>method.GetType()).Where(c=>c.GetInterfaces().Any(t=>t==typeof(T))).ToArray();*/
            return Assembly.GetExecutingAssembly().GetTypes().Where(t=>t.Namespace==typeof(InterfaceUtils).Namespace).Where(c => c.GetInterfaces().Any(t =>t==typeof(T))).ToArray();
        }
    }
    public static class Random
    {
        public static int RangeInt(int a, int b)
        {
            var randomByte = new byte[4];
            using(var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomByte);
            }
            int res=Math.Abs(System.BitConverter.ToInt32(randomByte,0))%(b-a);
            return res+a;
        } 
    }
  }