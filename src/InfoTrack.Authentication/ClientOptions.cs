namespace InfoTrack.Authentication
{
    public class ClientOptions
    {
        public int DefaultCacheExpiry { get; set; }

        public static ClientOptions Default = new ClientOptions
        {
            DefaultCacheExpiry = 86400
        };
    }
}
