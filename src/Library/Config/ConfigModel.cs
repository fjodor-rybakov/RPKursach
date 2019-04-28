namespace Config
{
    public class ConfigModel
    {
        public DefaultConnection DefaultConnection { get; set; }
        public Redis Redis { get; set; }
        public Jwt Jwt { get; set; }
    }
    
    public class DefaultConnection
    {
        public string Database { get; set; }
    }

    public class Redis
    {
        public string Url { get; set; }
        public bool AbortOnConnectFail { get; set; }
    }

    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int Lifetime { get; set; }
    }
}