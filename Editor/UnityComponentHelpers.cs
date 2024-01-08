﻿using UnityEngine;

namespace EventHorizon.MetaXR.Editor
{
	public class UnityComponentHelpers
	{
		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			var type = original.GetType();
			var copy = destination.AddComponent(type);
			var fields = type.GetFields();
			foreach (var field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}

			return copy as T;
		}
	}
}