namespace Framework.Prefs
{
    /// <summary>
    /// A interface for encoding and decoding preference data.
    /// </summary>
    public interface IEncryptor
    {

        byte[] Encode(byte[] plainData);

        byte[] Decode(byte[] cipherData);
    }
}
