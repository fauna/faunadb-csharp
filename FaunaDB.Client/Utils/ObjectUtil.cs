using System;
using System.Collections;
using System.Collections.Generic;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json.Serialization;

namespace FaunaDB.Utils
{
    static class ObjectUtil
    {
        public static Expr ToExpr(object obj) =>
            ToExpr(new DefaultContractResolver(), obj);

        static Expr ToExpr(IContractResolver resolver, object obj)
        {
            if (obj == null) return NullV.Instance;

            var type = obj.GetType();

            if (typeof(Expr).IsAssignableFrom(type)) return (Expr)obj;

            var contract = resolver.ResolveContract(type);

            if (contract is JsonPrimitiveContract)
            {
                if (typeof(string) == type) return (string)obj;
                if (typeof(long) == type) return (long)obj;
                if (typeof(int) == type) return (int)obj;
                if (typeof(double) == type) return (double)obj;
                if (typeof(float) == type) return (float)obj;
                if (typeof(bool) == type) return (bool)obj;
                if (typeof(DateTime) == type)
                {
                    var date = (DateTime)obj;

                    if (date.Ticks % (24 * 60 * 60 * 10000) > 0)
                        return new TimeV(date);

                    return new DateV(date);
                }
            }

            if (contract is JsonObjectContract)
            {
                var fields = new Dictionary<string, Expr>();

                foreach (var property in ((JsonObjectContract)contract).Properties)
                {
                    var value = property.ValueProvider.GetValue(obj);
                    fields.Add(property.PropertyName, ToExpr(resolver, value));
                }

                return ToExpr(fields);
            }

            if (contract is JsonDictionaryContract)
            {
                var fields = new Dictionary<string, Expr>();

                foreach (DictionaryEntry entry in (IDictionary)obj)
                    fields.Add(entry.Key.ToString(), ToExpr(resolver, entry.Value));

                return UnescapedObject.With("object", new UnescapedObject(fields));
            }

            if (contract is JsonArrayContract)
            {
                var values = new List<Expr>();

                foreach (var v in (IEnumerable)obj)
                    values.Add(ToExpr(resolver, v));

                return new UnescapedArray(values);
            }

            return NullV.Instance;
        }
    }
}
