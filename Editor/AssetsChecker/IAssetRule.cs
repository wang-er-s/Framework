namespace Framework.Editor.AssetsChecker
{
    using UnityEditor;

    public interface IAssetRule : IRule
    {
        void Check(AssetImporter assetImporter);
    }
}