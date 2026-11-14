using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Transform towerPads;
    [SerializeField] private TentenTower towerPrefab;
    [SerializeField] private RoundController round;

    private readonly HashSet<Transform> occupiedPads = new HashSet<Transform>();
    private InputAction placeAction;

    public void ResetPlacement()
    {
        occupiedPads.Clear();
    }

    private void OnEnable()
    {
        occupiedPads.Clear();
        placeAction = new InputAction("Place Tenten", InputActionType.Button, "<Mouse>/leftButton");
        placeAction.performed += OnPlace;
        placeAction.Enable();
    }

    private void OnDisable()
    {
        if (placeAction != null)
        {
            placeAction.performed -= OnPlace;
            placeAction.Dispose();
            placeAction = null;
        }
    }

    private void OnPlace(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse mouse)
        {
            PlaceAtScreenPosition(mouse.position.ReadValue());
        }
    }

    private void PlaceAtScreenPosition(Vector2 screenPosition)
    {
        if ((round != null && round.HasEnded) || gameCamera == null || towerPads == null || towerPrefab == null ||
            !Physics.Raycast(gameCamera.ScreenPointToRay(screenPosition), out RaycastHit hit))
        {
            return;
        }

        Transform pad = hit.transform;
        while (pad != null && pad.parent != towerPads)
        {
            pad = pad.parent;
        }

        if (pad == null || occupiedPads.Contains(pad) || pad.GetComponentInChildren<TentenTower>() != null)
        {
            return;
        }

        // Flattened cylinder colliders can extend above the visible mesh.
        float top = pad.GetComponent<Renderer>().bounds.max.y;
        foreach (Collider surface in pad.GetComponentsInChildren<Collider>())
        {
            Renderer visibleSurface = surface.GetComponent<Renderer>();
            if (visibleSurface != null)
            {
                top = Mathf.Max(top, visibleSurface.bounds.max.y);
            }
        }

        Vector3 position = new Vector3(pad.position.x, top, pad.position.z);
        TentenTower tower = Instantiate(towerPrefab, position, Quaternion.identity);
        tower.transform.SetParent(pad, true);
        occupiedPads.Add(pad);
    }
}
