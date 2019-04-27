using Assets.User;

namespace Redis
{
    public static class RedisEvents
    {
        public static class Events
        {
            public static readonly string ChannelName = "Events";
            
            // User
            public static readonly string RegistrationUserEvent = "__registrationUser__";
            public static readonly string UserCreated = "__userCreated__";
            public static readonly string UserCreatedError = "__userCreatedError__";
        }
    }
}