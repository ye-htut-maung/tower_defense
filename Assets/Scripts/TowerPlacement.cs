using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Transform towerPads;
    [SerializeField] private TentenTower towerPrefab;
    [SerializeField] private RoundController round;
    [SerializeField] private TowerDeploymentPanel panel;
    [SerializeField] private Material previewMaterial;
    [SerializeField] private string towerName = "Genin Tenten";
    [SerializeField] private string damageDescription = "Ranged physical damage";

    public const int SellRefund = 35;
    private enum SelectionMode { None, Empty, Occupied, ConfirmSale }
    private SelectionMode mode;
    private Transform selectedPad;
    private TentenTower selectedTower;
    private InputAction placeAction;
    private LineRenderer rangePreview;
    private LineRenderer selectionHighlight;
    private readonly List<RaycastResult> uiHits = new List<RaycastResult>();
    private bool CanManage => round != null && round.State == RoundController.RoundState.Ready;

    public void ResetPlacement() => ClearSelection();

    private void OnEnable()
    {
        panel.DeployRequested += Deploy;
        panel.SellRequested += AskToSell;
        panel.ConfirmRequested += ConfirmSale;
        panel.CancelRequested += ClearSelection;
        placeAction = new InputAction("Select tower pad", InputActionType.Button, "<Mouse>/leftButton");
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
        panel.DeployRequested -= Deploy;
        panel.SellRequested -= AskToSell;
        panel.ConfirmRequested -= ConfirmSale;
        panel.CancelRequested -= ClearSelection;
        ClearSelection();
    }

    private void OnPlace(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse mouse)
            PlaceAtScreenPosition(mouse.position.ReadValue());
    }

    // Retained as the screen-raycast entry point; selecting never spends currency.
    private void PlaceAtScreenPosition(Vector2 screenPosition)
    {
        if (!CanManage || gameCamera == null || towerPads == null || towerPrefab == null) return;
        // Raycast UI explicitly: IsPointerOverGameObject is stale during input callbacks.
        if (EventSystem.current != null)
        {
            uiHits.Clear();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
                { position = screenPosition }, uiHits);
            if (uiHits.Count > 0) return;
        }
        if (!Physics.Raycast(gameCamera.ScreenPointToRay(screenPosition), out RaycastHit hit)) return;
        Transform pad = hit.transform;
        while (pad != null && pad.parent != towerPads) pad = pad.parent;
        if (pad == null) return;
        selectedPad = pad;
        selectedTower = pad.GetComponentInChildren<TentenTower>();
        mode = selectedTower == null ? SelectionMode.Empty : SelectionMode.Occupied;
        panel.Show(towerName, damageDescription, mode == SelectionMode.Empty,
            RoundController.TentenCost, SellRefund, round.CanAffordTenten);
        DrawCircle(ref selectionHighlight, "Selected Pad", pad.position, 1.05f,
            new Color(1f, .8f, .2f), .08f);
        DrawCircle(ref rangePreview, "Attack Range Preview", pad.position,
            selectedTower != null ? selectedTower.AttackRange : towerPrefab.AttackRange,
            new Color(.3f, 1f, .85f), .045f);
    }

    private void DrawCircle(ref LineRenderer line, string name, Vector3 center, float radius,
        Color color, float width)
    {
        if (line == null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = previewMaterial;
            line.useWorldSpace = true;
            line.loop = true;
            line.positionCount = 96;
        }
        line.startColor = line.endColor = color;
        line.startWidth = line.endWidth = width;
        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = i * Mathf.PI * 2f / line.positionCount;
            line.SetPosition(i, new Vector3(center.x + Mathf.Cos(angle) * radius, .31f,
                center.z + Mathf.Sin(angle) * radius));
        }
        line.gameObject.SetActive(true);
    }

    private void Deploy()
    {
        if (!CanManage || mode != SelectionMode.Empty || selectedPad == null ||
            selectedPad.GetComponentInChildren<TentenTower>() != null) return;
        if (!round.CanAffordTenten)
        {
            panel.Show(towerName, damageDescription, true, RoundController.TentenCost,
                SellRefund, false);
            return;
        }
        float top = selectedPad.GetComponent<Renderer>().bounds.max.y;
        foreach (Collider surface in selectedPad.GetComponentsInChildren<Collider>())
        {
            Renderer mesh = surface.GetComponent<Renderer>();
            if (mesh != null) top = Mathf.Max(top, mesh.bounds.max.y);
        }
        TentenTower tower = Instantiate(towerPrefab,
            new Vector3(selectedPad.position.x, top, selectedPad.position.z), Quaternion.identity);
        tower.transform.SetParent(selectedPad, true);
        if (!round.TryPayForTenten())
        {
            tower.gameObject.SetActive(false);
            Destroy(tower.gameObject);
        }
        ClearSelection();
    }

    private void AskToSell()
    {
        if (!CanManage || mode != SelectionMode.Occupied || selectedTower == null) return;
        mode = SelectionMode.ConfirmSale;
        panel.AskToConfirmSale(SellRefund);
    }

    private void ConfirmSale()
    {
        if (!CanManage || mode != SelectionMode.ConfirmSale || selectedTower == null) return;
        TentenTower sold = selectedTower;
        ClearSelection(); // Consume confirmation before removing/refunding the tower.
        sold.gameObject.SetActive(false);
        Destroy(sold.gameObject);
        round.RefundTentenSale();
    }

    public void ClearSelection()
    {
        mode = SelectionMode.None;
        selectedPad = null;
        selectedTower = null;
        if (panel != null) panel.Hide();
        if (rangePreview != null) rangePreview.gameObject.SetActive(false);
        if (selectionHighlight != null) selectionHighlight.gameObject.SetActive(false);
    }
}
