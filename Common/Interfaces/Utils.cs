using System;
using System.Collections.Generic;
using System.Reflection;

namespace fidelizPlus_back
{
    public interface Utils
    {
        public string Quote(string s);

        public string FirstToLower(string s);

        public string Join(IEnumerable<string> toJoin, string separator);

        public string ListToMessage(string header, IEnumerable<string> list);

        public int SetBit(int value, int bit, bool valBit);

        public bool GetBit(int value, int bit);

        public IEnumerable<PropertyInfo> GetProps(Type type);

        public IEnumerable<PropertyInfo> GetProps<T>();

        public TTarget Cast<TTarget, TSource>(TSource source, int? id = null) where TTarget : new();
    }
}
