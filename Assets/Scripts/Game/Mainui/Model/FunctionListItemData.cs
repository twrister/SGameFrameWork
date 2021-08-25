using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;
using UnityEngine.Events;

namespace SthGame
{
    public class FunctionListItemData
    {
        public string Desc { get; private set; }
        public EFunctionItemType eFunctionItemType { get; private set; }
        public UnityAction Callback { get; private set; }
        public FunctionListItemData(string desc, UnityAction callback = null, EFunctionItemType itemType = EFunctionItemType.normal, ERedPointType redPoint = ERedPointType.None)
        {
            Desc = desc;
            Callback = callback;
            eFunctionItemType = itemType;
            redPointType = redPoint;
        }
        public ERedPointType redPointType { get; private set; }
    }

    public enum EFunctionItemType
    {
        normal,
        colorPlate,
    }
}
