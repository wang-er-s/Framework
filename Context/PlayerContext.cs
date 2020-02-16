using Framework.Services;

namespace Framework.Context
{
    public class PlayerContext : Context
    {
        public string Username { get; }
        public PlayerContext(string username) : this(username, null)
        {
            this.Username = username;
        }

        public PlayerContext(string username, IServiceContainer container) : base(container, null)
        {
            this.Username = username;
        }
    }
}