/*
* Create by Soso
* Time : 2018-12-11-04 下午
*/
using UnityEngine;
using System;

namespace SF
{
	public class CircleGuideCtrl : GuideCtrl
	{
		/// <summary>
		/// 镂空区域半径
		/// </summary>
		private float mRadius;
		/// <summary>
		/// 当前高亮区域的半径
		/// </summary>
		private float mCurrentRadius = 0f;

		protected override void SetMatShader ()
		{
			mShaderName = "UI/Guide/CircleGuide";
		}

		protected override void InitData ()
		{
			Target.rectTransform.GetWorldCorners ( _corners );
			//获取最终高亮区域半径
			mRadius = Vector2.Distance ( World2CanvasPos ( mCanvas, _corners[ 0 ] ),
			                             World2CanvasPos ( mCanvas, _corners[ 3 ] ) ) / 2f;
			//计算圆心
			float   x           = _corners[ 0 ].x + ( _corners[ 3 ].x - _corners[ 0 ].x ) / 2f;
			float   y           = _corners[ 0 ].y + ( _corners[ 1 ].y - _corners[ 0 ].y ) / 2f;
			Vector3 centerWorld = new Vector3 ( x, y, 0 );
			Vector2 center      = World2CanvasPos ( mCanvas, centerWorld );
			//Apply 设置数据到shader中
			Vector4 centerMat = new Vector4 ( center.x, center.y, 0, 0 );
			mMaterial.SetVector ( "_Center", centerMat );
			//计算当前高亮显示区域半径
			RectTransform canvasRectTransform = mCanvas.transform as RectTransform;
			canvasRectTransform.GetWorldCorners ( _corners );
			foreach ( Vector3 corner in _corners )
			{
				mCurrentRadius = Mathf.Max ( Vector3.Distance ( World2CanvasPos ( mCanvas, corner ), corner ),
				                             mCurrentRadius );
			}

			float initRadius = ShowAnim ? mCurrentRadius : mRadius;
			mMaterial.SetFloat ( "_Slider", initRadius );
		}

		private float shrinkVelocity = 0f;

		protected override void PlayShrinkAnim ()
		{
			if ( !ShowAnim )
				return;
			float value = Mathf.SmoothDamp ( mCurrentRadius, mRadius, ref shrinkVelocity, ShrinkTime );
			if ( !Mathf.Approximately ( value, mCurrentRadius ) )
			{
				mCurrentRadius = value;
				mMaterial.SetFloat ( "_Slider", mCurrentRadius );
			}
		}
	}
}
