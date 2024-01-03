using UnityEngine.UIElements;

namespace EventHorizon
{
	public static class UIHelpers
	{
		public static VisualElement CreateHeader(string headerText)
		{
			var decoratorContainer = new VisualElement();
			decoratorContainer.AddToClassList("unity-decorator-drawers-container");

			var metadataLabel = new Label(headerText);
			metadataLabel.AddToClassList("unity-header-drawer__label");
			decoratorContainer.Add(metadataLabel);

			return decoratorContainer;
		}
	}
}