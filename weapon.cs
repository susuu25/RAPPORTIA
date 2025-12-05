using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Método virtual para Fire, permitindo que subclasses o sobrescrevam
    public virtual void Fire()
    {
    }

    // Método virtual para OnEquip, permitindo que subclasses o sobrescrevam
    public virtual void OnEquip()
    {
    }
}