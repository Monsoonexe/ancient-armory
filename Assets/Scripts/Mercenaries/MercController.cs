using System.Collections;
using UnityEngine;
using RpgDB;

namespace AncientArmory
{
    [RequireComponent(typeof(Animator))]
    //[RequireComponent(typeof(Character))]//re-enable when refactored and able to be attached.
    [RequireComponent(typeof(SpriteRenderer))]
    public class MercController : MonoBehaviour
    {
        [Header("---Merc Info---")]
        [SerializeField]
        private string mercName;

        [Header("---Health---")]
        [SerializeField]
        private HealthUIController healthController;

        [SerializeField]
        private int currentHealth = 0;

        [Header("---Combat---")]
        public Weapon weapon;
        public int defense;
        public int cost;

        [Header("---Movement---")]
        [SerializeField]
        private float moveSpeed = 1.0f;

        private Coroutine movementCoroutine;

        /// <summary>
        /// Stopping distance.
        /// </summary>
        private const float closeEnoughDistance = 0.1f;
        
        //member Components
        private Animator myAnimator;
        private Character myCharacter;
        private SpriteRenderer mySpriteRenderer;
        private Transform myTransform;
        
        /// <summary>
        /// Used for initialization.
        /// </summary>
        public void Awake()
        {
            GatherReferences();
        }

        // Update is called once per frame
        void Update()
        {
            //healthController.UpdateHealth(currentHealth, maxHealth);//wasteful in update -- only when needed
        }

        /// <summary>
        /// Initialize references to classes.
        /// </summary>
        private void GatherReferences()
        {
            myCharacter = GetComponent<Character>();
            myAnimator = GetComponent<Animator>();
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            myTransform = transform;//cache to avoid excess garbage generation
        }

        /// <summary>
        /// Move Merc a little every frame closer to destination.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveToDestination(Vector3 targetPosition)
        {
            while(Vector3.Distance(myTransform.position, targetPosition) > closeEnoughDistance)
            {
                //do move
                myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Initialize maxHealth and currentHealth values.
        /// </summary>
        public void InitHealth()
        {
            currentHealth = myCharacter.Hit_Points();
            healthController.UpdateHealth(currentHealth, myCharacter.Hit_Points());//visuals
        }

        public void SetArmorAndDefense(Armor armor)
        {
            // Set armor for KAC check
            myCharacter.Armor = armor;
            defense = myCharacter.KAC();//refresh stats
        }

        private void Die()
        {
            Debug.Log("I am become death.", this);
        }

        public void ApplyHealing(int healingAmount)
        {
            currentHealth += healingAmount;
            if (currentHealth > myCharacter.Hit_Points())
                currentHealth = myCharacter.Hit_Points();
            healthController.UpdateHealth(currentHealth, myCharacter.Hit_Points());//visuals
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                Die();
                currentHealth = 0;//prevent negative health values

            }
            healthController.UpdateHealth(currentHealth, myCharacter.Hit_Points());//visuals
        }

        public int Attack(MercController target)
        {
            bool hit = myCharacter.AttackCheck(weapon, target.defense);
            if (hit)
                return myCharacter.Attack(weapon, target.defense);
            return 0;
        }

        /// <summary>
        /// Start moving Merc to target position over time.
        /// </summary>
        /// <param name="targetTransform"></param>
        public void SetDestination(Transform targetTransform)
        {
            SetDestination(targetTransform.position);
        }

        /// <summary>
        /// Start moving Merc to target position over time.
        /// </summary>
        /// <param name="targetPosition">Coordinates in World Space.</param>
        public void SetDestination(Vector3 targetPosition)
        {
            movementCoroutine = StartCoroutine(MoveToDestination(targetPosition));
        }

        /// <summary>
        /// Tell Merc to stop moving to a new position.
        /// </summary>
        public void StopMoving()
        {
            if(movementCoroutine != null)//if the coroutine is running...
            {
                StopCoroutine(movementCoroutine);//kill it
            }
        }

        /// <summary>
        ///  This may be used in the future.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defense"></param>
        /// <returns></returns>
        private int resolveSingleOrDualWeildAttack(Character attacker, int defense)
        {
            int damage = 0;
            if (attacker.Right_Hand.Name != "None")
                damage = attacker.Attack(attacker.Right_Hand, defense);
            if (attacker.Left_Hand.Name != "None" || attacker.Right_Hand.Name == "None")
                damage = attacker.Attack(attacker.Left_Hand, defense);
            return damage;
        }

    }
}