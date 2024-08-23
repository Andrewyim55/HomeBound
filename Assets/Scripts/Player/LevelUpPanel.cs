using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelUpPanel : MonoBehaviour
{
    [Header("Level Up UI")]
    [SerializeField] public GameObject LevelUpUI;
    [SerializeField] public Button[] itemButtons;
    [SerializeField] public Text[] itemTexts;
    [SerializeField] public Text[] descriptionTexts;
    [SerializeField] private GameObject PlayerScreen;

    private Player playerScript;
    private (string Name, string Description, float Value)[] items;

    void Start()
    {
        playerScript = PlayerScreen.GetComponent<Player>();
        items = new (string, string, float)[]
        {
            ("Max Health", "Max HP increase by 5", 5),
            ("Speed", "Speed increase by 0.25", 0.25f),
            ("Damage", "Damage increase by 5%", 0.05f),
            ("Reload Speed", "Reloading speed increase by 10%", 0.05f),
            ("Dash Cooldown", "Dash cooldown decrease by 5%", 0.05f),
            ("Ammo Count", "Ammo Count increase by 5%", 0.05f)
        };
    }

    public void UpdateLevelUpUI()
    {
        LevelUpUI.SetActive(true);
        var selectedItems = items.OrderBy(x => Random.value).Take(3).ToArray();

        for (int i = 0; i < 3; i++)
        {
            if (i < selectedItems.Length)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemTexts[i].text = $"{selectedItems[i].Name}";
                string name = selectedItems[i].Name;
                float value = selectedItems[i].Value;
                descriptionTexts[i].text = $"{selectedItems[i].Description}";
                int index = i;
                itemButtons[i].onClick.RemoveAllListeners();
                
                itemButtons[i].onClick.AddListener(() => OnItemButtonClick(name, value));
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnItemButtonClick(string Name, float Value)
    {
        playerScript.levelUp(Name, Value);
    }
}
