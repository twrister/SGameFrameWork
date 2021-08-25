using System;
using System.Collections;
using System.Collections.Generic;

namespace SthGame
{
    public static class RedPointParentData
    {
        static List<Tuple<ERedPointType, ERedPointType>> _tupleList = null;
        public static List<Tuple<ERedPointType, ERedPointType>> TupleList
        {
            get
            {
                if (_tupleList == null)
                {
                    _tupleList = new List<Tuple<ERedPointType, ERedPointType>>();
                    PrepareData();
                }
                return _tupleList;
            }
        }

        static void AddRedPointParentData(ERedPointType type, ERedPointType parentType)
        {
            _tupleList.Add(new Tuple<ERedPointType, ERedPointType>(type, parentType));
        }

        /// <summary>
        /// 红点父子关系在这添加
        /// </summary>
        static void PrepareData()
        {
            AddRedPointParentData(ERedPointType.RedPointDemo, ERedPointType.MainEntrance);
            AddRedPointParentData(ERedPointType.RedPointDemoSub1, ERedPointType.RedPointDemo);
            AddRedPointParentData(ERedPointType.RedPointDemoSub2, ERedPointType.RedPointDemo);
            AddRedPointParentData(ERedPointType.RedPointDemoSub3, ERedPointType.RedPointDemo);
        }

        /// <summary>
        /// 获得所有红点子类型
        /// </summary>
        /// <returns></returns>
        public static List<ERedPointType> GetRedPointSubTypes(ERedPointType redPointType)
        {
            List<ERedPointType> result = null;

            for (int i = 0; i < TupleList.Count; i++)
            {
                var tuple = TupleList[i];
                if (tuple.Item2 == redPointType)
                {
                    if (result == null) result = new List<ERedPointType>();
                    result.Add(tuple.Item1);
                }
            }

            return result;
        }

        /// <summary>
        /// 获得红点父类型
        /// </summary>
        /// <param name="redPointType"></param>
        /// <returns></returns>
        public static ERedPointType GetRedPointParentType(ERedPointType redPointType)
        {
            ERedPointType result = ERedPointType.None;

            for (int i = 0; i < TupleList.Count; i++)
            {
                var tuple = TupleList[i];
                if (tuple.Item1 == redPointType)
                {
                    result = tuple.Item2;
                    break;
                }
            }

            return result;
        }
    }
}
