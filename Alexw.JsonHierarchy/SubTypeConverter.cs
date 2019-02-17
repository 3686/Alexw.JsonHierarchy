using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Alexw.JsonHierarchy
{
    public class SubTypeConverter : JsonConverter
    {
        private static readonly BlockingCollection<Type> TypesWithAttributes = new BlockingCollection<Type>();
        private static readonly BlockingCollection<Relationship> Relationships = new BlockingCollection<Relationship>();

        private class Relationship
        {
            public Type OriginalType { get; }
            public Type NewType { get; }
            public string PropertyName { get; }
            public string Value { get; }

            public Relationship(Type originalType, Type newType, string propertyName, string value)
            {
                OriginalType = originalType;
                NewType = newType;
                PropertyName = propertyName;
                Value = value;
            }
        }

        public SubTypeConverter()
        {
            // todo limit to particular assemblies?

            // basic do-once
            if (TypesWithAttributes.Count != 0)
                return;

            // Find types with attributes added
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(SubTypeAttribute), true).Length > 0)
                    {
                        TypesWithAttributes.Add(type);
                    }
                }
            }

            // Extract the SubTypeAttribute attributes
            foreach (var item in TypesWithAttributes)
            {
                foreach (var attribute in item.GetCustomAttributes(true).OfType<SubTypeAttribute>())
                {
                    var r = new Relationship(item, attribute.NewType, attribute.PropertyName, attribute.Value);
                    if (Relationships.Contains(r) == false)
                    {
                        Relationships.Add(r);
                    }
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var bestType = CalculateTypeBasedOnProperties(jObject, objectType);
            var target = existingValue ?? Activator.CreateInstance(bestType);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        private static Type CalculateTypeBasedOnProperties(JObject jObject, Type objectType)
        {
            var bestTypeSoFar = objectType;
            var relationships = Relationships.Where(x => x.OriginalType == objectType).ToArray();
            foreach (var relationship in relationships)
            {
                var propertyOrNull = jObject.Properties().FirstOrDefault(p => p.Name.Eq(relationship.PropertyName));
                if (propertyOrNull != null && propertyOrNull.Values<string>().Any(o => o.Eq(relationship.Value)))
                {
                    return CalculateTypeBasedOnProperties(jObject, relationship.NewType);
                }
            }
            return bestTypeSoFar;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
