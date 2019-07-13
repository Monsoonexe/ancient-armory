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
            GameDatabase = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDatabase>();
        }

        /// <summary>
        /// Called by Button.
        /// </summary>
        public override void OnReadyIconPressed()
        {
            base.OnReadyIconPressed();
            SpawnMerc();
            infoPromptControllerInstance.LoadInfo(newMerc.GetComponent<MercController>());
        }

        /// <summary>
        /// Called by Button.
        /// </summary>
        public override void OnRightButton()//accept
        {
            Debug.Log("Send to Armory!", this);
            SendToArmory();
            readyIcon.SetActive(false);
            // deduct money
            StartTimerCycle(); // Start again
        }


        /// <summary>
        /// Called by Button.
        /// </summary>
        public override void OnLeftButton()
        {
            Debug.Log("Send to Battlefield!", this);
            SendToBattlefield();
            readyIcon.SetActive(false);
            // deduct money
            StartTimerCycle(); // Start again
        }

        /// <summary>
        /// Called by Timer Event.
        /// </summary>
        private void OnRecruitingComplete()
        {
            Debug.Log("Recruiting Complete! Increment up!", this);
            timerController.onTimerComplete.RemoveListener(OnRecruitingComplete);
            StartCoroutine(ShowCompleteWindow());//show message to player
            //TimerCycle();//or do so immediately
        }

        void SendToArmory()
        {
            armoryControllerInstance.Scavengers.Add(newMerc);
            newMerc.transform.parent = Battlefield.transform;
        }

        void SendToBattlefield()
        {
            battlefieldControllerInstance.WaitingLine.Add(newMerc);
            newMerc.transform.parent = Battlefield.transform;
        }

        /// <summary>
        /// Must be called before any other function that uses newMerc.
        /// </summary>
        void SpawnMerc()
        {
            if (Tavern.transform.childCount == 0) // if pool is empty
            {
                // Create a new instance of MercPrefab TODO: get exact start location
                newMerc = Instantiate(MercPrefab, Tavern.transform);
                // Create new character mono & attach it to newMerc
                Character mercCharacter = GameDatabase.Classes.CreateCharacter(newMerc, "Soldier", 1, GameDatabase.Extensions);
                // New merc level is equal to the number of mercs spawned
                mercCharacter.Level = ++MercsSpawned;

                attachMercController();
                assignStats(mercCharacter);
            }
            else // if pool has mercs
            {
                // Dead mercs are pooled in the DeadPool. Grab one to use.
                newMerc = DeadPool[0];
                // Make the invisible dead visible
                newMerc.SetActive(true);
                // Move to Tavern position. TODO: get exact start location
                newMerc.transform.parent = Tavern.transform;
                // Get existing Character component
                Character mercCharacter = newMerc.GetComponent<Character>();
                // New merc level is equal to the number of mercs spawned
                mercCharacter.Level = ++MercsSpawned;
                assignStats(mercCharacter);
            }
        }

        void attachMercController()
        {
            
        }

        void assignStats(Character merc)
        {
            
        }
    }
}
