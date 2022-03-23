namespace Framework.Editor.AssetsChecker
{
    using UnityEditor;

    public interface IAssetRule<T,A> : IRule<T> where T : CheckerCollection where A : AssetImporter
    {
        void Check(A assetImporter);
    }
}