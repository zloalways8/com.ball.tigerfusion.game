using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    public TextMeshProUGUI[] _score;
    public TextMeshProUGUI[] _level;
    public AudioSource bubblePopSound;
    public List<GameObject> bubblePrefabs;
    public Transform shooterPosition;
    public Transform bubbleParent;
    public float rowSpawnInterval = 5f;
    public GameObject gameOverUI;
    public GameObject gameGameUI;
    public GameObject gameWinUI;
    public TextMeshProUGUI scoreText;

    // Бонусы
    public int changeBubbleBonusUses = 2;
    public int stopFallBonusUses = 2;
    public TextMeshProUGUI changeBubbleBonusText;
    public TextMeshProUGUI stopFallBonusText;

    private List<List<GameObject>> bubbles = new List<List<GameObject>>();
    private GameObject currentBubble;
    private bool canShoot = true;
    private int score = 0;
    private int targetScore = 100;
    private bool stopFallActive = false;
    private bool isBonusButtonPressed;

    private void Start()
    {
        Time.timeScale = 1f;
        targetScore = PlayerPrefs.GetInt(EntityFormCoordinator.TemporalEngagementPhase, 0);
        targetScore *= 150;
        CreateRow(false);
        CreateRow(true);
        StartCoroutine(SpawnRows());
        SpawnBubbleForShooting();
        UpdateUI();
        UpdateBonusUI(); // Обновление UI для бонусов
        
    }


    void Update()
    {
        if (Input.GetMouseButtonUp(0) && canShoot && !isBonusButtonPressed)
        {
            ShootBubble();
        }
        RemoveFloatingBubblesFromNewest();
    }


    public void BlockShooting()
    {
        canShoot = false;
    }

    public void UnblockShooting()
    {
        canShoot = true;
    }


    void CreateRow(bool isOdd)
    {
        List<GameObject> row = new List<GameObject>();
        int count = isOdd ? 6 : 7; // Количество шаров в ряду
        float startX = -1.7f + (isOdd ? 0.5f : 0f); // Начальная позиция с небольшим смещением вправо

        float maxY = float.MinValue;
        foreach (var bubbleRow in bubbles)
        {
            foreach (var bubble in bubbleRow)
            {
                if (bubble != null && bubble.transform.position.y > maxY)
                {
                    maxY = bubble.transform.position.y;
                }
            }
        }

        float rowHeight = 0.5f; // Расстояние между рядами
        float rowY = maxY == float.MinValue ? bubbleParent.position.y : maxY + rowHeight;
        float bubbleSpacing = 0.55f; // Расстояние между пузырями в одном ряду

        for (int i = 0; i < count; i++)
        {
            Vector3 position = new Vector3(startX + i * bubbleSpacing, rowY, 0);
            GameObject bubble = Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Count)], position, Quaternion.identity);

            bubble.transform.SetParent(bubbleParent, true);
            bubble.transform.localPosition = new Vector3(bubble.transform.localPosition.x, bubble.transform.localPosition.y, 0);
            bubble.transform.localScale = new Vector3(1.308344f, 1.308344f, 1.308344f);

            row.Add(bubble);
        }

        bubbles.Add(row);
    }

    public void ActiveBall(bool stopball)
    {
        ChangeShootingBubble(0);
        stopFallActive = stopball;
    }
    IEnumerator SpawnRows()
    {
        while (true)
        {

            yield return new WaitForSeconds(rowSpawnInterval);
            if (!stopFallActive) // Проверка на активность бонуса
            {
                foreach (var bubbleRow in bubbles)
                {
                    foreach (var bubble in bubbleRow)
                    {
                        if (bubble != null)
                        {
                            bubble.transform.position += Vector3.down * 0.5f;
                        }
                    }
                }


                
                CreateRow(bubbles.Count % 2 == 0);
                
            }
            CheckLoseCondition();
        }
    }

    void SpawnBubbleForShooting()
    {
        if (currentBubble != null) return; // Защита от случайного спауна второго шара

        currentBubble = Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Count)], shooterPosition.position, Quaternion.identity);
        currentBubble.transform.SetParent(transform, false);
        currentBubble.transform.position = shooterPosition.position;
        currentBubble.transform.localScale = new Vector3(1.308344f, 1.308344f, 1.308344f);
    }


    void ShootBubble()
    {
        if (!_isActiveBall)
        {
            return;
        }
        if (currentBubble == null) return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        Vector3 direction = (worldPosition - shooterPosition.position).normalized;

        Rigidbody2D rb = currentBubble.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = direction * 200f;

        CollisionHandler collisionHandler = currentBubble.AddComponent<CollisionHandler>();
        collisionHandler.Initialize(this, bubbleParent, bubbles);

        currentBubble = null;
        canShoot = false;
        StartCoroutine(ReloadBubble());
    }

    IEnumerator ReloadBubble()
    {
        yield return new WaitForSeconds(1f);
        SpawnBubbleForShooting();
        canShoot = true;
    }

    public void RemoveFloatingBubblesFromNewest()
    {
        HashSet<GameObject> connectedBubbles = new HashSet<GameObject>();
        Queue<GameObject> queue = new Queue<GameObject>();

        // Добавляем в очередь все шары из последней строки (или только что добавленный шар)
        for (int colIndex = 0; colIndex < bubbles[bubbles.Count - 1].Count; colIndex++)
        {
            GameObject bubble = bubbles[bubbles.Count - 1][colIndex];
            if (bubble != null)
            {
                queue.Enqueue(bubble);
                connectedBubbles.Add(bubble);
            }
        }

        // BFS для поиска всех подключенных шаров
        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();
            Collider2D[] neighbors = Physics2D.OverlapCircleAll(current.transform.position, 0.4f); // Радиус проверки соседей

            foreach (var neighbor in neighbors)
            {
                if (neighbor.gameObject != null && !connectedBubbles.Contains(neighbor.gameObject) &&
                    (neighbor.CompareTag("BallPurple") || neighbor.CompareTag("BallGreen") ||
                     neighbor.CompareTag("BallBlue") || neighbor.CompareTag("BallRed")))
                {
                    connectedBubbles.Add(neighbor.gameObject);
                    queue.Enqueue(neighbor.gameObject);
                }
            }
        }

        // Удаляем шары, не подключенные к новой строке
        for (int rowIndex = 0; rowIndex < bubbles.Count; rowIndex++)
        {
            for (int colIndex = 0; colIndex < bubbles[rowIndex].Count; colIndex++)
            {
                GameObject bubble = bubbles[rowIndex][colIndex];
                if (bubble != null && !connectedBubbles.Contains(bubble))
                {
                    Destroy(bubble); // Удаляем сразу
                    bubbles[rowIndex][colIndex] = null; // Убираем ссылку из сетки
                }
            }
        }

        // Удаляем пустые строки
        bubbles.RemoveAll(row => row.All(b => b == null));
    }



    public void CheckMatches(GameObject bubble)
    {
        List<GameObject> matches = FindMatches(bubble);

        if (matches.Count >= 3)
        {
            foreach (var match in matches)
            {
                if (match != null)
                {
                    Destroy(match);
                    score += 10;

                    // Воспроизводим звук
                    
                        bubblePopSound.Play();
                   
                }
            }

            foreach (var row in bubbles)
            {
                row.RemoveAll(b => b == null);
            }

            RemoveFloatingBubblesFromNewest(); // Удаляем висящие шары сразу после удаления совпадений

            UpdateUI();

            if (score >= targetScore)
            {
                GameWin();
            }
        }
    }


    List<GameObject> FindMatches(GameObject bubble)
    {
        List<GameObject> matches = new List<GameObject> { bubble };
        string bubbleTag = bubble.tag;

        float searchRadius = 0.5f;
        Queue<GameObject> queue = new Queue<GameObject>();
        queue.Enqueue(bubble);
CheckLoseCondition();
        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();
            Collider2D[] neighbors = Physics2D.OverlapCircleAll(current.transform.position, searchRadius);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.CompareTag(bubbleTag) && !matches.Contains(neighbor.gameObject))
                {
                    matches.Add(neighbor.gameObject);
                    queue.Enqueue(neighbor.gameObject);
                }
            }
        }

        return matches;
    }

    void CheckLoseCondition()
    {
        foreach (var bubbleRow in bubbles)
        {
            foreach (var bubble in bubbleRow)
            {
                if (bubble != null)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(bubble.transform.position, 0.1f);
                    foreach (var collider in colliders)
                    {
                        if (collider.CompareTag("LoseBoundary"))
                        {
                            GameOver();
                            return;
                        }
                    }
                }
            }
        }
    }

    void GameOver()
    {
        foreach (var score in _score)
        {
            score.text = $"SCORE: {this.score}";
        }
        foreach (var level in _level)
        {
            level.text = $"LEVEL {PlayerPrefs.GetInt(EntityFormCoordinator.TemporalEngagementPhase, 0)}";
        }
        gameOverUI.SetActive(true);
        gameGameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        scoreText.text = $"SCORE: {score}/{targetScore}";
    }

    void GameWin()
    {
        FindObjectOfType<ApexScoreRegistrar>().SaveNewHighScore(score);
        foreach (var score in _score)
        {
            score.text = $"SCORE: {this.score}";
        }
        foreach (var level in _level)
        {
            level.text = $"LEVEL {PlayerPrefs.GetInt(EntityFormCoordinator.TemporalEngagementPhase, 0)}";
        }
        gameWinUI.SetActive(true);
        gameGameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Active(bool isActiveBall)
    {
        _isActiveBall = isActiveBall;

    }
    public bool _isActiveBall = true;
    // Реализация бонусов
    public void ChangeShootingBubble(int number)
    {
        changeBubbleBonusUses+=number;
        if (changeBubbleBonusUses > 0)
        {
            BlockShooting();
            isBonusButtonPressed = true; // Включаем защиту от выстрела

            if (currentBubble != null)
            {
                Destroy(currentBubble);
                currentBubble = null;
            }

            StartCoroutine(SpawnNewBubbleWithDelay());
            changeBubbleBonusUses--;
            UpdateBonusUI();
        }

        _isActiveBall = true;

    }

    private IEnumerator SpawnNewBubbleWithDelay()
    {
        
        yield return new WaitForEndOfFrame();

        SpawnBubbleForShooting();

        isBonusButtonPressed = false; // Разрешаем выстрел после завершения
        UnblockShooting();
    }

    public void StopFallBonus()
    {
        if (stopFallBonusUses > 0 && !stopFallActive)
        {
            BlockShooting(); // Блокируем стрельбу

            stopFallActive = true;
            StartCoroutine(StopFallTimer());

            stopFallBonusUses--;
            UpdateBonusUI();

            UnblockShooting(); // Разблокировать после использования бонус
        }
        _isActiveBall = true;
    }


    IEnumerator StopFallTimer()
    {
        float timeLeft = 10f;

        while (timeLeft > 0f)
        {
            // Уменьшаем время и обновляем UI
            timeLeft -= Time.deltaTime;
            stopFallBonusText.text = $"{Mathf.Ceil(timeLeft)}";

            yield return null;
        }

        stopFallActive = false;
        stopFallBonusText.text = $"{stopFallBonusUses}"; // После окончания бонуса показываем оставшееся количество
        
    }

    void UpdateBonusUI()
    {
        changeBubbleBonusText.text = $"{changeBubbleBonusUses}";
        stopFallBonusText.text = $"{stopFallBonusUses}";
    }
}

public class CollisionHandler : MonoBehaviour
{
    private BubbleShooter shooter;
    private Transform bubbleParent;
    private List<List<GameObject>> bubbles;
    private bool isAttached = false;

    public void Initialize(BubbleShooter shooterScript, Transform parent, List<List<GameObject>> bubbleRows)
    {
        shooter = shooterScript;
        bubbleParent = parent;
        bubbles = bubbleRows;
        StartCoroutine(SelfDestruct());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttached && (collision.gameObject.CompareTag("BallPurple") || collision.gameObject.CompareTag("BallGreen") ||
                            collision.gameObject.CompareTag("BallBlue") || collision.gameObject.CompareTag("BallRed")))
        {
            if (collision.gameObject == null) return; // Проверка на null перед взаимодействием

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                Destroy(rb);
            }

            transform.SetParent(bubbleParent, true);
            AlignToClosestBubble();
            AddToBubbleGrid();

            shooter.CheckMatches(gameObject);
            isAttached = true;
            StopAllCoroutines();
        }
    }

    void AlignToClosestBubble()
    {
        float closestDistance = float.MaxValue;
        GameObject closestBubble = null;

        foreach (var row in bubbles)
        {
            foreach (var bubble in row)
            {
                if (bubble == null) continue;

                float distance = Vector2.Distance(transform.position, bubble.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBubble = bubble;
                }
            }
        }

        if (closestBubble != null)
        {
            transform.position = closestBubble.transform.position + (transform.position - closestBubble.transform.position).normalized * 0.55f;
        }
    }

    void AddToBubbleGrid()
    {
        // Ищем ближайшую строку пузырей по Y
        float closestRowY = float.MaxValue;
        List<GameObject> closestRow = null;

        foreach (var row in bubbles)
        {
            if (row.Count > 0)
            {
                // Проверяем, существует ли хотя бы один объект в строке перед доступом к его позиции
                if (row[0] != null)
                {
                    float rowY = row[0].transform.position.y; // Предполагаем, что вся строка на одном уровне по Y
                    if (Mathf.Abs(rowY - transform.position.y) < Mathf.Abs(closestRowY - transform.position.y))
                    {
                        closestRowY = rowY;
                        closestRow = row;
                    }
                }
            }
        }

        if (closestRow != null)
        {
            closestRow.Add(gameObject);
        }
        else
        {
            // Если строки нет, создаём новую
            List<GameObject> newRow = new List<GameObject> { gameObject };
            bubbles.Add(newRow);
        }
        shooter.RemoveFloatingBubblesFromNewest();
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);

        // Если шар не был прикреплен, удаляем его
        if (!isAttached && gameObject != null)
        {
            Destroy(gameObject);
        }
        else
        {
            // Проверяем, не висит ли он в воздухе
            if (IsBubbleFloating() && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
    private bool IsBallTag(string tag)
    {
        return tag == "BallPurple" || tag == "BallGreen" || tag == "BallBlue" || tag == "BallRed";
    }
    // Метод, который проверяет, не висит ли шар в воздухе
    bool IsBubbleFloating()
    {
        // Радиус проверки можно подкорректировать в зависимости от расстояния между шарами
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.6f);

        foreach (var collider in colliders)
        {
            if (collider != null && IsBallTag(collider.tag))
            {
                return false; // Нашли поддержку
            }
        }

        return true; // Поддержки нет, шар "висит"
    }

}
