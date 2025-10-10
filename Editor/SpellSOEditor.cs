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
    private Dictionary<string, Type> effectTypes;

    private void OnEnable()
    {
        spellEffectsProp = serializedObject.FindProperty("SpellEffects");

        //������ ���� ����������� SpellEffect'�
        effectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(y => typeof(SpellEffect).IsAssignableFrom(y) && !y.IsAbstract)
            .ToDictionary(y => y.Name, y => y);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        spellEffectsProp ??= serializedObject.FindProperty("SpellEffects");

        //������� ��� ���������� SpellSO ����� SpellEffects
        DrawPropertiesExcluding(serializedObject, "SpellEffects");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spell Effects", EditorStyles.boldLabel);

        //������� ��� �������� ��������
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

            //������� ������ ��������
            if (GUILayout.Button("Remove"))
            {
                spellEffectsProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        //���� ���������� ����� ��������
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
