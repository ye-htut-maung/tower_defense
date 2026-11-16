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
    [SerializeField] private Button upgrade;
    [SerializeField] private Button tentenChoice;
    [SerializeField] private Button sasukeChoice;
    public event Action<bool> ChoiceRequested;

    public event Action DeployRequested;
    public event Action SellRequested;
    public event Action ConfirmRequested;
    public event Action CancelRequested;
    public event Action UpgradeRequested;
    private bool showingEmpty;
    private bool showingHighestTier;
    private bool confirmingSale;
    private bool confirmingUpgrade;
    private bool supportsUpgrade;

    private void Awake()
    {
        deploy.onClick.AddListener(() => DeployRequested?.Invoke());
        sell.onClick.AddListener(() => SellRequested?.Invoke());
        confirm.onClick.AddListener(() => ConfirmRequested?.Invoke());
        cancel.onClick.AddListener(() => CancelRequested?.Invoke());
        upgrade.onClick.AddListener(() => UpgradeRequested?.Invoke());
        tentenChoice.onClick.AddListener(() => ChoiceRequested?.Invoke(false));
        sasukeChoice.onClick.AddListener(() => ChoiceRequested?.Invoke(true));
        Hide();
    }

    public void Show(string name, string details, bool empty, int cost, int refund, bool affordable,
        bool highestTier = false, bool canUpgrade = false, bool canUpgradeType = true, bool sasuke = false)
    {
        content.SetActive(true);
        showingEmpty = empty;
        showingHighestTier = highestTier;
        supportsUpgrade = canUpgradeType;
        tentenChoice.gameObject.SetActive(empty);
        sasukeChoice.gameObject.SetActive(empty);
        tentenChoice.interactable = sasuke;
        sasukeChoice.interactable = !sasuke;
        confirmingSale = confirmingUpgrade = false;
        confirm.interactable = true;
        title.text = name;
        description.text = details;
        price.text = empty ? $"{cost} Ryo" : !supportsUpgrade ? $"Sell refund: {refund} Ryo" : highestTier ? "Highest tier" :
            $"{RoundController.ChuninUpgradeCost} Ryo upgrade\n1 to 2 physical damage per hit";
        message.text = empty ? (!affordable ? "Not enough Ryo." : "") :
            highestTier || !supportsUpgrade ? "" : !canUpgrade ? "Not enough Ryo." : "";
        deploy.gameObject.SetActive(empty);
        deploy.interactable = affordable;
        sell.gameObject.SetActive(!empty);
        sell.GetComponentInChildren<Text>().text = $"Sell for {refund} Ryo";
        confirm.gameObject.SetActive(false);
        upgrade.gameObject.SetActive(!empty && !highestTier && supportsUpgrade);
        upgrade.interactable = canUpgrade;
    }

    public void AskToConfirmSale(int refund)
    {
        confirmingSale = true;
        confirmingUpgrade = false;
        confirm.interactable = true;
        price.text = $"Sell refund: {refund} Ryo";
        message.text = $"Sell this tower for {refund} Ryo?";
        deploy.gameObject.SetActive(false);
        sell.gameObject.SetActive(false);
        confirm.gameObject.SetActive(true);
        confirm.GetComponentInChildren<Text>().text = "Confirm Sale";
        upgrade.gameObject.SetActive(false);
    }

    public void AskToConfirmUpgrade()
    {
        confirmingUpgrade = true;
        confirmingSale = false;
        confirm.interactable = true;
        message.text = "Upgrade to Chunin Tenten?";
        deploy.gameObject.SetActive(false);
        sell.gameObject.SetActive(false);
        upgrade.gameObject.SetActive(false);
        confirm.gameObject.SetActive(true);
        confirm.GetComponentInChildren<Text>().text = "Confirm Upgrade";
    }

    public void Hide() => content.SetActive(false);

    public void UpdateAffordability(bool canDeploy, bool canUpgrade)
    {
        if (!content.activeSelf) return;
        deploy.interactable = canDeploy;
        upgrade.interactable = canUpgrade;
        confirm.interactable = !confirmingUpgrade || canUpgrade;
        if (confirmingSale) return; // Rewards must not dismiss a pending sale.
        if (confirmingUpgrade)
            message.text = canUpgrade ? "Upgrade to Chunin Tenten?" : "Not enough Ryo.";
        else
            message.text = (showingEmpty ? !canDeploy : supportsUpgrade && !showingHighestTier && !canUpgrade)
                ? "Not enough Ryo." : "";
    }
}
