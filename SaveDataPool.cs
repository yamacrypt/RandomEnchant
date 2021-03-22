using System;
using BayatGames.SaveGameFree;
using System.Collections.Generic;
using System.Diagnostics;
using ZeroFormatter;
using System.Linq;
using RandomEnchant;
//using MainGame;
namespace RandomEnchant{
    
    public class SaveDataPool{
        static SaveDataPool instance;
        public static SaveDataPool Instance
        {
            get{
            if(instance==null)
                instance=new SaveDataPool();
            return instance;
            }
        }
        public class AnyAccessible{}
        private Dictionary<string, object> streams = new Dictionary<string, object>();
        class AccessableData{
            public Dictionary<String,(bool savable,bool loadable)> accesessableDictionary;
            public AccessableData(Type[] saver,Type[] loader)
            {   
                
                    accesessableDictionary=new Dictionary<string, (bool savable, bool loadable)>();
                    foreach (Type itemtype in saver)
                    {
                        var item=TypeUtils.TypeToString(itemtype);
                        if(accesessableDictionary.ContainsKey(item)){
                            accesessableDictionary[item]=(true,accesessableDictionary[item].loadable);
                        }
                        else{
                            accesessableDictionary[item]=(true,false);
                        }
                    }
                    foreach (Type itemtype in loader)
                    {
                        var item=TypeUtils.TypeToString(itemtype);
                        if(accesessableDictionary.ContainsKey(item)){
                            accesessableDictionary[item]=(accesessableDictionary[item].savable,true);
                        }
                        else{
                            accesessableDictionary[item]=(false,true);
                        }
                    }
                //this.data = data;
            }
        }
        //T is expected FormattableDataHolder
        public void RegisterAccessRights<T>(string key ,Type[] saver,Type[] loader,Func<T> Init=null){
            if(!streams.ContainsKey(key)){
                streams.Add(key, new AccessableData(saver,loader));
                if(Init!=null){
                   try
                   {
                        InitSave<T>(key,Init());
                   }
                   catch (System.Exception)
                   {
                        InitSaveBasic<T>(key,Init());
                   }
                  
                }
            }
            else
                throw new Exception();
        }
        public void RegisterAccessRights<T>(string key ,Type saver,Type[] loader,Func<T> Init=null){
            RegisterAccessRights<T>(key,new Type[]{saver},loader,Init);
        }
        public void RegisterAccessRights<T>(string key ,Type[] saver,Type loader,Func<T> Init=null){
           RegisterAccessRights<T>(key,saver,new Type[]{loader},Init);
        }
        public void RegisterAccessRights<T>(string key ,Type saver,Type loader,Func<T> Init=null){
            RegisterAccessRights<T>(key,new Type[]{saver},new Type[]{loader},Init);
        }
        bool isAny(AccessableData data,bool isLoad){
            string type=TypeUtils.TypeToString(typeof(AnyAccessible));
            return data.accesessableDictionary.ContainsKey(type)&&
            (isLoad==true
                ?data.accesessableDictionary[type].loadable
                :data.accesessableDictionary[type].savable
            );
        }
        public T Load<T>(string key) {
            
            object obj;
            if (streams.TryGetValue(key, out obj))
            {
                
                Type callertype=new StackTrace().GetFrame(1).GetMethod().ReflectedType.UnderlyingSystemType;
                string caller=TypeUtils.TypeToString(callertype);//UnityEngine.Debug.Log(caller);
                AccessableData accessable=obj as AccessableData;
                //UnityEngine.Debug.Log(isAny(accessable));
                if(isAny(accessable,true)||accessable.accesessableDictionary[caller].loadable){
                    var Data=SaveGame.Load<byte[]>(key);
                    var master=ZeroFormatterSerializer.Deserialize<FormattableMaster>(Data);
                    return master.Load<T>();
                    //return (T)ZeroFormatterSerializer.Deserialize<U>(Data).Load();
                    //T deserializer=new T();
                   // return (T)deserializer.Deserialize(serializedFormattableDataHolder) ;
                }
            }
            throw new KeyNotFoundException();
            //return (deserializer as IFormattableHolder<T>).Load();
            /*deserializer.Deserialize(serializedFormattableDataHolder);
            U formattableDataHolder=ZeroFormatterSerializer.Deserialize<U>(serializedFormattableDataHolder);
            T concreteData=formattableDataHolder.Load();
            return concreteData;*/
        }
        public T LoadBasic<T>(string key) {
            
            object obj;
            if (streams.TryGetValue(key, out obj))
            {
                
                Type callertype=new StackTrace().GetFrame(1).GetMethod().ReflectedType.UnderlyingSystemType;
                string caller=TypeUtils.TypeToString(callertype);//UnityEngine.Debug.Log(caller);
                AccessableData accessable=obj as AccessableData;
                //UnityEngine.Debug.Log(isAny(accessable));
                if(isAny(accessable,true)||accessable.accesessableDictionary[caller].loadable){
                    var Data=SaveGame.Load<byte[]>(key);
                    var res=ZeroFormatterSerializer.Deserialize<T>(Data);
                    return res;
                    //return (T)ZeroFormatterSerializer.Deserialize<U>(Data).Load();
                    //T deserializer=new T();
                   // return (T)deserializer.Deserialize(serializedFormattableDataHolder) ;
                }
            }
            throw new KeyNotFoundException();
            //return (deserializer as IFormattableHolder<T>).Load();
            /*deserializer.Deserialize(serializedFormattableDataHolder);
            U formattableDataHolder=ZeroFormatterSerializer.Deserialize<U>(serializedFormattableDataHolder);
            T concreteData=formattableDataHolder.Load();
            return concreteData;*/
        }
        public bool SaveBasic<T>(string key,T item) {
            if(item==null){
                SaveGame.Delete(key);
                return true;
            }
            object obj;
            if (streams.TryGetValue(key, out obj))
            {
                Type callertype=new StackTrace().GetFrame(1).GetMethod().ReflectedType.UnderlyingSystemType;
                string caller=TypeUtils.TypeToString(callertype);//UnityEngine.Debug.Log(caller);
                AccessableData accessable=obj as AccessableData;
                if(isAny(accessable,false)||accessable.accesessableDictionary[caller].savable){    
                    byte[] data=ZeroFormatterSerializer.Serialize<T>(item);
                    SaveGame.Save<byte[]>(key,data);
                    return true;
                }
            }
            throw new KeyNotFoundException();
          // var serializedFormattableDataHolder=ZeroFormatterSerializer.Serialize<U>(formattableDataHolder);
           //SaveGame.Save<byte[]>(key,serializedFormattableDataHolder);
        }
        public bool Save<T>(string key,T item) {
            if(item==null){
                SaveGame.Delete(key);
                return true;
            }
            object obj;
            if (streams.TryGetValue(key, out obj))
            {
                Type callertype=new StackTrace().GetFrame(1).GetMethod().ReflectedType.UnderlyingSystemType;
                string caller=TypeUtils.TypeToString(callertype);//Type.GetType(caller.ToString().Split('+')[0]);
                //UnityEngine.Debug.Log(caller);
                AccessableData accessable=obj as AccessableData;
                if(isAny(accessable,false)||accessable.accesessableDictionary[caller].savable){
                    FormattableMaster master=new FormattableMaster();
                    master.Set<T>(item);
                    //UnityEngine.Debug.Log(master.FormattableName);
                    byte[] data=ZeroFormatterSerializer.Serialize<FormattableMaster>(master);
                    SaveGame.Save<byte[]>(key,data);
                    return true;
                }
            }
            throw new KeyNotFoundException();
          // var serializedFormattableDataHolder=ZeroFormatterSerializer.Serialize<U>(formattableDataHolder);
           //SaveGame.Save<byte[]>(key,serializedFormattableDataHolder);
        }
        bool InitSave<T>(string key,T item) {
            object obj;
            if(SaveGame.Exists(key))
                return true;
            if (streams.TryGetValue(key, out obj))
            {
                //Type caller=new StackTrace().GetFrame(1).GetMethod().ReflectedType;
                UnityEngine.Debug.Log("InitSave");
                    FormattableMaster master=new FormattableMaster();
                    master.Set<T>(item);
                    //UnityEngine.Debug.Log(master.FormattableName);
                    byte[] data=ZeroFormatterSerializer.Serialize<FormattableMaster>(master);
                    SaveGame.Save<byte[]>(key,data);
                   
                
            }
             return true;
          // var serializedFormattableDataHolder=ZeroFormatterSerializer.Serialize<U>(formattableDataHolder);
           //SaveGame.Save<byte[]>(key,serializedFormattableDataHolder);
        }
        bool InitSaveBasic<T>(string key,T item) {
            object obj;
            if(SaveGame.Exists(key))
                return true;
            if (streams.TryGetValue(key, out obj))
            {
                UnityEngine.Debug.Log("InitSaveBasic");
                    //UnityEngine.Debug.Log(master.FormattableName);
                    byte[] data=ZeroFormatterSerializer.Serialize<T>(item);
                    SaveGame.Save<byte[]>(key,data);
                   
                
            }
             return true;
          // var serializedFormattableDataHolder=ZeroFormatterSerializer.Serialize<U>(formattableDataHolder);
           //SaveGame.Save<byte[]>(key,serializedFormattableDataHolder);
        }
    }
}