using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform pistolSocket;
    [SerializeField] Camera cam;
    [SerializeField] Transform defaultTarget;
    [SerializeField] Transform aimTarget;
    [SerializeField] RectTransform aimReticle;
    [SerializeField] CinemachineCamera cineCam;
    [SerializeField] CinemachineOrbitalFollow cineOrbit;
    [SerializeField] float centerOrbitAimHeight; // 0.07f
    [SerializeField] float centerOrbitAimRadius; // 2.07f
    [SerializeField] float topOrbitAimHeight;
    [SerializeField] float topOrbitAimRadius;
    [SerializeField] Transform MuzzleFlash;
    [SerializeField] GameObject plasmaExplosionInstance;
    [SerializeField] GameObject plasmaExplosionHitInstance;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] TextMeshProUGUI ammoText;
    GameObject weapon;

    Vector2 screenPoint;
    Vector3 aimPosition;
    Vector3 defaultPosition;

    float orbitDefaultHeight;
    float orbitDefaultRadius;
    float rayDistance = 100.0f;

    bool hasWeapon = false;
    bool aiming = false;
    float attackCooldown;

    int ammo = 5;
    int currentAmmo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orbitDefaultHeight = cineOrbit.Orbits.Center.Height;
        orbitDefaultRadius = cineOrbit.Orbits.Center.Radius;
        aimReticle.gameObject.SetActive(false);
        currentAmmo = ammo;

    }

    // Update is called once per frame
    void Update()
    {
        bool dead = playerMovement.isDead();
        ammoText.text = currentAmmo.ToString();
        if(Input.GetKey(KeyCode.Mouse1) && hasWeapon && !dead)
        {
            cineCam.Follow = aimTarget;
            cineOrbit.Orbits.Center.Height = centerOrbitAimHeight;
            cineOrbit.Orbits.Center.Radius = centerOrbitAimRadius;
            cineOrbit.Orbits.Top.Height = topOrbitAimHeight;
            cineOrbit.Orbits.Top.Radius = topOrbitAimRadius;

            aimReticle.gameObject.SetActive(true);
            aiming = true;
            animator.SetBool("aim", true);
        }
        else
        {
            cineOrbit.Orbits.Center.Height = orbitDefaultHeight;
            cineOrbit.Orbits.Center.Radius = orbitDefaultRadius;
            cineCam.Follow = defaultTarget;
            aimReticle.gameObject.SetActive(false);

            aiming = false;
            animator.SetBool("aim", false);
        }

        attackCooldown += Time.deltaTime;

        if(aiming && Input.GetKeyDown(KeyCode.Mouse0) && attackCooldown > 0.25f && !dead && currentAmmo > 0)
        {
            currentAmmo--;

            attackCooldown = 0.0f;
            animator.SetTrigger("shoot");
            GameObject plasmaExplosion = Instantiate(plasmaExplosionInstance, MuzzleFlash.transform.position, Quaternion.identity);
            Destroy(plasmaExplosion, 2.0f);

            // Center of the screen
            Vector3 screenCenter = new Vector3(
                Screen.width / 2f,
                Screen.height / 2f,
                0f
            );

            // Create a ray from the camera through the screen center
            Ray ray = cam.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);
                GameObject plasmaExplosionHit = Instantiate(plasmaExplosionHitInstance, hit.point, Quaternion.identity);
                plasmaExplosion.transform.parent = MuzzleFlash.transform;
                Destroy(plasmaExplosionHit, 2.0f);

                GameObject hitObj = hit.collider.gameObject;

                if(hitObj.CompareTag("Mook"))
                {
                    Mook mook = hitObj.GetComponent<Mook>();
                    mook.Damage(0.25f);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentAmmo < ammo)
            Reload();
    }

    void Reload()
    {
        animator.SetTrigger("reload");
        currentAmmo = ammo;
        playerMovement.Freeze();
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
