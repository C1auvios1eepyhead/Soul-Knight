using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using  UnityEngine.SceneManagement;
public class MenuManager : Singleton<MenuManager>
{   
    [Header("Config")]
    [SerializeField] private PlayerCreation[] players;

    [Header("UI")]
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Image playerIcon;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerHealthMaxStat;
    [SerializeField] private TextMeshProUGUI playerArmorMaxStat;


    private SelectablePlayer currentPlayer; 

    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer(){
        for(int i = 0; i < players.Length; i++){
        
            PlayerMovement player = Instantiate(players[i].Player, players[i].CreationPos.position, Quaternion.identity, players[i].CreationPos);
            player.enabled = false;
        }
    }
    public void ClickPlayer(SelectablePlayer selectablePlayer){
        startButton.SetActive(false);
        if(currentPlayer != null){
            currentPlayer.GetComponent<PlayerMovement>().enabled = false;
        }
        currentPlayer = selectablePlayer;
        showPlayerStats();
    }

    public void SelectPlayer(){
        currentPlayer.GetComponent<PlayerMovement>().enabled = true;
        PlayerSelectionManager.Instance.SelectedPlayer = currentPlayer.Config.playerType;
        PlayerSelectionManager.Instance.Player =
        currentPlayer.Config;

        currentPlayer.Config.ResetPlayerStats();
        ClosePlayerPanel();
        ShowStartButton();
    }
     
    public void StartButton(){
        SceneManager.LoadScene("Game");
    }

    private void showPlayerStats(){
        playerPanel.SetActive(true);
        playerIcon.sprite = currentPlayer.Config.Icon;
        playerName.text = currentPlayer.Config.Name;
        playerLevel.text = $"Level {currentPlayer.Config.Level}";
        playerHealthMaxStat.text = currentPlayer.Config.MaxHealth.ToString();
        playerArmorMaxStat.text = currentPlayer.Config.MaxArmor.ToString();
    }

    public void ClosePlayerPanel(){
        playerPanel.SetActive(false);
    }

    public void ShowStartButton(){
        startButton.SetActive(true);
    }
}

[Serializable]
public class PlayerCreation{
    public PlayerMovement Player;
    public Transform CreationPos;

}