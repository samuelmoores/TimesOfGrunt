using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] PlayerAttack playerAttack;
    bool foundWeapon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetWeapon(GameObject newWeapon)
    {
        playerAttack.SetWeapon(newWeapon);
    }

    public bool FoundWeapon()
    {
        return foundWeapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pickup"))
        {
            foundWeapon = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            foundWeapon = false;
        }
    }
}
