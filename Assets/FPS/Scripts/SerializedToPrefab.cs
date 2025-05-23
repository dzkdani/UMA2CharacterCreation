using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;
using UMA.Editors;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SerializedToPrefab : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;

    [Header("Setting")]
    [SerializeField] private string prefabName;
    [SerializeField] private DynamicCharacterAvatar baseObject;
    [SerializeField] private bool UnswizzleNormalMaps = true;
    [SerializeField] private bool AddStandaloneDNA = true;
    [SerializeField] private string folderPath;

    private void Awake()
    {
#if UNITY_EDITOR
        if (button != null)
        {
            button.gameObject.SetActive(true);
            button.onClick.AddListener(MakeASerializedPrefab);
        }
#else
        if (button != null)
        {
            button.gameObject.SetActive(false);
        }
#endif
    }

    /// <summary>
    /// This function is only available in the editor and creates a prefab of this GameObject.
    /// </summary>
    private void MakeASerializedPrefab()
    {
#if UNITY_EDITOR

        string folderPath = "Assets/FPS/Output";
        string CharacterName = baseObject.gameObject.name + "_Prefab.prefab";

        if (!string.IsNullOrEmpty(this.folderPath))
        {
            folderPath = this.folderPath;
        }

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(folderPath, "");
        }

        if (!string.IsNullOrEmpty(prefabName))
        {
            CharacterName = prefabName;
        }

        // Create the prefab and save it
        if (baseObject != null)
        {
            UMAAvatarLoadSaveMenuItems.ConvertToNonUMA(baseObject.gameObject, baseObject, folderPath, UnswizzleNormalMaps, CharacterName, AddStandaloneDNA);
        }
        else
        {
            if (baseObject == null)
            {
                Debug.Log("A valid character with DynamicCharacterAvatar or DynamicAvatar must be supplied");
            }
            if (string.IsNullOrEmpty(CharacterName))
            {
                Debug.Log("Prefab Name cannot be empty");
            }
        }

#else
        Debug.LogWarning("MakeASerializedPrefab called outside of the editor.");
#endif
    }
}
