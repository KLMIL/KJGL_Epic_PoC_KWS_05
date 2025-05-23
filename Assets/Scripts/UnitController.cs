using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    public string unitName;
    public int maxHP = 100;
    public int currentHP;
    public GameObject hpBarPrefab;
    GameObject hpBarInstance;
    Slider hpSlider;
    public int attackDamage = 10;
    public float attackSpeed = 1f;
    public float attackRange = 2f;
    public bool isEnemy = false;
    public bool isAlive = true;

    public System.Action<UnitController, UnitController> _customAttackBehavior;


    private void Start()
    {
        currentHP = maxHP;

        // HP바 생성
        hpBarInstance = Instantiate(hpBarPrefab, transform);
        hpSlider = hpBarInstance.GetComponentInChildren<Slider>();
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
    }

    public void Attack(UnitController target)
    {
        if (target != null && target.isAlive)
        {
            if (_customAttackBehavior != null)
            {
                _customAttackBehavior(this, target);
            }
            else
            {
                target.TakeDamage(attackDamage);
                Debug.Log($"{unitName} attacked {target.unitName} for {attackDamage} damage");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        hpSlider.value = currentHP;
        if (currentHP <= 0)
        {
            currentHP = 0;
            isAlive = false;
            gameObject.SetActive(false);
        }
        Debug.Log($"{unitName} took {damage}, HP: {currentHP}");
    }
}
