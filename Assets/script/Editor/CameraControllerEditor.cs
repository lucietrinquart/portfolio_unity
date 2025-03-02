#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    private string selectedMachine = "";
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CameraController cameraController = (CameraController)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Outils de configuration des vues", EditorStyles.boldLabel);
        
        // Liste déroulante pour sélectionner une machine
        string[] machineNames = new string[cameraController.machineSettings.Length];
        for (int i = 0; i < cameraController.machineSettings.Length; i++)
        {
            machineNames[i] = cameraController.machineSettings[i].machineName;
        }
        
        if (machineNames.Length > 0)
        {
            int selectedIndex = EditorGUILayout.Popup("Sélectionner une vue", 
                System.Array.IndexOf(machineNames, selectedMachine), machineNames);
                
            if (selectedIndex >= 0 && selectedIndex < machineNames.Length)
            {
                selectedMachine = machineNames[selectedIndex];
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Tester cette vue"))
                {
                    cameraController.TestMachineView(selectedMachine);
                    SceneView.RepaintAll();
                }
                
                if (GUILayout.Button("Capturer la position et rotation actuelles"))
                {
                    Undo.RecordObject(cameraController, "Capture Camera Transform");
                    cameraController.CaptureCurrentTransform(selectedMachine);
                    EditorUtility.SetDirty(cameraController);
                }
                
                EditorGUILayout.HelpBox(
                    "1. Sélectionnez une vue dans la liste\n" +
                    "2. Positionnez manuellement la caméra dans la scène\n" +
                    "3. Cliquez sur 'Capturer la position et rotation actuelles'\n" +
                    "4. Testez la vue avec 'Tester cette vue'", 
                    MessageType.Info);
            }
        }
    }
}
#endif