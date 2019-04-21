using System;
using StackExchange.Redis;
using Assets.Redis;

namespace Redis
{
    public class RedisContext
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static RedisContext _instance;
        
        private RedisContext() {}

        public static RedisContext GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = new RedisContext();
            SetConfiguration();

            return _instance;
        }

        private static void SetConfiguration()
        {
            var configurationOptionsTable = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "localhost:6379" }
            };

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptionsTable));
        }
        
        public static ConnectionMultiplexer Connection => _lazyConnection.Value;
        public static IDatabase RedisCache => Connection.GetDatabase();

        public static Message GetMessage(string message)
        {
            if (message.Length == 0) return null;
            var eventName = message.Split(' ')[0];
            var value = message.Substring(eventName.Length + 1);
            return new Message { Event = eventName, Value = value };
        }
    }
}
