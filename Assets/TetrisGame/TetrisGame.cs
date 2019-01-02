using UnityEngine;

public class TetrisGame : MonoBehaviour, FigurePolygonListener
{
    public Borders TetrisBorders;
    public NextFigureArea TetrisNextFigureArea;
    public Transform FiguresParent;
    public GameObject GameUI;
    public GameObject GamePauseUI;
    public GameObject GameOverUI;

    // Constants
    const int kScreenResolutionWidth            = 800;
    const int kScreenResolutionHeight           = 600;
    const string kStartPanelTitle               = "TETRIS GAME";
    const float kFigureActionInterval           = 0.02f; // interval for figure control key events handling (in seconds)
    const float kMoveVelocityIncreasing         = 6 * kFigureActionInterval; // value for increasing figure moving velocity every kFigureActionInterval seconds
    const float kSpeedUpVelocityIncreasing      = 10 * kFigureActionInterval; // value for increasing figure speed up velocity every kFigureActionInterval seconds

    const string kBottomBorderName              = "BottomBorder";
    const string kGameUI_Scores                 = "ScoresValue";
    const string kGamePauseUI_Title             = "Title";
    const string kGameOverUI_Scores             = "FinalScores";

    Transform currentFigure;
    Transform nextFigure;
    int scores = 0;

    bool isGameStarted = false;
    bool isGamePaused = false;

    float lastMoveActionTime = 0;
    CollisionsList collisionList = new CollisionsList();

    FigureCreator figureCreator;
    FigurePusher figurePusher;
    NextFigurePusher nextFigurePusher;

    // Use this for initialization
    void Start()
    {
        figureCreator = new FigureCreator();
        figurePusher = new FigurePusher(TetrisBorders, FiguresParent);
        nextFigurePusher = new NextFigurePusher(TetrisNextFigureArea);

        Screen.SetResolution(kScreenResolutionWidth, kScreenResolutionHeight, false);
    }

    void FixedUpdate()
    {
        collisionList.Clear();
    }

    void Update()
    {
        // Show info at the beginning of the game
        if (!isGameStarted)
            PauseGame(kStartPanelTitle);

        UpdateKeyEvents();
    }

    void StartGame()
    {
        isGameStarted = true;
        NextFigure();
    }

    void UpdateKeyEvents()
    {
        // Escape button
        if (Input.GetKeyUp("escape"))
        {
            Application.Quit();
        }

        // Pause/Resume Button
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame("PAUSE");
        }

        // Figure control buttons
        if ( !isGamePaused && Time.realtimeSinceStartup - lastMoveActionTime >= kFigureActionInterval )
        {
            bool leftArrow = Input.GetKey(KeyCode.LeftArrow),
                rightArrow = Input.GetKey(KeyCode.RightArrow);

            if ((leftArrow || rightArrow))
            {
                // Move figure to the left or to the right
                if (currentFigure)
                {
                    Rigidbody2D rigidbody2D = currentFigure.GetComponent<Rigidbody2D>();
                    Debug.Assert(rigidbody2D);

                    float addedVelocity = kMoveVelocityIncreasing;
                    if (leftArrow)
                        addedVelocity = -addedVelocity;
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + addedVelocity, rigidbody2D.velocity.y);
                }

                lastMoveActionTime = Time.realtimeSinceStartup;
            }

            bool downArrow = Input.GetKey(KeyCode.DownArrow);
            if (downArrow)
            {
                // Speed up figure
                if (currentFigure)
                {
                    Rigidbody2D rigidbody2D = currentFigure.GetComponent<Rigidbody2D>();
                    Debug.Assert(rigidbody2D);

                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y - kSpeedUpVelocityIncreasing);
                }
            }
        }
    }

    void FigurePolygonListener.OnCollision(Transform object1, Transform object2)
    {
        // When collision happened, both figure polygons call OnCollision, but we must handle collision only once, so check it for prevent handling twice
        if (!collisionList.HasCollision(object1, object2))
        {
            collisionList.AddCollision(object1, object2);
        }
        else
            return;
        
        // one of the objects is a border
        if (TetrisBorders.IsBorder(object1) || TetrisBorders.IsBorder(object2))
        {
            // other object is figure
            if (IsFigure(object1) || IsFigure(object2))
            {
                if (IsFigure(object1))
                    CollisionFigureBorder(object1, object2);
                else
                    CollisionFigureBorder(object2, object1);
            }
        }
        // both objects are figures
        else if (IsFigure(object1) && IsFigure(object2))
        {
            CollisionFigureFigure(object1, object2);
        }
    }

    private void CollisionFigureBorder(Transform figure, Transform border)
    {
        // if active figure has reached bottom border, push next figure
        if (figure == currentFigure && border == TetrisBorders.Bottom)
        {
            NextFigure();
        }
    }

    private void CollisionFigureFigure(Transform figure1, Transform figure2)
    {
        FigureDataComponent figureDataStorage1 = figure1.GetComponent<FigureDataComponent>();
        FigureDataComponent figureDataStorage2 = figure2.GetComponent<FigureDataComponent>();

        if (!figureDataStorage1 || !figureDataStorage2)
        {
            Debug.Assert(figureDataStorage1 && figureDataStorage2);
            return;
        }

        if (figureDataStorage1.Data.Color == figureDataStorage2.Data.Color)
        {
            CollisionFigureFigureSameColor(figure1, figure2);
        }

        if (figure1 == currentFigure || figure2 == currentFigure)
        {
            NextFigure();
        }
    }

    private void CollisionFigureFigureSameColor(Transform figure1, Transform figure2)
    {
        // If two figures with same color has collided, destroy both
        Destroy(figure1.gameObject);
        Destroy(figure2.gameObject);

        if (!IsGameOver())
            SetScores(scores + 1);
    }

    private void PauseGame( string titleText )
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;

            if (GamePauseUI)
            {
                GamePauseUI.SetActive(true);
                Component title = GamePauseUI.transform.Find(kGamePauseUI_Title);
                if (title)
                {
                    UnityEngine.UI.Text uiTitle = title.gameObject.GetComponent<UnityEngine.UI.Text>();
                    uiTitle.text = titleText;
                }
            }
            if (GameUI)
                GameUI.SetActive(false);

            isGamePaused = true;
        }
    }

    private void ResumeGame()
    {
        if (isGamePaused)
        {
            if (!isGameStarted)
                StartGame();

            Time.timeScale = 1;

            if (GamePauseUI)
                GamePauseUI.SetActive(false);
            if (GameUI)
                GameUI.SetActive(true);

            isGamePaused = false;
        }
    }

    private void GameOver()
    {
        currentFigure = null;
        
        if (GameOverUI)
        {
            GameOverUI.SetActive(true);
            Component uiScores = GameOverUI.transform.Find(kGameOverUI_Scores);
            if(uiScores)
            {
                UnityEngine.UI.Text uiScoresText = uiScores.GetComponent<UnityEngine.UI.Text>();
                uiScoresText.text = scores.ToString();
            }
        }
        if (GameUI)
            GameUI.SetActive(false);
    }

    private bool IsGameOver()
    {
        return currentFigure == null;
    }

    private bool IsFigure(Transform obj)
    {
        return obj && obj.parent == FiguresParent;
    }

    private void SetScores(int _scores)
    {
        scores = _scores;

        if (GameUI)
        {
            Component uiScores = GameUI.transform.Find(kGameUI_Scores);
            if (uiScores)
            {
                UnityEngine.UI.Text uiScoresText = uiScores.GetComponent<UnityEngine.UI.Text>();
                uiScoresText.text = scores.ToString();
            }
        }
    }

    private void NextFigure()
    {
        FigureData newFigureData;
        if (nextFigure == null)
        {
            newFigureData = figureCreator.GenerateFigure();
        }
        else
        {
            // take figure data from the next figure
            FigureDataComponent figureDataStorage = nextFigure.GetComponent<FigureDataComponent>();
            newFigureData = figureDataStorage.Data;
        }

        currentFigure = figurePusher.Push(newFigureData); // returns null if can't push figure

        if (currentFigure)
        {
            FigurePolygon polygon = currentFigure.GetComponent<FigurePolygon>();
            polygon.AddListener(this);
        }
        else
        {
            // can't push figure, so the game is over
            GameOver();
        }

        if (nextFigure)
        {
            Destroy(nextFigure.gameObject); // remove previous figure from next figure area
            nextFigure = null;
        }
        
        // generate next figure for displaying
        FigureData nextFigureData = figureCreator.GenerateFigure();
        nextFigure = nextFigurePusher.Push(nextFigureData);
    }
}
