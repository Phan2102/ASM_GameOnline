using TMPro;
using UnityEngine;

public class HealthSystem_Enemy : Health_Base
{
    protected override void InitHealthText()
    {
        Transform healthTextTransform = transform.Find("Health Text");
        if (healthTextTransform != null)
        {
            healthText = healthTextTransform.GetComponent<TextMeshPro>();
            healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
            Debug.Log("Tìm thấy Health Text trong prefab Enemy!");
        }
        else
        {
            GameObject textObject = new GameObject("Health Text");
            textObject.transform.SetParent(transform);
            textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
            healthText = textObject.AddComponent<TextMeshPro>();
            healthText.text = $"{MaxHealth}/{MaxHealth}";
            healthText.fontSize = 3;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.red;
            Debug.LogWarning("KHÔNG tìm thấy Health Text, đã tạo mới cho Enemy!");
        }
    }
}
