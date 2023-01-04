using UnityEditor;
[CustomEditor(typeof(AIInstance),true), CanEditMultipleObjects()]
public class AIInstanceEditor : Editor
{
    private Editor _editor;
    private bool _foldSO;
    private AIInstance myTarget;
    private SerializedProperty _serializedProperty;
    private const string PropertyName = "aiScriptableObject";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Draw ScriptableObject in the inspector
        _serializedProperty = serializedObject.FindProperty(PropertyName);
        //myTarget.gameObject.name = $"{_serializedProperty.name}";
        if(_serializedProperty != null && _serializedProperty.objectReferenceValue != null)
        {
            _foldSO = EditorGUILayout.InspectorTitlebar(_foldSO, _serializedProperty.objectReferenceValue);
            if (_foldSO)
            {
                CreateCachedEditor(_serializedProperty.objectReferenceValue, null, ref _editor);
                EditorGUI.indentLevel++;
                _editor.OnInspectorGUI();
            }
        }
    }
}