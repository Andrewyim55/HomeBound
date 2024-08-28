using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public Text healthText;
    [SerializeField] public Image weaponDisplay;
    [SerializeField] public Image cooldownImage;
    [SerializeField] public GameObject deathScreenUI;
    [SerializeField] public GameObject LevelUpUI;
    [SerializeField] public Text timerText;
    [SerializeField] public Image healthBarImage;
    [SerializeField] public Text ammoCount;
    [SerializeField] public Animator skillCDAnimator;


    // Start is called before the first frame update
    void Start()
    {
        Player.instance.healthText = healthText;
        Player.instance.weaponDisplay = weaponDisplay;
        Player.instance.cooldownImage = cooldownImage;
        Player.instance.deathScreenUI = deathScreenUI;
        Player.instance.LevelUpUI = LevelUpUI;
        Player.instance.timerText = timerText;
        Player.instance.healthBarImage = healthBarImage;
        Player.instance.ammoCount = ammoCount;
        Player.instance.gameObject.GetComponent<SkillCD>().CooldownImage = cooldownImage;
        Player.instance.skillCDAnimator = cooldownImage.GetComponent<Animator>();
        Player.instance.UpdateHealthBar();
    }
}
