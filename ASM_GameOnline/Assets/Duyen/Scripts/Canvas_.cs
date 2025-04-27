using UnityEngine;
using TMPro;
using System.Collections;

public class Canvas_ : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;  

    private void Start()
    {
        StartCoroutine(BlinkAndDisappear());
    }

    private IEnumerator BlinkAndDisappear()
    {
        float timer = 0f;

        while (timer < 15f)
        {
            textComponent.enabled = !textComponent.enabled;  // Đảo trạng thái của Text (hiện/ẩn)
            timer += 0.5f;  // Tạo hiệu ứng nhấp nháy mỗi 0.5 giây
            yield return new WaitForSeconds(0.5f);
        }

        textComponent.enabled = false;  // Sau 15 giây, ẩn chữ
    }
}
