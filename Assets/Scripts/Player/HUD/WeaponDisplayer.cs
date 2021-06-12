using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player.HUD
{
    public class WeaponDisplayer : MonoBehaviour
    {
        public Image iconDisplayer;
        private WeaponController _weaponController;
        public Transform barrelDisplayer;

        private void Awake()
        {
            _weaponController = GetComponent<WeaponController>();
        }

        public void OnWeaponSelection()
        {
            if (!_weaponController) return;

            if (iconDisplayer)
            {
                iconDisplayer.sprite = _weaponController.weaponLibrary[_weaponController.CurrentWeapon].weaponIcon;
            }

            if (barrelDisplayer)
            {
                FindOrCreateWeaponModel(
                    _weaponController.weaponLibrary[_weaponController.CurrentWeapon].weaponModel);
            }
        }

        private GameObject FindOrCreateWeaponModel(GameObject weaponModel)
        {
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
                result.transform.localPosition = Vector3.zero;
                result.transform.localRotation = Quaternion.identity;
            }

            result.SetActive(true);
            return result;
        }
    }
}