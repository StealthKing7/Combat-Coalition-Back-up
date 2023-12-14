using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class scr_WeaponAttachmentUI : MonoBehaviour
{
    public static scr_WeaponAttachmentUI Instance { get; private set; }



    [Serializable]
    public class WeaponPartButton
    {
        public scr_Models.AttachmentTypes partType;
        public Button button;
        public TextMeshProUGUI buttonTextMesh;
    }


    [SerializeField] private List<WeaponPartButton> weaponPartButtonList;


    private void Awake()
    {
        Instance = this;

        foreach (WeaponPartButton weaponPartButton in weaponPartButtonList)
        {
            weaponPartButton.button.onClick.AddListener(() => {
                scr_GunAttachmentsSystem.Instance.ChangePart(weaponPartButton.partType);
            });
        }
    }

    private void Start()
    {
        scr_GunAttachmentsSystem.Instance.OnAnyPartChanged += WeaponAttachmentSystem_OnAnyPartChanged;

        RefreshButtonTextMesh();
    }

    private void WeaponAttachmentSystem_OnAnyPartChanged(object sender, EventArgs e)
    {
        RefreshButtonTextMesh();
    }

    private void RefreshButtonTextMesh()
    {
        foreach (WeaponPartButton weaponPartButton in weaponPartButtonList)
        {
            // Don't like how the numbers look
            //weaponPartButton.buttonTextMesh.text = weaponPartButton.partType.ToString().ToUpper() + " " + (WeaponAttachmentSystem.Instance.GetPartIndex(weaponPartButton.partType)+1);
            weaponPartButton.buttonTextMesh.text = weaponPartButton.partType.ToString().ToUpper();
        }

    }
    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
