﻿# Alexw.JsonHierarchy
Enables class hierarchy deserialization using attributes for Newtonsoft.Json
This was enspired by [JsonSubTypes](https://github.com/manuc66/JsonSubTypes) which didn't support multiple inheritance using different properties.

## Usage

1. Add the converter attribute (so the logic kicks in)
2. Let the convert know which field and values resolve types

If you've not yet declared a property and value, it'll default to it's base type.
We purposely support inheritance.

## Example

```csharp
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
```

Given the following JSON:

```
{
  "id": "order-1",
  "items": [{
      "id": "0",
      "type": "product",
      "productValue": "hello-from-product"
    },
    {
      "id": "1",
      "type": "relation",
      "relationValue": "hello-from-relation-1"
    },
    {
      "id": "2",
      "type": "relation",
      "relationValue": "hello-from-relation-2",
      "relation": "customRelation",
      "value": "hello-from-custom-relation-1"
    }
  ]
}
```

The converter returns the following:

```
[Test]
public void Test1()
{
    var json = LoadJson(); // example json

    var result = JsonConvert.DeserializeObject<Order>(json);

    Assert.That(result, Is.TypeOf<Order>());
    Assert.That(result.Items.Count, Is.EqualTo(3));
    Assert.That(result.Items.ElementAt(0), Is.TypeOf<ProductItem>());
    Assert.That(result.Items.ElementAt(1), Is.TypeOf<RelationItem>());
    Assert.That(result.Items.ElementAt(2), Is.TypeOf<CustomRelationItem>());
}
```