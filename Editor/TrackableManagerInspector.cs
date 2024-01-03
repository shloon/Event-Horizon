using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace EventHorizon.Editor
{
	[CustomEditor(typeof(TrackableManagerComponent))]
	public class ListViewExample : UnityEditor.Editor
	{
		private List<KeyValuePair<TrackableID, ITrackable>> items;
		private ListView listView;

		private void OnEnable() => items = new List<KeyValuePair<TrackableID, ITrackable>>();

		private void UpdateListView()
		{
			var sample = (TrackableManagerComponent) target;
			if (sample.RegisteredTrackables == null)
			{
				return;
			}

			items.Clear();
			foreach (var kvp in sample.RegisteredTrackables)
			{
				items.Add(kvp);
			}

			listView.Rebuild();
		}

		public override VisualElement CreateInspectorGUI()
		{
			var rootVisualElement = new VisualElement();

			var header = UIHelpers.CreateHeader("Detected Trackables");
			rootVisualElement.Add(header);

			const float itemHeight = 36;
			listView = new ListView(items,
				itemHeight,
				() => new ListItem(),
				(e, i) => BindItem(e as ListItem, i));
			listView.reorderable = false;
			listView.style.maxHeight = 250f;
			listView.style.flexGrow = 1f;
			listView.showBorder = true;
			rootVisualElement.Add(listView);

			rootVisualElement.schedule.Execute(UpdateListView).Every(1000); // Updates ~60 times a sec

			return rootVisualElement;
		}

		private void BindItem(ListItem elem, int i)
		{
			var trackableInfo = items[i];

			var trackableIdField = elem.Q<TextField>("trackableID");
			trackableIdField.value = trackableInfo.Key.IsValid ? trackableInfo.Key.ToString() : "Unassigned";

			var trackableNameField = elem.Q<Label>("trackableName");
			trackableNameField.text = trackableInfo.Value.Name;

			var locateBtn = elem.Q<Button>("locateBtn");
			if (trackableInfo.Value is Object obj)
			{
				locateBtn.userData = obj;
				locateBtn.SetEnabled(true);
			}
			else
			{
				locateBtn.SetEnabled(false);
			}
		}

		private class ListItem : VisualElement
		{
			public ListItem()
			{
				var rootElement = new VisualElement
				{
					style =
					{
						paddingTop = 5f,
						paddingBottom = 5f,
						paddingRight = 15f,
						paddingLeft = 15f,
						borderBottomColor = Color.gray,
						borderBottomWidth = 1f
					}
				};

				var trackableField = new VisualElement { name = "trackable" };
				trackableField.AddToClassList("unity-base-field");
				trackableField.AddToClassList("unity-composite-field");
				trackableField.AddToClassList("unity-base-field__inspector-field");

				var trackableLabel = new Label("Object");
				trackableLabel.name = "trackableName";
				trackableLabel.AddToClassList("unity-base-field__label");
				trackableLabel.AddToClassList("unity-composite-field__label");
				trackableField.Add(trackableLabel);

				var fieldData = new VisualElement();
				fieldData.AddToClassList("unity-base-field__input");
				fieldData.AddToClassList("unity-composite-field__input");
				trackableField.Add(fieldData);

				var trackableNameField = new TextField();
				trackableNameField.name = "trackableID";
				trackableNameField.label = "ID";
				trackableNameField.labelElement.style.minWidth = 30f;
				trackableNameField.AddToClassList("unity-composite-field__field");
				trackableNameField.AddToClassList("unity-composite-field__field--first");
				trackableNameField.AddToClassList("unity-base-field__inspector-field");
				fieldData.Add(trackableNameField);

				var locateBtn = new Button();
				locateBtn.name = "locateBtn";
				locateBtn.text = "Locate";
				locateBtn.style.marginLeft = 2;
				locateBtn.style.flexGrow = 0;
				locateBtn.style.minWidth = 100;
				locateBtn.AddToClassList("unity-composite-field__field");
				locateBtn.AddToClassList("unity-composite-field__field--first");
				locateBtn.clicked += () => Selection.activeObject = locateBtn.userData as Object;
				locateBtn.SetEnabled(false);
				fieldData.Add(locateBtn);

				rootElement.Add(trackableField);

				Add(rootElement);
			}
		}
	}
}