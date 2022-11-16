using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerCanva : MonoBehaviour
{
    public Color friendlyColor;

    public Color enemyColor;

    public Slider healthbar;

    public Image icon;

    public Image healbarColor;

    public Image iconColor;

    Transform cam;

    public Image glow;

    public GameObject damagePrefab;

    public void SetUp(Character character, int player)
    {
        healthbar.maxValue = character.maxHealth;
        healthbar.value = character.maxHealth;
        icon.sprite = character.characterIcon;
        cam = Camera.main.transform;
    }
    public void ChangeColor()
    {
        healbarColor.color = enemyColor;
        iconColor.color = enemyColor;
        glow.color = enemyColor;
    }

    public void SmoothSync(float health)
    {
        StopAllCoroutines();
        StartCoroutine(Damage(health));
    }
    public void ResetHealth(int health)
    {
        healthbar.value = health;
    }
    public void SpawnDamage(int damage)
    {
        GameObject obj = Instantiate(damagePrefab);
        obj.transform.SetParent(transform);
        obj.GetComponent<TMP_Text>().text = "- " + (int)(damage);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.forward = transform.forward;
        Destroy(obj, 1);
    }
    private void Update()
    {
        if (cam)
            transform.LookAt(cam);
    }

    public IEnumerator Damage(float health)
    {
        float x = health;
        if (healthbar.value > health)
        
            while (healthbar.value > health + 0.1f)
            {
                healthbar.value = Math.dampFloat(healthbar.value, health, 2f, Time.deltaTime);
                yield return null;
            }
        else if(healthbar.value < health)
            while (healthbar.value < health - 0.1f)
            {
                healthbar.value = Math.dampFloat(healthbar.value, health, 2f, Time.deltaTime);
                yield return null;
            }
        StopAllCoroutines();
    }
}
