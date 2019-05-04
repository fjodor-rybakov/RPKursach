namespace Redis
{
    public static class RedisEvents
    {
        public static class Events
        {
            public const string ChannelName = "Events";

            // User
            public const string LoginUserEvent = "__loginUser__";
            public const string GetImagesEvent = "__getImages__";
        }
    }
}