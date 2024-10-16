using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonIgnoreIfEmptyAttribute : Attribute { }

    public class ConditionalJsonIgnoreContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization
        )
        {
            var property = base.CreateProperty(member, memberSerialization);

            var jsonIgnoreAttr = member.GetCustomAttribute<JsonIgnoreIfEmptyAttribute>();
            if (jsonIgnoreAttr != null)
            {
                property.ShouldSerialize = obj =>
                {
                    if (obj == null)
                        return false; // Ensure obj is not null

                    var value = property.ValueProvider?.GetValue(obj);
                    return !IsEmpty(value);
                };
            }

            return property;
        }

        private static bool IsEmpty(object? value)
        {
            if (value == null)
                return true;

            if (value is string str)
                return string.IsNullOrEmpty(str);

            if (value is ICollection collection)
                return collection.Count == 0;

            // Add more checks if needed (e.g., arrays)
            return false;
        }
    }
}
