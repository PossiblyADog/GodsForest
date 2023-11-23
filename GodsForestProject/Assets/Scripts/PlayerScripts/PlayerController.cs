using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private int currentWeaponIndex;
    private SpriteRenderer playerSprite;
    private Vector2 mousePos;
    private Transform playerCharacter;
    private Rigidbody2D playerBody;
    public PlayerInputActions playerActions;
    private bool isAlive, isFiringLMB, isFiringRMB;
    private float rollCounter;
    public AudioClip rollSound;
    private Animator playerAnimator;
    private SpriteRenderer currentWeaponSprite;

    public List<ItemList> items = new List<ItemList>();
    public List<AbstractPlayerWeapon> currentWeapons = new List<AbstractPlayerWeapon>();
    public AudioSource playerAudio;

    public TMPro.TMP_Text fText;
    void Awake()      
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            isAlive = true;
            isFiringLMB = false;
            isFiringRMB = false;
            currentWeaponSprite = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            playerSprite = GetComponent<SpriteRenderer>();
            playerAnimator = GetComponent<Animator>();
            playerCharacter = GetComponent<Transform>();
            playerAudio = GetComponentInChildren<AudioSource>();

            playerBody = GetComponent<Rigidbody2D>();
            ActivateActionMap();
            //PlayerStateManager.playerManager.LoadPrefs();
        }
        else
        {
            Destroy(gameObject);
        }


    }

    public void ActivateActionMap()
    {
            playerActions = new PlayerInputActions();
            playerActions.Player.Enable();

            playerActions.Player.Fire.performed += Fire_LMB;
            playerActions.Player.SecondaryFire.performed += Fire_RMB;
            playerActions.Player.DodgeRoll.performed += DodgeRoll;
            playerActions.Player.SwapWeaponRight.performed += SwapWeaponRight;
            playerActions.Player.SwapWeaponLeft.performed += SwapWeaponLeft;         
    }


    private void Start()
    {
        playerAudio.volume = GameManager.instance.sfxVolume/4;        
        if (currentWeapons.Count == 0)
        {
            var starter = new StarterStaff();
            starter.Awake();
            starter.SetCDPanelData();
            currentWeapons.Add(starter);
          
            currentWeaponIndex = 0;
            currentWeaponSprite.sprite = currentWeapons[0].weaponSprite;
            //currentWeaponSprite.transform.localPosition = new Vector3(.4f, 0, 0);
            
        }
        StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
        
        //StartCoroutine(ItemUpdate());

    }


    private void Update()
    {
        
        if (playerActions.Player.CursorPosition != null)
        {
            //mousePos = Camera.main.ScreenToWorldPoint(playerActions.Player.CursorPosition.ReadValue<Vector2>());
            mousePos = CursorController.instance.transform.position + new Vector3(0, 0.1f, 0);
        }
    }

    IEnumerator ItemUpdate()
    {
        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                i.item.Update(this, i.itemStacks);
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(ItemUpdate());
        }
    }

    public Vector2 CurrentVelocity()
    {
        return playerBody.velocity;
    }


    private void FixedUpdate()
    {
        if (isAlive)
        { 
            if (playerBody != null)
            {
                if (playerActions.Player.Move.ReadValue<Vector2>() != null)
                {
                    //testText.text = PlayerStateManager.playerManager.currentSpeed.ToString();
                    Vector2 inputVector = playerActions.Player.Move.ReadValue<Vector2>();
                    playerBody.AddForce(inputVector * PlayerStateManager.playerManager.currentSpeed, ForceMode2D.Impulse);
                }

                if (Mathf.Abs(playerBody.velocity.x) > .03f || Mathf.Abs(playerBody.velocity.y) > .03f)
                {
                    playerAnimator.SetBool("isMoving", true);
                    if (!playerAudio.isPlaying) 
                    { 
                        playerAudio.Play();
                    }

                }
                else
                {
                    if (playerAudio.isPlaying)
                    {
                        playerAudio.Stop();
                    }
                    playerAnimator.SetBool("isMoving", false);
                }


            }
            if (playerBody.velocity.x > 0.0f)
            {
                playerSprite.flipX = false;
            }
            else if (playerBody.velocity.x < 0.0f)
            {
                playerSprite.flipX = true;
            }

        }
    }

    private void Fire_LMB(InputAction.CallbackContext context)
    {
        if (!Cursor.visible)
        {
            isFiringLMB = !isFiringLMB;
            if (isFiringLMB)
            {
                StartCoroutine(Fire_Primary());
            }
        }
    }

    private IEnumerator Fire_Primary()
    {

        Vector2 travelDirection = mousePos - (Vector2)transform.position;
        currentWeapons[currentWeaponIndex].OnLMB(this, travelDirection);
        yield return new WaitForSeconds(.05f);
        if (isFiringLMB)
        {
            StartCoroutine(Fire_Primary());
        }
        else
        {
            currentWeapons[currentWeaponIndex].OnLMB_Release(this, travelDirection);
        }
    }

    private IEnumerator Fire_Secondary()
    {
        Vector2 travelDirection = mousePos - (Vector2)transform.position;
        currentWeapons[currentWeaponIndex].OnRMB(this, travelDirection);
        yield return new WaitForSeconds(.05f);
        if (isFiringRMB)
        {
            StartCoroutine(Fire_Secondary());
        }
    }

    public void Call_LMB_Items()
    {
        Vector2 travelDirection = mousePos - (Vector2)transform.position;
        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                i.item.OnLMB(this, travelDirection, i.itemStacks);

            }
        }
    }


    private void Fire_RMB(InputAction.CallbackContext context)
    {
        isFiringRMB = !isFiringRMB;
        if (isFiringRMB)
        {
            StartCoroutine(Fire_Secondary());
        }

    }
    public void Call_RMB_Items()
    {
        Vector2 travelDirection = mousePos - (Vector2)transform.position;
        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                i.item.OnRMB(this, travelDirection, i.itemStacks);
            }
        }
    }

    public void Fire_LMB_Rear()
    {
        Vector2 travelDirection = -(mousePos - (Vector2)transform.position);
        currentWeapons[currentWeaponIndex].LMB_Manual(this, travelDirection);

        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                if (!i.name.Contains("Mirror"))
                { i.item.OnLMB(this, travelDirection, i.itemStacks); }

            }
        }
    }

    public void PlayPlayerSound(AudioClip sound, bool pitchShift)
    {
        if (pitchShift)
        {
            playerAudio.pitch = 1.000f + Random.Range(-0.150f, 0.150f);
        }
        else if(playerAudio.pitch != 1.0f)
        {
            playerAudio.pitch = 1.0f;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position, GameManager.instance.sfxVolume);
    }

    private void DodgeRoll(InputAction.CallbackContext obj)
    {
        if (playerBody != null && playerBody.velocity != new Vector2(0, 0) && Time.time > rollCounter)
        {
            rollCounter = Time.time + PlayerStateManager.playerManager.rollCooldown;
            playerAnimator.SetTrigger("isRolling");
            AudioSource.PlayClipAtPoint(rollSound, transform.position, GameManager.instance.sfxVolume);
            DodgeRollItems();
            Vector2 rollDirection = new Vector2(playerCharacter.transform.position.x + playerBody.velocity.x, playerCharacter.transform.position.y + playerBody.velocity.y);
            playerBody.AddForceAtPosition(new Vector2(playerBody.velocity.x * PlayerStateManager.playerManager.currentRollForce, playerBody.velocity.y * PlayerStateManager.playerManager.currentRollForce), rollDirection, ForceMode2D.Impulse);
        }

    }

    private void DodgeRollItems()
    {
        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                i.item.OnPlayerRoll(this, i.itemStacks);
            }
        }
    }

    internal void TakeDamage(int damageToTake)
    {
        if (isAlive)
        {
            playerAnimator.SetTrigger("isHurt");

            var damage = (damageToTake - PlayerStateManager.playerManager.flatArmorModifier);

            

            if (damage > 0)
            {
                var damText = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), transform.position + new Vector3(.05f, .05f), Quaternion.identity);
                damText.transform.localScale *= 3.0f;
                damText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                damText.GetComponentInChildren<TMPro.TMP_Text>().text = (damage).ToString();
                PlayerStateManager.playerManager.SetCurrentHP(-damage);
                Destroy(damText, .75f);
            }



            if (items.Count > 0)
            {
                foreach (var i in items)
                {
                    i.item.OnPlayerDamaged(this, i.itemStacks);

                }
            }
        }
    }

    public void TriggerIncomingProjectileItems()
    {
        if (items.Count > 0)
        {
            foreach (var i in items)
            {
                i.item.OnIncomingProjectile(this, i.itemStacks);

            }
        }
    }
    internal void GetKnocked(Vector3 direction, float knockbackForce)
    {
        playerBody.AddForce(direction * knockbackForce / PlayerStateManager.playerManager.defKnockMultiplier, ForceMode2D.Impulse); ;
    }

    internal void SummonTheReaper()
    {
        playerAnimator.SetTrigger("isDead");
        isAlive = false;
        StartCoroutine(DeathProtocol());
    }

    internal IEnumerator DeathProtocol()
    {
        if (!isAlive)
        {  
            if(items.Find(item => item.item.GiveName() == "Old Woman's Pendant") != null)
            {
                items.Remove(items.Find(item => item.item.GiveName() == "Old Woman's Pendant"));
                yield return new WaitForSeconds(1.5f);
                PlayerStateManager.playerManager.SetCurrentHP(30 + Mathf.Abs(PlayerStateManager.playerManager.currentHP));
                playerAnimator.SetTrigger("isRevive");
                PlayerStateManager.playerManager.SetInvulnerableWindow(1.25f);
                var reviveShield = Instantiate(Resources.Load<GameObject>("Sfx/HolyDeflector"), transform.position, Quaternion.identity);
                reviveShield.GetComponent<PointEffector2D>().forceMagnitude *= 2;
                reviveShield.transform.localScale *= 3.0f;
                Destroy(reviveShield, .5f);
                isAlive = true;
            }

            if (!isAlive)
            {

                if (BossHealthUpdater.instance.transform.GetChild(0).gameObject.activeSelf)
                {
                    BossHealthUpdater.instance.Switch();
                }

                GameManager.instance.CampLevelLoad();
                yield return new WaitForSeconds(1.5f);
                currentWeapons = new List<AbstractPlayerWeapon>();
                items = new List<ItemList>();
                var starter = new StarterStaff();
                starter.Awake();
                currentWeapons.Add(starter);
                currentWeaponIndex = 0;
                currentWeaponSprite.sprite = currentWeapons[0].weaponSprite;
                isAlive = true;
                PlayerStateManager.playerManager.Initialize();
                playerAnimator.SetTrigger("isReset");
                HealthbarUpdater.instance.ResetHP();
                StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
                if (GetComponentInChildren<PlayerQuestIndicator>() != null)
                {
                    Destroy(GetComponentInChildren<PlayerQuestIndicator>().gameObject);
                }
                if(transform.Find("LaserBot(Clone)") != null)
                {
                    Destroy(transform.Find("LaserBot(Clone)").gameObject);
                }
            }
        }
    }


    internal void AddWeapon(AbstractPlayerWeapon weaponToAdd)
    {
        var staffName = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), transform.position + new Vector3(0, .25f), Quaternion.identity);
        staffName.GetComponentInChildren<TMPro.TMP_Text>().fontStyle = TMPro.FontStyles.Italic;
        staffName.GetComponentInChildren<TMPro.TMP_Text>().color = Color.blue;
        staffName.GetComponentInChildren<TMPro.TMP_Text>().text = (weaponToAdd.GiveName()).ToString();
        staffName.transform.localScale = staffName.transform.lossyScale * 2.5f;
        Destroy(staffName, .75f);
        if (currentWeapons.Count < 3)
        {
            weaponToAdd.Awake();
            currentWeapons.Add(weaponToAdd);
            StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
        }
        else
        {
            if (!currentWeapons[currentWeaponIndex].GiveName().Contains("Starter"))
            {
                Instantiate(PlayerStateManager.playerManager.staffDrops.Find(match => match.name == currentWeapons[currentWeaponIndex].GiveName()), transform.position, Quaternion.identity);
            }
                currentWeapons.Remove(currentWeapons[currentWeaponIndex]);
                currentWeapons.RemoveAll(match => match == null);
            
                weaponToAdd.Awake();
                currentWeapons.Add(weaponToAdd);
                StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
                currentWeaponSprite.sprite = currentWeapons[currentWeaponIndex].weaponSprite;
            
        }
              
    }
    private void SwapWeaponRight(InputAction.CallbackContext context)
    {
        currentWeaponIndex++;

        if (currentWeaponIndex >= currentWeapons.Count)
        { currentWeaponIndex = 0; }

        currentWeaponSprite.sprite = currentWeapons[currentWeaponIndex].weaponSprite;
        StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
        currentWeapons[currentWeaponIndex].SetCDPanelData();
    }

    private void SwapWeaponLeft(InputAction.CallbackContext context)
    {

        currentWeaponIndex--;
        if(currentWeaponIndex < 0)
        {
            currentWeaponIndex = currentWeapons.Count - 1;
        }
    

        currentWeaponSprite.sprite = currentWeapons[currentWeaponIndex].weaponSprite;
        StaffInfo.instance.SetStaffInfo(currentWeaponIndex);
        currentWeapons[currentWeaponIndex].SetCDPanelData();
    }

    internal Vector2 GetWeaponPosition()
    {
        return currentWeaponSprite.transform.position;
    }

    public void AddItem(ItemList itemToAdd)
    {
            var itemText = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), transform.position + new Vector3(0, .25f), Quaternion.identity);
            itemText.GetComponentInChildren<TMPro.TMP_Text>().text = "+" + (itemToAdd.item.GiveName()).ToString();
            itemText.transform.localScale = itemText.transform.lossyScale * 2.5f;
            int rarity = itemToAdd.item.ItemRarity();
            switch (rarity)
            {
                case  0: 
                        break;
                case  1:
                    itemText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.grey;
                    break;
                case  2:
                    itemText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.green;
                    break;
                case  3:
                    itemText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.cyan;
                    break;
                case  4:
                    itemText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.magenta;
                    break;
                case  6:
                    itemText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.yellow;
                    break;
            }
            Destroy(itemText, .75f);

        if(items.Count > 0)
        { 
            bool alreadyHave = false;
            foreach (var item in items)
            {
                if (item.name == itemToAdd.name)
                { alreadyHave = true; }       
            }

            if (alreadyHave)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].name == itemToAdd.name)
                    {
                        items[i].itemStacks += 1;                        
                    }
                }
            }
            else
            {
                items.Add(itemToAdd);                
            }
            
        }
        else
        {
            items.Add(itemToAdd);           
        }

        itemToAdd.item.OnPickUp();




    }
}
