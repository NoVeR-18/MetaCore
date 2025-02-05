using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    public MeshFilter playerModelParent; // Родительский объект, где меняются модели игрока
    private GameObject currentModel;

    private void Start()
    {
        ApplySavedSkin();
    }

    private void ApplySavedSkin()
    {
        if (SkinManager.Instance != null)
        {
            List<SkinData> skins = SkinManager.Instance.GetSkinList();
            if (skins == null || skins.Count == 0) return;

            int selectedSkinIndex = SkinManager.Instance.GetSelectedSkinIndex();
            if (selectedSkinIndex >= 0 && selectedSkinIndex < skins.Count)
            {
                SetPlayerModel(skins[selectedSkinIndex].modelPrefab);
            }
        }
    }

    public void SetPlayerModel(Mesh newModelPrefab)
    {
        if (currentModel != null)
            Destroy(currentModel);

        if (newModelPrefab != null)
        {
            playerModelParent.mesh = newModelPrefab;
            //currentModel.transform.localPosition = Vector3.zero;
            //currentModel.transform.localRotation = Quaternion.identity;
        }
    }
}
