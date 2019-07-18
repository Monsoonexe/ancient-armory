using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RpgDB;

namespace AncientArmory
{
    public sealed class TavernController : ControllerBase
    {
        // Required game objects:
        // readyIcon
        // infoPromptControllerInstance
        [Header("---TavernController---")]
        public int MercsSpawned;
        public GameObject MercPrefab;

        /// <summary>
        /// Point where a Merc gets added to the world.
        /// </summary>
        [SerializeField]
        [Tooltip("Point where a Merc gets added to the world.")]
        private Transform mercSpawnPoint;

        /// <summary>
        /// DeadPool. 
        /// </summary>
        private readonly List<GameObject> DeadPool = new List<GameObject>();

        //private member Component references

        //private external references
        GameObject Armory;
        GameObject Tavern;
        GameObject Battlefield;
        GameDatabase GameDatabase;

        GameObject newMerc;
        Roll Roll;

        protected override void Awake()
        {
            base.Awake();
            AddStaticReferences();

        }

        protected override void Start()
        {
            base.Start();
            cooldownDelay = 10;
            cooldownMessage = "cooldownMessage";
            StartTimerCycle();//let it begin
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Nothing happening in here is static.</remarks>
        private void AddStaticReferences()
        {
            Roll = new Roll();
            Armory = GameObject.FindGameObjectWithTag("ArmoryController");
            Battlefield = GameObject.FindGameObjectWithTag("BattlefieldController");
            Tavern = GameObject.FindGameObjectWithTag("TavernController");
            GameDatabase = GameObject.FindGameObjectWithTag("GameDatabase").GetComponent<GameDatabase>();
        }
        
        //
        // Timer Cycle Functions
        //

        public override void OnReadyIconPressed()
        {
            base.OnReadyIconPressed();
            SpawnMerc();
            infoPromptControllerInstance.LoadInfo(newMerc.GetComponent<MercController>());
        }

        public override void OnRightButton()//accept
        {
            Debug.Log("Send to Armory!", this);
            SendToArmory();
            readyIcon.SetActive(false);
            // deduct money
            StartTimerCycle(); // Start again
        }

        public override void OnLeftButton()
        {
            Debug.Log("Send to Battlefield!", this);
            SendToBattlefield();
            readyIcon.SetActive(false);
            // deduct money
            StartTimerCycle(); // Start again
        }

        private void OnRecruitingComplete()
        {
            Debug.Log("Recruiting Complete! Increment up!", this);
            timerController.onTimerComplete.RemoveListener(OnRecruitingComplete);
            StartCoroutine(ShowCompleteWindow()); //show message to player
            //TimerCycle(); //or do so immediately
        }

        //
        // Merc Control Functions
        //

        void SendToArmory()
        {
            var mercController = newMerc.GetComponent<MercController>();//cache this to avoid excessive reflection
            mercController.SetDestination(armoryControllerInstance.transform.position);//set target destination

            armoryControllerInstance.Scavengers.Add(newMerc);//alert Controller of new charge
            newMerc.transform.parent = armoryControllerInstance.transform;//set Controller as parent
        }

        void SendToBattlefield()
        {
            var mercController = newMerc.GetComponent<MercController>();//cache this to avoid excessive reflection
            mercController.SetDestination(armoryControllerInstance.transform.position);//set target destination

            battlefieldControllerInstance.WaitingLine.Add(newMerc);//alert Controller of new charge
            newMerc.transform.parent = battlefieldControllerInstance.transform;//set Controller as parent
        }
        
        //
        // Merc Spawn Funtions
        //

        void SpawnMerc()
        {
            if (DeadPool.Count == 0) // if pool is empty
            {
                newMercInstance();
            }
            else // if pool has mercs
            {
                //2nd merc pull will still be first merc.  
                newMercFromPool();
            }

            newMerc.transform.position = mercSpawnPoint.position;//set starting point
        }

        void newMercInstance()
        {
            // Create a new instance of MercPrefab
            newMerc = Instantiate(MercPrefab);
            // Create new character mono & attach it to newMerc
            Character mercCharacter = GameDatabase.Classes.CreateCharacter(newMerc, "Soldier", 1, GameDatabase.Extensions);
            newMerc.AddComponent<MercController>();
            InitializeMerc(mercCharacter);
        }

        void newMercFromPool()
        {
            // Dead mercs are pooled in the DeadPool. Grab one to use.
            newMerc = DeadPool[0];
            // Make the invisible dead visible
            newMerc.SetActive(true);
            // Get existing Character component
            Character mercCharacter = newMerc.GetComponent<Character>();
            InitializeMerc(mercCharacter);
        }

        void InitializeMerc(Character mercCharacter)
        {
            MercController controller = newMerc.GetComponent<MercController>();
            mercCharacter.Level = ++MercsSpawned;
            assignStats(mercCharacter);
            assignCost(mercCharacter);
            controller.InitHealth();
        }

        void assignCost(Character merc)
        {
            MercController controller = newMerc.GetComponent<MercController>();
            int maxCost = merc.Abilities.STR;
            maxCost += merc.Abilities.DEX;
            maxCost += merc.Abilities.CON;
            controller.cost = Roll.rollDie(maxCost);
        }

        void assignStats(Character merc)
        {
            // level divided by 3, rounded
            int minimum = Convert.ToInt32(Math.Round((merc.Level / 3f), 0));
            int maximum = merc.Level + 1;
            merc.Abilities.STR = Roll.rollDie(maximum) + minimum;
            merc.Abilities.DEX = Roll.rollDie(maximum) + minimum;
            merc.Abilities.CON = Roll.rollDie(maximum) + minimum;
        }
    }
}
