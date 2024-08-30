using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelUpPanel : MonoBehaviour
{
    public static LevelUpPanel instance;
    [Header("Level Up UI")]
    [SerializeField] public GameObject ButtonUI;
    [SerializeField] public Button[] itemButtons;
    [SerializeField] public Image[] itemImage;
    [SerializeField] public Text[] itemTexts;
    [SerializeField] public Text[] descriptionTexts;
    [SerializeField] private Sprite[] itemSprites; //images

    private (string Name, string Description, float Value, Sprite Image)[] items;

    void Start()
    {
        items = new (string, string, float, Sprite)[]
        {
            ("Max Health", "Max HP increase by 5", 5, itemSprites[0] ),
            ("Speed", "Speed increase by 0.25", 0.25f, itemSprites[1]),
            ("Damage", "Damage increase by 5%", 0.05f, itemSprites[2]),
            ("Reload Speed", "Reloading speed increase by 10%", 0.05f, itemSprites[3]),
            ("Dash Cooldown", "Dash cooldown decrease by 5%", 0.05f, itemSprites[4]),
            ("Ammo Count", "Ammo Count increase by 5%", 0.05f, itemSprites[5])
        };
    }

    public IEnumerator UpdateLevelUpUI()
    {
        ButtonUI.SetActive(true);
        var selectedItems = items.OrderBy(x => Random.value).Take(4).ToArray();

        for (int i = 0; i < 3; i++)
        {
            if (i < selectedItems.Length)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemTexts[i].text = $"{selectedItems[i].Name}";
                string name = selectedItems[i].Name;
                float value = selectedItems[i].Value;
                descriptionTexts[i].text = $"{selectedItems[i].Description}";
                itemImage[i].sprite = selectedItems[i].Image;
                int index = i;
                itemButtons[i].onClick.RemoveAllListeners();

                itemButtons[i].onClick.AddListener(() => OnItemButtonClick(name, value));
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
        yield return new WaitForSecondsRealtime(1.2f);
    }

    void OnItemButtonClick(string Name, float Value)
    {
        Player.instance.levelUp(Name, Value);
    }
}
