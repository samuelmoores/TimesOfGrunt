using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform pistolSocket;
    GameObject weapon;

    bool hasWeapon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AttachWeapon(GameObject weaponToAttach)
    {
        weaponToAttach.transform.position = pistolSocket.position;
        weaponToAttach.transform.parent = pistolSocket;
        Debug.Log(weaponToAttach.transform.position);
    }

    public void SetWeapon(GameObject newWeapon)
    {
        hasWeapon = true;
        animator.SetBool("hasPistol", true);
        AttachWeapon(newWeapon);
    }
}
