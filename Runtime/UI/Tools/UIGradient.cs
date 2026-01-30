using UnityEngine;
using UnityEngine.UI;

namespace EasyGameFramework.Essentials
{
	[AddComponentMenu("UI/Effects/Gradient")]
	public class UIGradient : BaseMeshEffect
	{
		public Color Color1 = Color.white;
		public Color Color2 = Color.white;

		[Range(-180f, 180f)] public float Angle = -90.0f;

		public override void ModifyMesh(VertexHelper vh)
		{
			if (enabled)
			{
				var rect = graphic.rectTransform.rect;
				var dir = RotationDir(Angle);

				var localPositionMatrix = LocalPositionMatrix(rect, dir);

				var vertex = default(UIVertex);
				for (var i = 0; i < vh.currentVertCount; i++)
				{
					vh.PopulateUIVertex(ref vertex, i);
					var localPosition = localPositionMatrix * vertex.position;
					vertex.color *= Color.Lerp(Color2, Color1, localPosition.y);
					vh.SetUIVertex(vertex, i);
				}
			}
		}

		public struct Matrix2x3
		{
			public readonly float M00, M01, M02, M10, M11, M12;

			public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
			{
				M00 = m00;
				M01 = m01;
				M02 = m02;
				M10 = m10;
				M11 = m11;
				M12 = m12;
			}

			public static Vector2 operator *(Matrix2x3 m, Vector2 v)
			{
				float x = (m.M00 * v.x) - (m.M01 * v.y) + m.M02;
				float y = (m.M10 * v.x) + (m.M11 * v.y) + m.M12;
				return new Vector2(x, y);
			}
		}

		private Matrix2x3 LocalPositionMatrix(Rect rect, Vector2 dir)
		{
			float cos = dir.x;
			float sin = dir.y;
			Vector2 rectMin = rect.min;
			Vector2 rectSize = rect.size;
			float c = 0.5f;
			float ax = rectMin.x / rectSize.x + c;
			float ay = rectMin.y / rectSize.y + c;
			float m00 = cos / rectSize.x;
			float m01 = sin / rectSize.y;
			float m02 = -(ax * cos - ay * sin - c);
			float m10 = sin / rectSize.x;
			float m11 = cos / rectSize.y;
			float m12 = -(ax * sin + ay * cos - c);
			return new Matrix2x3(m00, m01, m02, m10, m11, m12);
		}

		private Vector2 RotationDir(float angle)
		{
			float angleRad = angle * Mathf.Deg2Rad;
			float cos = Mathf.Cos(angleRad);
			float sin = Mathf.Sin(angleRad);
			return new Vector2(cos, sin);
		}
	}
}
