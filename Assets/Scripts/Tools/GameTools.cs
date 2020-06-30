using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SthGame
{
    public class GameTools
    {
        public static GameObject AddChild(GameObject parent)
        {
            return AddChild(parent.transform);
        }

        public static GameObject AddChild(Transform parent)
        {
            GameObject obj = new GameObject();
            if (parent != null)
            {
                obj.transform.parent = parent;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
            }
            return obj;
        }

        public static T CreateDeepCopy<T>(T obj)
        {
            T t;
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            t = (T)formatter.Deserialize(memoryStream);
            return t;
        }
    }
}