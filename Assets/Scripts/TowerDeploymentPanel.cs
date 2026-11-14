using System;
using UnityEngine;
using UnityEngine.UI;

// Shared presentation and actions; tower identity/details are supplied by placement.
public class TowerDeploymentPanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text price;
    [SerializeField] private Text message;
    [SerializeField] private Button deploy;
    [SerializeField] private Button sell;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;

    public event Action DeployRequested;
    public event Action SellRequested;
    public event Action ConfirmRequested;
    public event Action CancelRequested;

    private void Awake()
    {
        deploy.onClick.AddListener(() => DeployRequested?.Invoke());
        sell.onClick.AddListener(() => SellRequested?.Invoke());
        confirm.onClick.AddListener(() => ConfirmRequested?.Invoke());
        cancel.onClick.AddListener(() => CancelRequested?.Invoke());
        Hide();
    }

    public void Show(string name, string details, bool empty, int cost, int refund, bool affordable)
    {
        content.SetActive(true);
        title.text = name;
        description.text = details;
        price.text = empty ? $"{cost} Ryo" : $"Deployment cost: {cost} Ryo";
        message.text = empty && !affordable ? "Not enough Ryo." : "";
        deploy.gameObject.SetActive(empty);
        deploy.interactable = affordable;
        sell.gameObject.SetActive(!empty);
        sell.GetComponentInChildren<Text>().text = $"Sell for {refund} Ryo";
        confirm.gameObject.SetActive(false);
    }

    public void AskToConfirmSale(int refund)
    {
        message.text = $"Sell this tower for {refund} Ryo?";
        deploy.gameObject.SetActive(false);
        sell.gameObject.SetActive(false);
        confirm.gameObject.SetActive(true);
    }

    public void Hide() => content.SetActive(false);
}
