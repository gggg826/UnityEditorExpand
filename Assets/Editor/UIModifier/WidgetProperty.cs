/********************************************************************
*	created:	26/10/2017   11:39
*	filename: 	WidgetProperty
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class WidgetProperty
{
	public System.Action OnAllPropertiesChanged;
	public System.Action<UIWidget.Pivot> OnPivotChanged;
	public System.Action<int> OnDepthChanged;
	public System.Action<int, int> OnDimensionsChanged;
	public System.Action OnSnap;

	public string Name;
	public int PivotH;
	public int PivotV;
	public int Depth;
	public int Width;
	public int Height;
	public Vector3 Trans;
	protected bool state;

	#region Constructor
	public WidgetProperty(int pivotH = -1
						, int pivotV = -1
						, int depth = -1
						, int width = -1
						, int height = -1)
	{
		SetProperty(pivotH, pivotV, depth, width, height);
	}

	public void SetProperty(int pivotH = -1
					, int pivotV = -1
					, int depth = -1
					, int width = -1
					, int height = -1)
	{
		PivotH = pivotH;
		PivotV = pivotV;
		Depth = depth;
		Width = width;
		Height = height;
	}
	#endregion

	#region Public Method
	public void SetProperty(WidgetProperty content)
	{
		PivotH = content.PivotH;
		PivotV = content.PivotV;
		Depth = content.Depth;
		Width = content.Width;
		Height = content.Height;
	}

	public void DrawProperty()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		Color temp = GUI.color;
		GUI.color = Color.cyan;
		if (GUILayout.Button("RESET CONDITIONS"))
		{
			Reset();
		}
		GUI.color = temp;

		GUILayout.Space(20f);
		GUILayout.BeginHorizontal();
		Name = EditorGUILayout.TextField("GameobjectName", Name, GUILayout.MinWidth(100));
		//GUILayout.Space(18);
		GUILayout.EndHorizontal();

		GUILayout.Space(8);
		DrawTransform();
		GUILayout.Space(8);
		DrawSubProperty();
		GUILayout.Space(8);
		DrawWidget();
		GUILayout.Space(8);
		
		GUI.color = Color.cyan;
		if (GUILayout.Button("Modify All Properties", GUILayout.MinHeight(300)))
		{
			if (OnAllPropertiesChanged != null)
				OnAllPropertiesChanged();
		}
		GUI.color = temp;
		GUILayout.EndVertical();
	}

	public void Reset()
	{
		PivotH = 1;
		PivotV = 1;
		Depth = -1;
		Width = -1;
		Height = -1;
		Trans = new Vector3(-1, -1, -1);
		//state = false;

		SubReset();
	}

	public UIWidget.Pivot GetPivot()
	{
		return (UIWidget.Pivot)(3 * PivotH + PivotV);
	}
	#endregion

	#region Abstract method
	public abstract void SubReset();

	public abstract void DrawSubProperty();
	#endregion

	#region Render method
	protected void DrawTransform()
	{
		state = UIModifierUtils.DrawContentHeader("Transform", state);
		if (state)
		{
			UIModifierUtils.BeginContents(UIModifierUtils.DefaultContentColor);
			#region Sprie LocalPosition
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			bool reset = GUILayout.Button("P", GUILayout.Width(20f));
			EditorGUIUtility.labelWidth = 12;
			Trans.x = EditorGUILayout.FloatField("x", Trans.x, GUILayout.MinWidth(30f));
			Trans.y = EditorGUILayout.FloatField("y", Trans.y, GUILayout.MinWidth(30f));
			Trans.z = EditorGUILayout.FloatField("z", Trans.z, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 70;
			if (reset)
			{
				Trans.x = 0;
				Trans.y = 0;
				Trans.z = 0;
			}
			EditorGUIUtility.labelWidth = 70;
			GUILayout.EndHorizontal();
			#endregion
			UIModifierUtils.EndContents(UIModifierUtils.DefaultContentColor);
		}
	}

	protected void DrawWidget()
	{
		state = UIModifierUtils.DrawContentHeader("Widget", state);
		if (state)
		{
			//GUILayout.Space(10f);
			UIModifierUtils.BeginContents(UIModifierUtils.DefaultContentColor);
			#region Pivot
			GUILayout.Space(3f);
			int newPivotH = PivotH, newPivotV = PivotV;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pivot", GUILayout.Width(66f));
			if (GUILayout.Toggle(newPivotH == 0, "\u25C4", "ButtonLeft"))
				newPivotH = 0;
			if (GUILayout.Toggle(newPivotH == 1, "\u25AC", "ButtonMid"))
				newPivotH = 1;
			if (GUILayout.Toggle(newPivotH == 2, "\u25BA", "ButtonRight"))
				newPivotH = 2;
			if (GUILayout.Toggle(newPivotV == 0, "\u25B2", "ButtonLeft"))
				newPivotV = 0;
			if (GUILayout.Toggle(newPivotV == 1, "\u258C", "ButtonMid"))
				newPivotV = 1;
			if (GUILayout.Toggle(newPivotV == 2, "\u25BC", "ButtonRight"))
				newPivotV = 2;
			GUILayout.EndHorizontal();

			if (PivotH != newPivotH)
			{
				PivotH = newPivotH;
			}
			if (PivotV != newPivotV)
			{
				PivotV = newPivotV;
			}

			//if (GUILayout.Button("Change Pivot"))
			//{
			//	UIWidget.Pivot pivot = (UIWidget.Pivot)(3 * PivotV + PivotH);
			//	//Debug.Log(pivot.ToString());
			//	if (OnPivotChanged != null)
			//		OnPivotChanged(pivot);
			//}
			#endregion
			#region DrawDepth
			GUILayout.Space(3);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Depth", GUILayout.Width(66f));
			if (GUILayout.Button("Back", GUILayout.MinWidth(46f)))
			{
				Depth -= 1;
			}

			GUILayout.Space(5);
			int newDepth = EditorGUILayout.IntField("", Depth, GUILayout.MinWidth(50));
			if (newDepth != Depth)
			{
				Depth = newDepth;
			}
			GUILayout.Space(5);

			if (GUILayout.Button("Forward", GUILayout.MinWidth(60f)))
			{
				Depth += 1;
			}
			GUILayout.EndHorizontal();
			//if (GUILayout.Button("Change Depth"))
			//{
			//	//Debug.Log("Current Depth: " + Depth);
			//	if (OnDepthChanged != null)
			//		OnDepthChanged(Depth);
			//}
			#endregion
			#region DrawDimensions
			GUILayout.Space(3);
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			Width = EditorGUILayout.IntField("Size", Width, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 12;
			Height = EditorGUILayout.IntField("x", Height, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 70;
			if (GUILayout.Button("Snap", GUILayout.Width(60f)))
			{
				if (OnSnap != null)
					OnSnap();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.IntField("Aspect", -1, GUILayout.MinWidth(30f));
				EditorGUILayout.EnumPopup(UIWidget.AspectRatioSource.Free);
				EditorGUI.EndDisabledGroup();
			}
			GUILayout.EndHorizontal();
			//if (GUILayout.Button("Change Dimensions"))
			//{
			//	//Debug.LogError("UNDONE: Change this button to toggle");
			//	if (OnDimensionsChanged != null)
			//		OnDimensionsChanged(Width, Height);
			//}
			#endregion
			#region DrawAspect
			//GUILayout.BeginHorizontal();
			//{
			//	EditorGUI.BeginDisabledGroup(true);
			//	EditorGUILayout.IntField("Aspect", -1, GUILayout.MinWidth(30f));
			//	EditorGUI.EndDisabledGroup();
			//}
			//GUILayout.EndHorizontal();
			#endregion
			UIModifierUtils.EndContents(UIModifierUtils.DefaultContentColor);
		}
	}
	#endregion
}
