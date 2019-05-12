namespace Redis
{
    public static class RedisEvents
    {
        public static class Events
        {
            public const string ChannelName = "Events";

            // User
            public const string LoginUserEvent = "__loginUser__";
            
            // Products
            public const string AddProductEvent = "__addProduct__";
            public const string UpdateProductEvent = "__updateProduct__";
        }
    }
}