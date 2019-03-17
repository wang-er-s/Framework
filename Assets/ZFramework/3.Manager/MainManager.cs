/*
* Create by Soso
* Time : 2018-12-11-06 下午
*/
using UnityEngine;
using System;

namespace ZFramework
{

	public enum EnvironmentMode
	{
		Devloping,

		Test,

		Production
	}

	public abstract class MainManager : MonoBehaviour
	{
		public EnvironmentMode mode;

		private static EnvironmentMode sharedMode;

		private static bool isModeSetted = false;

		private void Start ()
		{
			if ( !isModeSetted )
			{
				sharedMode = mode;
				isModeSetted = true;
			}

			switch ( sharedMode )
			{
				case EnvironmentMode.Devloping:
					LaunchInDevelopingMode ();
					break;
				case EnvironmentMode.Test:
					LaunchInTestMode ();
					break;
				case EnvironmentMode.Production:
					LaunchInProductionMode ();
					break;
			}
		}

		protected abstract void LaunchInDevelopingMode ();

		protected abstract void LaunchInTestMode ();

		protected abstract void LaunchInProductionMode ();

	}
}
