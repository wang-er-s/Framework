namespace Framework.Prefs
{
    public interface IFactory
    {
        /// <summary>
        /// Create an instance of the preferences.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Preferences Create(string name);
    }
}
