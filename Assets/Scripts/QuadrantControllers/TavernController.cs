using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        List<GameObject> DeadPool;
        GameObject Armory;
        GameObject Tavern;
        GameObject Battlefield;
        GameDatabase GameDatabase;

        GameObject newMerc;

        protected override void Start()
        {
            base.Start();
            AddStaticReferences();
            cooldownDelay = 10;
            cooldownMessage = "cooldownMessage";
            StartTimerCycle();
        }

        void AddStaticReferences()
        {
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
            // TODO: Add delay + walking over animation
            armoryControllerInstance.Scavengers.Add(newMerc);
            newMerc.transform.parent = Battlefield.transform;
        }

        void SendToBattlefield()
        {
            // TODO: Add delay + walking over animation
            battlefieldControllerInstance.WaitingLine.Add(newMerc);
            newMerc.transform.parent = Battlefield.transform;
        }

        //
        // Merc Spawn Funtions
        //

        void SpawnMerc()
        {
            Character mercCharacter;
            if (DeadPool.Count == 0) // if pool is empty
            {
                newMercInstance();
            }
            else // if pool has mercs
            {
                newMercFromPool();
            }
        }

        void newMercInstance()
        {
            // Create a new instance of MercPrefab
            newMerc = Instantiate(MercPrefab, Tavern.transform); // TODO: get exact start location
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
            // Move to Tavern position.
            newMerc.transform.parent = Tavern.transform; // TODO: get exact start location
            // Get existing Character component
            Character mercCharacter = newMerc.GetComponent<Character>();
            InitializeMerc(mercCharacter);
        }

        void InitializeMerc(Character mercCharacter)
        {
            // New merc level is equal to the number of mercs spawned
            mercCharacter.Level = ++MercsSpawned;
            assignStats(mercCharacter);
            newMerc.GetComponent<MercController>().SetHealth();
        }

        void assignStats(Character merc)
        {
            merc.Abilities.STR = 10;
            merc.Abilities.DEX = 10;
            merc.Abilities.CON = 10;
        }
    }
}
