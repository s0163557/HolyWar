using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[CustomEditor(typeof(SpellSO))]
public class SpellSOEditor : Editor
{
    private SerializedProperty spellEffectsProp;
    private SerializedProperty targetFilterProp;

    private Dictionary<string, Type> effectTypes;
    private Dictionary<string, Type> filterTypes;

    private void OnEnable()
    {
        spellEffectsProp = serializedObject.FindProperty("SpellEffects");

        //Найдем всех наследников SpellEffect'а
        effectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(y => typeof(SpellEffect).IsAssignableFrom(y) && !y.IsAbstract)
            .ToDictionary(y => y.Name, y => y);


        targetFilterProp = serializedObject.FindProperty("TargetFilter");
        //Найдем всех наследников TargetFilter'а
        filterTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(y => typeof(TargetFilter).IsAssignableFrom(y) && !y.IsAbstract)
            .ToDictionary(y => y.Name, y => y);
        
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Выпишем все переменные SpellSO кроме SpellEffects и TargetFilter
        DrawPropertiesExcluding(serializedObject, "SpellEffects");

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Target Filter", EditorStyles.boldLabel);
        targetFilterProp ??= serializedObject.FindProperty("TargetFilter");

        //Поле добавления фильтра
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Filter Type", EditorStyles.boldLabel);
        string typeName = targetFilterProp.managedReferenceValue == null ? "None" : targetFilterProp.managedReferenceValue.ToString();
        if (GUILayout.Button(typeName))
        {
            GenericMenu menu = new GenericMenu();
            foreach (var keyValuePair in filterTypes)
            {
                menu.AddItem(new GUIContent(keyValuePair.Key), false, () =>
                {
                    targetFilterProp.managedReferenceValue = Activator.CreateInstance(keyValuePair.Value);
                    serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spell Effects", EditorStyles.boldLabel);
        spellEffectsProp ??= serializedObject.FindProperty("SpellEffects");

        //Выпишем все значения эффектов
        for (int i = 0; i < spellEffectsProp.arraySize; i++)
        {
            var elementProp = spellEffectsProp.GetArrayElementAtIndex(i);
            var effectInstance = elementProp.managedReferenceValue;

            EditorGUILayout.BeginVertical("box");

            if (effectInstance != null)
            {
                EditorGUILayout.LabelField(effectInstance.GetType().Name, EditorStyles.boldLabel);

                var iterator = elementProp.Copy();
                var endProp = iterator.GetEndProperty();
                iterator.NextVisible(true);

                while (!SerializedProperty.EqualContents(iterator, endProp))
                {
                    if (iterator.name != "managedReferenceFullTypename")
                        EditorGUILayout.PropertyField(iterator, true);

                    iterator.NextVisible(false);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Null effect");
            }

            //Добавим кнопку удаления
            if (GUILayout.Button("Remove"))
            {
                spellEffectsProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        //Поле добавления новых эффектов
        if (GUILayout.Button("Add effect"))
        {
            GenericMenu menu = new GenericMenu();

            foreach (var keyValuePair in effectTypes)
            {
                menu.AddItem(new GUIContent(keyValuePair.Key), false, () =>
                {
                    int newIndex = spellEffectsProp.arraySize;
                    spellEffectsProp.InsertArrayElementAtIndex(newIndex);
                    var newElement = spellEffectsProp.GetArrayElementAtIndex(newIndex);
                    newElement.managedReferenceValue = Activator.CreateInstance(keyValuePair.Value);
                    serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
