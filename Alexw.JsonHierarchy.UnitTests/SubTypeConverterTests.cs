using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Alexw.JsonHierarchy.UnitTests
{
    public class SubTypeConverterTests
    {
        [Test]
        public void Test1()
        {
            var json = "{\"id\":\"order-1\",\"items\":[{\"id\":\"0\",\"type\":\"product\",\"productValue\":\"hello-from-product\"},{\"id\":\"1\",\"type\":\"relation\",\"relationValue\":\"hello-from-relation-1\"},{\"id\":\"2\",\"type\":\"relation\",\"relationValue\":\"hello-from-relation-2\",\"relation\":\"customRelation\",\"value\":\"hello-from-custom-relation-1\"}]}";

            var result = JsonConvert.DeserializeObject<Order>(json);

            Assert.That(result, Is.TypeOf<Order>());
            Assert.That(result.Items.Count, Is.EqualTo(3));
            Assert.That(result.Items.ElementAt(0), Is.TypeOf<ProductItem>());
            Assert.That(result.Items.ElementAt(1), Is.TypeOf<RelationItem>());
            Assert.That(result.Items.ElementAt(2), Is.TypeOf<CustomRelationItem>());
        }

        public class Order
        {
            public ICollection<Item> Items { get; set; }
        }

        [JsonConverter(typeof(SubTypeConverter))]
        [SubType(typeof(ProductItem), "type", "product")]
        [SubType(typeof(RelationItem), "type", "relation")]
        public class Item
        {
            public string Id { get; set; }
            public string Type { get; set; }
        }

        public class ProductItem : Item
        {
            public string ProductValue { get; set; }
        }

        [SubType(typeof(CustomRelationItem), "relation", "customRelation")]
        public class RelationItem : Item
        {
            public string RelationValue { get; set; }
            public string Relation { get; set; }
        }

        public class CustomRelationItem : RelationItem
        {
            public string Value { get; set; }
        }
    }
}