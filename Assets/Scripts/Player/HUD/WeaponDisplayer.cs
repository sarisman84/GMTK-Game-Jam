using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player.HUD
{
    public class WeaponDisplayer : MonoBehaviour
    {
        public Image iconDisplayer;

        public Transform barrelDisplayer;


        public void OnWeaponSelection(WeaponSettings selectedWeapon)
        {
            if (iconDisplayer)
            {
                if (!iconDisplayer.transform.parent.gameObject.activeSelf && gameObject.activeSelf)
                    iconDisplayer.transform.parent.gameObject.SetActive(true);
                iconDisplayer.sprite = selectedWeapon.weaponIcon;
            }

            if (barrelDisplayer)
            {
                FindOrCreateWeaponModel(
                    selectedWeapon.weaponModel);
            }
        }

        private GameObject FindOrCreateWeaponModel(GameObject weaponModel)
        {
            if (!weaponModel) return null;
            GameObject result = null;
            for (int i = 0; i < barrelDisplayer.childCount; i++)
            {
                Transform child = barrelDisplayer.GetChild(i);
                child.gameObject.SetActive(false);
                if (child.gameObject.name.Contains(weaponModel.name))
                {
                    result = child.gameObject;
                }
            }

            if (!result)
            {
                result = Instantiate(weaponModel, barrelDisplayer);
                result.transform.localPosition = Vector3.up;
                result.transform.localRotation = Quaternion.identity;
            }

            result.SetActive(true);
            return result;
        }


        public void Reset()
        {
            SetActive(false);
        }

        public void SetActive(bool state)
        {
            iconDisplayer.transform.parent.gameObject.SetActive(state);
        }
    }
}