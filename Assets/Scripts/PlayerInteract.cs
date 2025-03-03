using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public TextMeshProUGUI animalFact;
    public GameObject popUpPanel;    
    public float interactRange = 2f;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
            bool interacted = false;
            Cursor.lockState = CursorLockMode.None;
            foreach (var hitCollider in hitColliders) {
                if (hitCollider.GetComponent<AnimalInteractable>() != null) {
                    hitCollider.GetComponent<AnimalInteractable>().Interact();
                    popUpPanel.SetActive(true); // Show pop-up when interaction happens
                    interacted = true;
                    animalFact.text = hitCollider.GetComponent<AnimalInteractable>().animalText;

                    break;  // Stop after first interaction
                }
            }

            if (!interacted) {
                popUpPanel.SetActive(false); // Hide pop-up if no interaction
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
