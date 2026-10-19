using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 4f;
    public LayerMask layersToIgnore;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance, ~layersToIgnore))
        {
            GeneratorItem generator = hit.collider.GetComponent<GeneratorItem>();

            if (generator != null)
            {
                generator.ActivateGenerator();
            }
        }
            
    }
}
