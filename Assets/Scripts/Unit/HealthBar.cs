using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthbarSprite;
    [SerializeField] private Camera camera;
    [SerializeField] float target = 1;
    [SerializeField] private float reduceSpeed = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        target = currentHealth / maxHealth;
    }
}
