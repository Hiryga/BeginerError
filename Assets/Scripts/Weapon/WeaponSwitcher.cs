using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject bow;

    private GameObject activeWeapon;

    private void Start()
    {
        SetActiveWeapon(sword);
    }

    private void Update()
    {
        // Можно заменить на Input Actions!
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(sword);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(bow);
        }
    }

    private void SetActiveWeapon(GameObject weapon)
    {
        sword.SetActive(false);
        bow.SetActive(false);

        if (weapon != null)
        {
            weapon.SetActive(true);
            activeWeapon = weapon;
        }
    }

    public GameObject GetActiveWeapon()
    {
        return activeWeapon;
    }
}
