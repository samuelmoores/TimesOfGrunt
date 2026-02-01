using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform pistolSocket;
    [SerializeField] Camera cam;
    GameObject weapon;

    bool hasWeapon = false;
    bool aiming = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1) && hasWeapon)
        {
            aiming = true;
            animator.SetBool("aim", true);
        }
        else
        {
            aiming = false;
            animator.SetBool("aim", false);
        }


    }

    void AttachWeapon(GameObject weaponToAttach)
    {
        weaponToAttach.transform.position = pistolSocket.position;
        weaponToAttach.transform.parent = pistolSocket;
    }

    public bool Aiming()
    {
        return aiming;
    }

    public void SetWeapon(GameObject newWeapon)
    {
        hasWeapon = true;
        animator.SetBool("hasPistol", true);
        AttachWeapon(newWeapon);
    }
}
