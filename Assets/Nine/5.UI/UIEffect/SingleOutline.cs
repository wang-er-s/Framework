using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Nine
{
	[AddComponentMenu("UI/Effects/SingleOutline", 16)]
	public class SingleOutline : Outline
	{
		public static Pool<List<UIVertex>> outlinePool =
			new SimpleObjectPool<List<UIVertex>> ( () => new List<UIVertex> (), initCount : 10 );
		protected SingleOutline()
		{}
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!IsActive())
				return;

            List<UIVertex> verts = outlinePool.Spawn();
			vh.GetUIVertexStream(verts);

			var neededCpacity = verts.Count * 2;
			if (verts.Capacity < neededCpacity)
				verts.Capacity = neededCpacity;

			var start = 0;
			var end   = verts.Count;
			ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);

			vh.Clear();
			vh.AddUIVertexTriangleStream(verts);
			outlinePool.DeSpawn(verts);
		}

	}
}