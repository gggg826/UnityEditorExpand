/********************************************************************
*	created:	30/10/2017  11:10
*	filename: 	UIModifierHelper
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;

public class UIModifierHelper
{
	/// <summary>
	/// Just Modify LocalPosition, not all values of Transform.
	/// </summary>
	/// <param name="widget"></param>
	/// <param name="value"></param>
	static public void ChangeTransform(UIWidget widget, WidgetProperty value)
	{
		if(value.Trans != new Vector3(-1,-1,-1))
		{
			widget.transform.localPosition = value.Trans;
		}
	}

	static public void ChangeWidgetProperties(UIWidget widget, WidgetProperty value)
	{
		widget.pivot = value.GetPivot();

		if(value.Depth != -1)
		{
			widget.depth = value.Depth;
		}

		if(value.Width != -1 && value.Height != -1)
		{
			widget.width = value.Width;
			widget.height = value.Height;
		}

		if (widget is UISprite)
		{
			widget.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		}
	}

	static public bool OnSnap(UIWidget widget, WidgetProperty value)
	{
		if (!string.IsNullOrEmpty(value.Name) && widget.transform.name != value.Name)
			return false;

		if(widget is UILabel)
		{
			EditorUtility.DisplayDialog("Error", "Operation is forbidden in UILabel!!", "OK");
			return false;
		}

		UISprite sprite = widget as UISprite;
		sprite.type = UIBasicSprite.Type.Simple;
		sprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		sprite.MakePixelPerfect();
		EditorUtility.SetDirty(widget);
		return true;
	}

	static public void ChangeLabelProperties(UILabel label, LabelProperty value)
	{
		if (label == null)
			return;

		if(value.UI_Font != null)
		{
			label.bitmapFont = value.UI_Font;
		}

		if(value.Font_Size != -1)
		{
			label.fontSize = value.Font_Size;
		}

		label.fontStyle = value.Font_Style;

		label.modifier = value.Modifier_Type;

		label.alignment = value.Alig_Type;

		label.effectStyle = value.Effect_Type;
		if(label.effectStyle != UILabel.Effect.None)
		{
			label.effectColor = value.Effect_Color;
			label.effectDistance = value.Effect_Distance;
		}

		label.overflowMethod = value.Overflow_Method;

		label.overflowWidth = value.Overflow_Width;

		label.applyGradient = value.Gradient;

		if(label.applyGradient)
		{
			label.gradientTop = value.Gradient_Top_Color;
			label.gradientBottom = value.Gradient_Bottom_Color;
		}

		label.supportEncoding = value.BBCode;
		if(label.supportEncoding)
		{
			label.symbolStyle = value.Symbol_Style;
		}

		if(value.Max_Lines !=-1)
		{
			label.maxLineCount = value.Max_Lines;
		}

		label.keepCrispWhenShrunk = value.Crisp;
	}

	static public void ChangeSpriteProperties(UISprite sprite, SpriteProperty spriteValue)
	{
		if (sprite == null)
			return;

		if(spriteValue.Atlas != null)
		{
			sprite.atlas = spriteValue.Atlas;
			if(!string.IsNullOrEmpty(spriteValue.Sprite_Name))
				sprite.spriteName = spriteValue.Sprite_Name;
		}

		sprite.flip = spriteValue.Flip_Type;
		sprite.type = spriteValue.Sprite_Type;

		if (sprite.type == UISprite.Type.Simple)
		{
			sprite.applyGradient = spriteValue.Gradient;
			if(sprite.applyGradient)
			{
				sprite.gradientTop    = spriteValue.Gradient_Top_Color;
				sprite.gradientBottom = spriteValue.Gradient_Bottom_Color;
			}
		}
		else if (sprite.type == UISprite.Type.Tiled)
		{

		}
		else if (sprite.type == UISprite.Type.Sliced)
		{
			sprite.fillCenter    = spriteValue.Fill_Center;

			sprite.applyGradient = spriteValue.Gradient;
			if (sprite.applyGradient)
			{
				sprite.gradientTop    = spriteValue.Gradient_Top_Color;
				sprite.gradientBottom = spriteValue.Gradient_Bottom_Color;
			}
		}
		else if (sprite.type == UISprite.Type.Filled)
		{
			sprite.fillDirection = spriteValue.FillDirection_Type;
			sprite.fillAmount    = spriteValue.Fill_Amount;
			sprite.invert        = spriteValue.Invert;
		}
		else if (sprite.type == UISprite.Type.Advanced)
		{
			sprite.leftType   = spriteValue.Left_Type;
			sprite.rightType  = spriteValue.Right_Type;
			sprite.bottomType = spriteValue.Bottom_Type;
			sprite.topType    = spriteValue.Top_Type;
			sprite.centerType = spriteValue.Center_Type;
		}
	}

	static public bool ChangeAllProperties(UIWidget widget, object value)
	{
		if (widget is UILabel)
		{
			UILabel label = widget as UILabel;
			LabelProperty labelValue = value as LabelProperty;

			if (!string.IsNullOrEmpty(labelValue.Name) && widget.transform.name != labelValue.Name)
				return false;

			ChangeLabelProperties(label, labelValue);
			ChangeTransform(widget, labelValue);
			ChangeWidgetProperties(widget, labelValue);
			EditorUtility.SetDirty(label);
			return true;
		}
		else if(widget is UISprite)
		{
			UISprite sprite = widget as UISprite;
			SpriteProperty spriteValue = value as SpriteProperty;

			if (!string.IsNullOrEmpty(spriteValue.Name) && widget.transform.name != spriteValue.Name)
				return false;

			ChangeSpriteProperties(sprite, spriteValue);
			ChangeTransform(sprite, spriteValue);
			ChangeWidgetProperties(sprite, spriteValue);
			EditorUtility.SetDirty(sprite);
			return true;
		}
		return false;
	}

	static public bool UpgradeUIImageButton(UIImageButton uiImageButton)
	{

		UIButton btn = uiImageButton.GetComponent<UIButton>();

		if (btn == null)
		{
			btn = uiImageButton.gameObject.AddComponent<UIButton>();
			if (uiImageButton.target != null)
				btn.tweenTarget = uiImageButton.target.gameObject;
			else
				btn.tweenTarget = uiImageButton.gameObject;

			UISprite sp = btn.tweenTarget.GetComponent<UISprite>();
			if (sp != null)
				sp.spriteName = uiImageButton.normalSprite;
		}

		btn.EnableColor = false;
		btn.hoverSprite = uiImageButton.hoverSprite;
		btn.pressedSprite = uiImageButton.pressedSprite;
		btn.disabledSprite = uiImageButton.disabledSprite;
		//btn.pixelSnap = uiImageButton.pixelSnap;
		btn.pixelSnap = false;
	
		if (uiImageButton != null)
		{
			if (Application.isEditor)
				UnityEngine.Object.DestroyImmediate(uiImageButton);
			else
				UnityEngine.Object.Destroy(uiImageButton);
		}
		EditorUtility.SetDirty(uiImageButton);
		return true;
	}

	static public bool ChangeButtonProperties(UIButton button, ButtonProperty value)
	{
		if (button == null)
			return false;
		if(button.tweenTarget == null)
		{
			Debug.LogError("There has none TweenTarget..  Obj: " + button.gameObject.name);
			return false;
		}

		if(value.EnableColor)
		{
			button.EnableColor      = true;
			button.defaultColor     = value.Normal_Color;
			button.hover            = value.Hover_Color;
			button.pressed          = value.Pressed_Color;
			button.disabledColor    = value.Disabled_Color;
		}

		if(value.EnableSprite)
		{
			UISprite sprite = button.tweenTarget.GetComponent<UISprite>();
			if (sprite == null)
			{
				Debug.LogError("Check State. It's not Sprtie.  Obj: " + button.tweenTarget.name);
				return false;
			}

			sprite.atlas            = value.Atlas;
			sprite.spriteName = value.Normal_Sprite;
			sprite.type = UIBasicSprite.Type.Sliced;
			EditorUtility.SetDirty(sprite);

			button.normalSprite     = value.Normal_Sprite;
			button.hoverSprite      = value.Hover_Sprite;
			button.pressedSprite    = value.Pressed_Sprite;
			button.disabledSprite   = value.Disabled_Sprite;
		}
		else
		{
			UI2DSprite sprite2D = button.tweenTarget.GetComponent<UI2DSprite>();
			if (sprite2D == null)
			{
				Debug.LogError("Check State. It's not Sprite2D..  Obj: " + button.tweenTarget.name);
				return false;
			}
			sprite2D.sprite2D = value.Normal_Sprite2D;
			EditorUtility.SetDirty(sprite2D);

			button.normalSprite2D   = value.Normal_Sprite2D;
			button.hoverSprite2D    = value.Hover_Sprite2D;
			button.pressedSprite2D  = value.Pressed_Sprite2D;
			button.disabledSprite2D = value.Disabled_Sprite2D;
		}
		EditorUtility.SetDirty(button);
		return true;
	}
}
