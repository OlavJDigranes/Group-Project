using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    struct Status
    {
        //string _name;
        int _health;
        int _stamina;
        int _mana;
        int _gold;
        int _lastDamage;
        List<GameObject> _inventory;

        public Status(int Health, int Stamina, int Mana, int Gold, List<GameObject> Inventory)
        {
            _health = Health;
            _stamina = Stamina;
            _mana = Mana;
            _gold = Gold;
            _lastDamage = 0;
            _inventory = Inventory;
        }

        public void SetHealth(int Health)
        {
            _health = Health;
        }

        public int GetHealth()
        {
            return _health;
        }

        public void SetStamina(int Stamina)
        {
            _stamina = Stamina;
        }

        public int GetStamina()
        {
            return _stamina;
        }

        public void SetMana(int Mana)
        {
            _mana = Mana;
        }

        public int GetMana()
        {
            return _mana;
        }

        public void SetGold(int Gold)
        {
            _gold = Gold;
        }

        public void AddGold(int GoldToAdd)
        {
            _gold += GoldToAdd;
        }

        public void RemoveGold(int GoldToRemove)
        {
            _gold -= GoldToRemove;
        }

        public int GetGold()
        {
            return _gold;
        }

        public void SetLastDamage(int LastDamage)
        {
            _lastDamage = LastDamage;
        }

        public int GetLastDamage()
        {
            return _lastDamage;
        }

        public void AddToInventory(GameObject obj)
        {
            _inventory.Add(obj);
        }

        public GameObject SwapObjectFromInventory(int idx, GameObject objToSwap)
        {
            GameObject old = _inventory[idx];
            _inventory[idx] = objToSwap;
            return old;
        }

        public void ReplaceInventory(List<GameObject> inv)
        {
            _inventory = inv;
        }

        public List<GameObject> GetInventory()
        {
            return _inventory;
        }
    }

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D cc;
    private PlayerMovement pm;
    private Status _status;

    [SerializeField]
    private int Health;
    [SerializeField]
    private int Stamina;
    [SerializeField]
    private int Mana;
    [SerializeField]
    private int Gold;
    [SerializeField]
    private List<GameObject> Inventory;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        pm = gameObject.GetComponent<PlayerMovement>();
        _status.SetHealth(Health);
        _status.SetStamina(Stamina);
        _status.SetMana(Mana);
        _status.SetGold(Gold);
        _status.SetLastDamage(0);
        _status.ReplaceInventory(Inventory);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}