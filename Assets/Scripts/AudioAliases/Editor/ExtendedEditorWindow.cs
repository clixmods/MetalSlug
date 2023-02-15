using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Audio.Editor
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected List<SerializedObject> serializedObject;    
        protected SerializedProperty currentProperty;
        protected SerializedObject selectedSerializeObject;


        private string selectedPropertyPath;
        protected SerializedProperty selectedProperty;
        protected SerializedProperty currentElemFromArraySelected;
        protected bool showAll = false;

        protected const string AliasesPropertyName = "aliases";
        protected const string AliasesTagCategoryName = "Audio Aliase/";
        
        protected void DrawSidebar(SerializedObject prop)
        {
            EditorGUILayout.BeginHorizontal();
            currentProperty = prop.FindProperty(AliasesPropertyName);
            if (GUILayout.Button($"{prop.targetObject.name} Count : {currentProperty.arraySize}" ))
            {
                selectedSerializeObject = prop;
                showAll = false;
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawField(string propName, bool relative)
        {
            if( relative && currentElemFromArraySelected != null)
            {
                SerializedProperty sP = currentElemFromArraySelected.FindPropertyRelative(propName);
                if (propName == "isLooping")
                {

                }
                EditorGUILayout.PropertyField(sP, true);
            }
            else if (selectedSerializeObject != null)
            {
                EditorGUILayout.PropertyField(selectedSerializeObject.FindProperty(propName),true);
            }
        }
        protected virtual void Apply()
        {
        }
    }
}
