using Framework.Asynchronous;
using UnityEngine;

namespace Framework.Audio
{
	/// <summary>
	/// 音频资源类
	/// </summary>
	internal class AssetAudio
	{
		private IAsyncResult _assetRef;
		//private AssetOperationHandle _handle;
		private System.Action<AudioClip> _userCallback;

		/// <summary>
		/// 音频层级
		/// </summary>
		public EAudioLayer AudioLayer { private set; get; }

		/// <summary>
		/// 资源对象
		/// </summary>
		public AudioClip Clip { private set; get; }


		public AssetAudio(string location, EAudioLayer audioLayer)
		{
			AudioLayer = audioLayer;
			//_assetRef = new AssetReference(location);
		}
		public void Load(System.Action<AudioClip> callback)
		{
			if (_userCallback != null)
				return;

			_userCallback = callback;
			/*_handle = _assetRef.LoadAssetAsync<AudioClip>();
			_handle.Completed += HandleCompleted;*/
		}
		public void UnLoad()
		{
			if (_assetRef != null)
			{
				//_assetRef.Release();
				_assetRef = null;
			}
			_userCallback = null;
		}
		private void HandleCompleted()
		{
			//Clip = _handle.AssetObject as AudioClip;
			_userCallback?.Invoke(Clip);	
		}
	}
}