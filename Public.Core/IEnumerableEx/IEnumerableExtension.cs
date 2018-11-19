using System;
using System.Collections.Generic;

namespace Public.Core.IEnumerableEx
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 自定义扩展Foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }
        }
    }
}
