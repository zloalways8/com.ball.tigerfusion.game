using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    private Transform spawnPoint;   // Точка спауна шарика
    public Image arrow; 
    private Vector2 shootDirection; // Направление выстрела
    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            arrow.gameObject.SetActive(true);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shootDirection = (mousePos - (Vector2)transform.position).normalized;

            // Обновляем направление стрелки
            UpdateArrowDirection(shootDirection); // Передаём направление, чтобы стрелка указывала правильно
        }

        if (Input.GetMouseButtonUp(0))
        {
            arrow.gameObject.SetActive(false);
        }
    }
    void UpdateArrowDirection(Vector2 direction)
    {
        // Вычисляем угол для поворота стрелки
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Угол в радианах
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // Поворачиваем на -90 градусов

        // Обновляем позицию стрелки (если необходимо)
        if (spawnPoint != null)
        {
            arrow.transform.position = spawnPoint.position; // Стрелка следует за точкой спауна
        }
    }
}
