using Framework.Prefs;

namespace Framework.Context
{
    public class ApplicationContext : Context
    {
        /// <summary>
        /// Retrieve a global preferences.
        /// </summary>
        /// <returns></returns>
        public virtual Preferences GetGlobalPreferences()
        {
            return Preferences.GetGlobalPreferences();
        }

        /// <summary>
        /// Retrieve a user's preferences.
        /// </summary>
        /// <param name="name">The name of the preferences to retrieve.eg:username or username@zone</param>
        /// <returns></returns>
        public virtual Preferences GetUserPreferences(string name)
        {
            return Preferences.GetPreferences(name);
        }
    }
}