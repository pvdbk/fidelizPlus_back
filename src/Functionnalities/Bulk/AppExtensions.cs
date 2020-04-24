using System;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace fidelizPlus_back
{
    public static class AppExtensions
    {
        private static readonly FiltersHandler filtersHandler = new FiltersHandler();

        private static readonly Type[] atomicTypes = new Type[] {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(string),
            typeof(DateTime),
            typeof(DateTime?)
        };

        public static string Quote(this string s) => s == null
            ? null
            : "\"" + s + "\"";

        public static string FirstToLower(this string s) => s == "" || s == null
            ? s
            : s.Substring(0, 1).ToLower() + s.Substring(1);

        public static string ToJson(this object o) => JsonSerializer.Serialize(o);

        public static byte[] ToBytes(this string s) => Encoding.UTF8.GetBytes(s);

        public static string BytesToString(this byte[] b) => Encoding.UTF8.GetString(b);

        public static int ToInt(this string s)
        {
            int ret = 0;
            try
            {
                ret = Int32.Parse(s);
            }
            catch
            { }
            return ret;
        }

        public static void ForEach<T>(this IEnumerable<T> toHandle, Action<T> callback)
        {
            foreach (T item in toHandle)
            {
                callback(item);
            }
        }

        public static TOut Reduce<TIn, TOut>(this IEnumerable<TIn> toHandle, Func<TOut, TIn, TOut> callback, TOut init)
        {
            TOut ret = init;
            foreach (TIn item in toHandle)
            {
                ret = callback(ret, item);
            }
            return ret;
        }

        public static int SetBit(this int value, int bitPosition, bool valBit)
        {
            int bitMask = 1 << bitPosition;
            return valBit ? value | bitMask : value & ~bitMask;
        }

        public static bool GetBit(this int value, int bitPosition) =>
            (value & (1 << bitPosition)) != 0;

        public static IEnumerable<PropertyInfo> GetProps(this Type type) =>
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public static IEnumerable<PropertyInfo> GetAtomicProps(this Type type) => type.GetProps()
            .Where(prop => atomicTypes.Contains(prop.PropertyType));

        public static T CastAs<T>(this object source) where T : new()
        {
            T ret = new T();
            var commonProps = new List<(PropertyInfo, PropertyInfo)>();
            IEnumerable<PropertyInfo> targetProps = typeof(T).GetProps();
            IEnumerable<PropertyInfo> sourceProps = source.GetType().GetProps();
            targetProps.ForEach(targetProp =>
            {
                PropertyInfo sourceProp = sourceProps
                    .Where(prop => prop.Name == targetProp.Name && prop.PropertyType == targetProp.PropertyType)
                    .FirstOrDefault();
                if (sourceProp != null)
                {
                    commonProps.Add((targetProp, sourceProp));
                }
            });
            commonProps.ForEach(props =>
            {
                (PropertyInfo targetProp, PropertyInfo sourceProp) = props;
                targetProp.SetValue(ret, sourceProp.GetValue(source));
            });
            return ret;
        }

        public static Func<T, bool> ToTest<T>(this string filter) where T : class
        {
            try
            {
                return filter == null
                    ? x => true
                    : filtersHandler.TreeToTest(
                        new Tree(filter),
                        typeof(T)
                    );
            }
            catch (AppException e)
            {
                throw new AppException(e.Content, 400);
            }
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder) =>
            builder.UseMiddleware<ExceptionHandler>();

        public static IApplicationBuilder UsePaymentHandler(this IApplicationBuilder builder) =>
            builder.UseMiddleware<Payment.PaymentHandler>();

        public static IApplicationBuilder UseSessionHandler(this IApplicationBuilder builder) =>
            builder.UseMiddleware<Identification.SessionHandler>();
    }
}
