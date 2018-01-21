using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
public class Foo : EditorWindow
{
	Mesh mPreviewMesh;
	Material mPreviewMaterial;
	PreviewRenderUtility mPreviewRenderUtility;
	[MenuItem("Tools/Foo")]
	static void Setup()
	{
		GetWindow(typeof(Foo));
	}
	void OnGUI()
	{
		if (mPreviewRenderUtility == null)
		{
			mPreviewRenderUtility = new PreviewRenderUtility();
			mPreviewRenderUtility.m_Camera.farClipPlane = 500;
			mPreviewRenderUtility.m_Camera.clearFlags = CameraClearFlags.SolidColor;
			mPreviewRenderUtility.m_Camera.transform.position = new Vector3(0, 0, -10);
			var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			var meshFilter = go.GetComponent<MeshFilter>();
			mPreviewMesh = meshFilter.sharedMesh;
			mPreviewMaterial = go.GetComponent<Renderer>().sharedMaterial;
			DestroyImmediate(go);
		}
		var drawRect = new Rect(0, 0, 100, 100);
		mPreviewRenderUtility.BeginPreview(drawRect, GUIStyle.none);
		InternalEditorUtility.SetCustomLighting(mPreviewRenderUtility.m_Light, new Color(0.6f, 0.6f, 0.6f, 1f));
		mPreviewRenderUtility.DrawMesh(mPreviewMesh, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(30, 45, 0), Vector3.one), mPreviewMaterial, 0);
		mPreviewRenderUtility.m_Camera.Render(); var texture = mPreviewRenderUtility.EndPreview();
		InternalEditorUtility.RemoveCustomLighting();
		GUI.Box(drawRect, texture);
	}
}