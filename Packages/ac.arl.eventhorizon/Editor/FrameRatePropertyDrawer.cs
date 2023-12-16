using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EventHorizon
{
	[CustomPropertyDrawer(typeof(FrameRate))]
	class FrameRatePropertyDrawer : PropertyDrawer
	{
		TextField textField;
		string internalText;
		string placeholderText;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			textField = new TextField();
			textField.AddToClassList("unity-base-field__aligned");
			textField.label = property.displayName;

			var currentFrameRate = (FrameRate) property.boxedValue;
			UpdateFrameRateDrawerStrings(currentFrameRate);

			textField.value = placeholderText;
			textField.RegisterCallback<FocusInEvent>(OnFocusIn);
			textField.RegisterCallback<FocusOutEvent>(ev => OnFocusOut(property));

			return textField;
		}

		private void UpdateFrameRateDrawerStrings(FrameRate frameRate)
		{
			internalText = $"{frameRate.numerator}" + (frameRate.denominator == 1 ? "" : $"/{frameRate.denominator}");
			placeholderText = frameRate.GetAsDouble().ToString("0.###") + " FPS";
		}

		private void OnFocusIn(FocusInEvent ev)
		{
			textField.value = internalText;
		}

		private void OnFocusOut(SerializedProperty property)
		{
			internalText = textField.value;
			if (FrameRate.TryParse(internalText, out FrameRate frameRate))
			{
				property.boxedValue = frameRate;
				property.serializedObject.ApplyModifiedProperties();
				UpdateFrameRateDrawerStrings(frameRate);
			}
			textField.value = placeholderText;
		}
	}
}