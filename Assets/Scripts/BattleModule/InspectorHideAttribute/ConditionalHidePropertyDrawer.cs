using UnityEngine;
using UnityEditor;

 
[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
        GUI.enabled = true;
        //bool wasEnabled = false;
        //GUI.enabled = false;
        if (condHAtt.HideInInspector==1&&enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
            
        }
        else if (condHAtt.HideInInspector == 0 && !enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        //GUI.enabled = false;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
        //!condHAtt.HideInInspector ||
        if (condHAtt.HideInInspector == 1 && enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);//位移
        }
        else if(condHAtt.HideInInspector == 0 && !enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);//位移
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;//不位移
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}