namespace EasyDoorSystem
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(EasyDoor))]
    public class EasyDoorEditor : Editor
    {
        private EasyDoor door;
        private SerializedObject serializedDoor;
        private Texture2D logo;

        private bool showSettings = true;
        private bool showTransforms = true;
        private bool showAudio = true;
        private bool showEvents = true;
        private bool showItemSettings = true;

        private void OnEnable()
        {
            door = (EasyDoor)target;
            if (door == null) return;

            serializedDoor = new SerializedObject(door);
            logo = Resources.Load<Texture2D>("easy-door-system-icon");
        }

        public override void OnInspectorGUI()
        {
            if (door == null || serializedDoor == null)
            {
                EditorGUILayout.HelpBox("EasyDoor reference missing!", MessageType.Error);
                return;
            }

            serializedDoor.Update();

            DrawLogo();
            DrawMainSettings();
            DrawItemSettings();     // 👈 ITEM PANELİ
            DrawTransformControls();
            DrawUtilities();
            EditorGUILayout.Space(20);
            DrawAudioSettings();
            DrawEventSettings();

            serializedDoor.ApplyModifiedProperties();
        }

        private void DrawLogo()
        {
            if (!logo) return;

            GUILayout.Space(10);
            Rect rect = GUILayoutUtility.GetRect(300, 60);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
            GUILayout.Space(10);
        }

        private void DrawMainSettings()
        {
            showSettings = EditorGUILayout.Foldout(showSettings, EditorGUIUtility.IconContent("Settings"), true);
            if (!showSettings) return;

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("movementType"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("movementSpeed"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("rotationSpeed"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("autoCloseDelay"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("detectionRange"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("interactKey"));
            EditorGUILayout.HelpBox("Player objesi 'Player' tag'ine sahip olmalı.", MessageType.Info);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // ======================= ITEM PANELİ =======================

        private void DrawItemSettings()
        {
            showItemSettings = EditorGUILayout.Foldout(showItemSettings, new GUIContent("🔑 Key / Item Settings"), true);
            if (!showItemSettings) return;

            EditorGUILayout.BeginVertical("HelpBox");

            SerializedProperty requiresItem = serializedDoor.FindProperty("requiresItem");
            SerializedProperty requiredItem = serializedDoor.FindProperty("requiredItem");
            SerializedProperty consumeItemOnUse = serializedDoor.FindProperty("consumeItemOnUse");
            
            SerializedProperty lockedSound = serializedDoor.FindProperty("lockedSound");

            EditorGUILayout.PropertyField(requiresItem, new GUIContent("Requires Item"));

            if (requiresItem.boolValue)
            {
                EditorGUILayout.PropertyField(requiredItem, new GUIContent("Required Item"));
                EditorGUILayout.PropertyField(consumeItemOnUse, new GUIContent("Consume Item On Use"));
               
                EditorGUILayout.PropertyField(lockedSound, new GUIContent("Locked Sound"));

                EditorGUILayout.HelpBox("Bu kapı sadece seçili slotta bu item varsa açılır.", MessageType.Info);
            }

            else
            {
                EditorGUILayout.HelpBox("Bu kapı item istemiyor, normal kapı gibi çalışır.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // ======================= TRANSFORM =======================

        private void DrawTransformControls()
        {
            showTransforms = EditorGUILayout.Foldout(showTransforms, EditorGUIUtility.IconContent("d_Transform Icon"), true);
            if (!showTransforms) return;

            EditorGUILayout.BeginVertical("HelpBox");

            GUIStyle stateStyle = new GUIStyle(EditorStyles.helpBox);
            stateStyle.normal.textColor = door.IsOpen ? Color.green : Color.red;
            EditorGUILayout.LabelField($"Current State: {(door.IsOpen ? "Open" : "Closed")}", stateStyle);

            EditorGUILayout.LabelField($"Position: {door.transform.localPosition}");
            EditorGUILayout.LabelField($"Rotation: {door.transform.localEulerAngles}");

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button(" Save Open State", GUILayout.Height(40)))
            {
                door.SaveCurrentState(true);
                SaveCurrentTransformStates(true);
            }

            GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
            if (GUILayout.Button(" Save Closed State", GUILayout.Height(40)))
            {
                door.SaveCurrentState(false);
                SaveCurrentTransformStates(false);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void SaveCurrentTransformStates(bool isOpen)
        {
            if (serializedDoor == null) return;

            SerializedProperty posProp = serializedDoor.FindProperty(isOpen ? "openedPosition" : "closedPosition");
            SerializedProperty rotProp = serializedDoor.FindProperty(isOpen ? "openedRotation" : "closedRotation");

            if (posProp != null) posProp.vector3Value = door.transform.localPosition;
            if (rotProp != null) rotProp.vector3Value = door.transform.localEulerAngles;
        }

        // ======================= AUDIO =======================

        private void DrawAudioSettings()
        {
            showAudio = EditorGUILayout.Foldout(showAudio, EditorGUIUtility.IconContent("AudioSource Icon"), true);
            if (!showAudio) return;

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("doorOpenSound"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("doorCloseSound"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("audioVolume"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // ======================= EVENTS =======================

        private void DrawEventSettings()
        {
            showEvents = EditorGUILayout.Foldout(showEvents, EditorGUIUtility.IconContent("EventSystem Icon"), true);
            if (!showEvents) return;

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("OnDoorOpening"));
            EditorGUILayout.PropertyField(serializedDoor.FindProperty("OnDoorClosed"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // ======================= UTILITIES =======================

        private void DrawUtilities()
        {
            EditorGUILayout.LabelField("🛠 Utilities", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("OPEN", GUILayout.Height(30)))
                door.OpenDoor();

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("CLOSE", GUILayout.Height(30)))
                door.CloseDoor();

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
    }
}

