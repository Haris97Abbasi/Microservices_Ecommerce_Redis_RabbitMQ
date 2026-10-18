using Catalog.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Catalog.Infrastructure.Persistence;

public static class ProductMapping
{
    public static void Register()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Product)))
            return;

        BsonClassMap.RegisterClassMap<Product>(cm =>
        {
            cm.AutoMap();

            cm.MapIdMember(p => p.Id)
              .SetIdGenerator(StringObjectIdGenerator.Instance)
              .SetSerializer(new StringSerializer(BsonType.ObjectId));

            cm.MapMember(p => p.Price).SetSerializer(new DecimalSerializer(BsonType.Decimal128));
        });
    }
}
