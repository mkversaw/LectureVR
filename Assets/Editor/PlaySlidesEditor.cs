using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// Much of this was made while referencing https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
// it's a fantastic resource on reorderable lists in Unity.

// Displaying the sprites, and resizing the list was mainly achieved by analyzing
// https://pastebin.com/WhfRgcdC
// Very nice code that I should probably emulate more closely but it's 2 in the morning.

[CustomEditor(typeof(PlaySlides))]
[CanEditMultipleObjects]
public class PlaySlidesEditor : Editor
{
	
	// Makes it easier to mess with the spacing in that first lambda function
	private int audioFieldSize = 100;
	private int animFieldSize = 100;
	private int slideFieldSize = 100;
	
	// Defining a GUIStyle so that we can use html tags on the labels in the inspector
	private GUIStyle htmlStyle = GUIStyle.none;
	
    private ReorderableList list;
	
	//private AudioSource speaker;
	
	
	private void OnEnable(){
		
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("slides"), true, true, true, true);
		
		// ALL OF THESE LAMBDAS ARE CALLBACK FUNCTIONS. THAT MEANS THEY'RE GONNA GET
		// CALLED BY UNITY A LOT BEHIND THE SCENES. THEY AREN'T REALLY FOR OUR USE.
		
		// A lambda function that gives the reorderable list actual information to draw
		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
		
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			
			// gets the sprite assigned to a specific element
			Sprite s = (element.FindPropertyRelative("slide").objectReferenceValue as Sprite);
			
			// sets the margin between elements
			rect.y += 2;
			
			// sets the height of each element
			rect.height = 50;
			
			// Sets up the Sprite display
			if(s != null){
				EditorGUI.DrawPreviewTexture(
					new Rect(rect.x, rect.y, rect.width - slideFieldSize - audioFieldSize - animFieldSize, rect.height),  // Rect(x position, y position, width, height)
					s.texture // draws the sprite as a texture
					);
			} else{
				
				EditorGUI.LabelField(
				new Rect(rect.x, rect.y, rect.width - slideFieldSize - audioFieldSize - animFieldSize, rect.height),  // Rect(x position, y position, width, height)
				"empty"
				);
				
			}
			
			// Sets up the Sprite field
			EditorGUI.PropertyField(
				new Rect(rect.x + rect.width - slideFieldSize - audioFieldSize - animFieldSize, rect.y, slideFieldSize, rect.height),  // Rect(x position, y position, width, height)
				element.FindPropertyRelative("slide"), // Finds the "slide" variable from the SlideOrder struct
				GUIContent.none // Fills the space with empty
				);
				
			// Sets up the attachedAudio field
			EditorGUI.PropertyField(
				new Rect(rect.x + rect.width - audioFieldSize - animFieldSize, rect.y, audioFieldSize, rect.height), // Rect(x position, y position, width, height)
				element.FindPropertyRelative("attachedAudio"), // Finds the "attachedAudio" variable from the SlideOrder struct
				GUIContent.none // Fills the space with empty (0 in this case because it's a float)
				);
				
			// Sets up the attachedAnim field
			EditorGUI.PropertyField(
				new Rect(rect.x + rect.width - animFieldSize, rect.y, audioFieldSize, rect.height), // Rect(x position, y position, width, height)
				element.FindPropertyRelative("attachedAnim"), // Finds the "attachedAnim" variable from the SlideOrder struct
				GUIContent.none // Fills the space with empty (0 in this case because it's a float)
				);
		};
		
		
		// This lambda function determines the height of the element bar.
		list.elementHeightCallback = (index) => {
			
			// Honestly not sure what this does.
			Repaint();
			
			// Whatever value returned becomes the height of the specific element bar.
			return 55;
			
		};
		
		// Another lambda function that defines the title of this reorderable list
		list.drawHeaderCallback = (Rect rect) => {
			
			EditorGUI.LabelField(rect, "Sequence of Slides");
			
		};
		
		// Another lambda function. This one highlights the selected slide sprite when selected in the list
		list.onSelectCallback = (ReorderableList l) => {
			
			var slide = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("slide").objectReferenceValue as GameObject;
			
			// if the slide exists in the project? I think???
			if (slide)
				EditorGUIUtility.PingObject(slide.gameObject);
			
		};
		
		// Yet another lambda function. This one prevents you from removing the last slide in the list
		list.onCanRemoveCallback = (ReorderableList l) => {
			
			// returns true if the number of items is greater than 1.
			// otherwise the function returns false on being able to remove an item.
			return l.count > 1;
			
		};
		
		htmlStyle.richText = true;
		
	}
	
	public override void OnInspectorGUI(){
		
		serializedObject.Update();
		
		//GUILayout.BeginHorizontal();
		//GUILayout.Label("<b>Audio Source<b>", htmlStyle);
		base.DrawDefaultInspector();
		
		GUILayout.Space(20f);
		GUILayout.Label("<b>Edit Slides</b>", htmlStyle);
		GUILayout.Space(5f);
		

		list.DoLayoutList();
		
		serializedObject.ApplyModifiedProperties();
	}
}
