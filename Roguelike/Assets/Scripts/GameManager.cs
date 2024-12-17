using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BoardManager boardManager;  
    public PlayerController playerController;
    public TurnManager turnManager { get; private set; }

    private int m_FoodAmount = 50;

    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

    public int currentLevel = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        turnManager = new TurnManager();
        turnManager.OnTick += OnTurnHappen;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
       
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        //boardManager.Init();
        //playerController.Spawn(boardManager, new Vector2Int(1, 1));



        StartNewGame();
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Comida: " + m_FoodAmount;

        if(m_FoodAmount <= 0)
        {
            playerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!! \n\nHas avanzado a traves de " + (currentLevel - 1) + " niveles";
        }
    }

    public void NewLevel()
    {
        boardManager.DeleteMap();
        boardManager.Init();
        playerController.Spawn(boardManager, new Vector2Int(1, 1));

        currentLevel++;
    }

    public void StartNewGame()
    {
        

        m_GameOverPanel.style.visibility = Visibility.Hidden;
        currentLevel = 1;
        m_FoodAmount = 50;
        m_FoodLabel.text = "Comida: " + m_FoodAmount;

        boardManager.DeleteMap();
        boardManager.Init();
        playerController.Init();
        playerController.Spawn(boardManager, new Vector2Int (1, 1));
    }
}
