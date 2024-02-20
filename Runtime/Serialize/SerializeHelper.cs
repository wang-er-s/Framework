using System.IO;
using System;
#if MongoDb
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
#endif
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace Framework
{
    public static class SerializeHelper
    {
        public static T Deserialize<T>(byte[] bytes, int index = 0, int count = -1)
        {
            return (T)Deserialize(typeof(T), bytes, index, count);
        }
        
        public static object Deserialize(Type type, byte[] bytes, int index = 0, int count = -1)
        {
#if MongoDb
            if (count == -1) count = bytes.Length;
            using MemoryStream memoryStream = new MemoryStream(bytes,index, count);
            return BsonSerializer.Deserialize(memoryStream, type);
#endif
            return NTDeserialize(type, bytes, index, count);
        }

        public static object Deserialize(Type type, Stream stream)
        {
#if MongoDb
            return BsonSerializer.Deserialize(stream, type);
#endif
            return NTDeserialize(type, stream);
        }

        public static object NTDeserialize(Type type, string json)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static object NTDeserialize(Type type, byte[] bytes, int index = 0, int count = -1)
        {
            JsonSerializer serializer = new JsonSerializer();
            using MemoryStream memoryStream = new MemoryStream(bytes,index, count);
            using StreamReader reader = new StreamReader(memoryStream);
            return serializer.Deserialize(reader, type);
        }

        public static object NTDeserialize(Type type, Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            using StreamReader reader = new StreamReader(stream);
            return serializer.Deserialize(reader, type);
        }

        public static object Deserialize(Type type, string json)
        {
#if MongoDb
            return BsonSerializer.Deserialize(json, type);
#endif
            return NTDeserialize(type, json);
        }

        public static T Deserialize<T>(string json)
        {
            return (T)Deserialize(typeof(T), json);
        }

        public static T DeserializeNT<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        
        public static byte[] Serialize(object message)
        {
            return message.ToBson();
        }

        public static void Serialize(object message, Stream stream)
        {
#if MongoDb
            using BsonBinaryWriter bsonWriter = new BsonBinaryWriter(stream, BsonBinaryWriterSettings.Defaults);
            BsonSerializationContext context = BsonSerializationContext.CreateRoot(bsonWriter);
            BsonSerializationArgs args = default;
            args.NominalType = typeof (object);
            IBsonSerializer serializer = BsonSerializer.LookupSerializer(args.NominalType);
            serializer.Serialize(context, args, message);
#endif
            NTSerialize(message, stream);
        }

        public static byte[] ToBson(this object obj)
        {
#if MongoDb
            return BsonExtensionMethods.ToBson(obj);
#endif
            MemoryStream stream = new MemoryStream();
            Serialize(obj, stream);
            return stream.GetBuffer();
        }
        
        public static string ToJson(this object obj)
        {
#if MongoDb
            return BsonExtensionMethods.ToJson(obj);
#endif
            return obj.ToNTJson();
        }

        /// <summary>
        /// 使用Newtonsoft.json，优点不需要增加额外属性就可以序列化字典
        /// 缺点是慢，特殊情况使用
        /// </summary>
        public static string ToNTJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static void NTSerialize(object message, Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonWriter jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, message);
                jsonWriter.Flush();
                // 将Stream的位置重置到起始位置
                stream.Position = 0; 
            }
        }
    }
}