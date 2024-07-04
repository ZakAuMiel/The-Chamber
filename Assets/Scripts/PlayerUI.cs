using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthbarFill;

    private Player player;
    private PlayerController controller;
    private WeaponManager weaponManager;

    [SerializeField]
    private Text ammoText;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreboard;

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        PauseMenu.isOn = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        SetHealthAmount(player.GetHealthPct());
        SetAmmoAmount(weaponManager.currentMagazineSize);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;

        if (PauseMenu.isOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void SetHealthAmount(float _amount)
    {
        healthbarFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }
}
