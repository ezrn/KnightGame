using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactMask; //Interactable

    [Header("References")]
    [SerializeField] private Transform playerRoot; // "Player" object
    [SerializeField] private PlayerStats playerStats;

    void Update() // Creates a sphere that checks for nearby objects with the Interactable script and layer
    {
        if (!Input.GetKeyDown(interactKey)) return;

        Collider[] hits = Physics.OverlapSphere(
            playerRoot.position,
            interactRange,
            interactMask,
            QueryTriggerInteraction.Collide);

        if (hits.Length == 0) return;

        Collider nearest = null;
        float bestSqr = float.MaxValue;
        foreach (var col in hits)
        {
            float sqr = (col.transform.position - playerRoot.position).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; nearest = col; }
        }

        // only picks nearest one
        if (nearest != null &&
            nearest.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact(playerStats);
        }
    }
}
