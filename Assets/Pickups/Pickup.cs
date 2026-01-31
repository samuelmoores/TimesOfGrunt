using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Pickup : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject textObject;
    [SerializeField] PlayerInteraction playerInteraction;
    bool pickedUp = false;
    float bobTime = 0.5f;
    float timer;
    float bobDirection = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textObject.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInteraction.FoundWeapon())
        {
            playerInteraction.SetWeapon(gameObject);
            gameObject.transform.localRotation = Quaternion.identity;
            pickedUp = true;
            textObject.SetActive(false);
        }

        if (!pickedUp)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);

            timer += Time.deltaTime;

            if(timer > bobTime)
            {
                bobDirection *= -1.0f;
                timer = 0.0f;
            }

            transform.Translate(Vector3.up * timer * bobDirection * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !pickedUp)
        {
            textObject.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !pickedUp)
        {
            textObject.gameObject.SetActive(false);
        }
    }
}
