using UnityEngine;
using System.Collections;

namespace AncientArmory
{
    public class ControllerBase : MonoBehaviour
    {
        protected static BattlefieldController battlefieldControllerInstance;
        protected static TavernController tavernControllerInstance;
        protected static ResearchController researchControllerInstance;
        protected static ArmoryController armoryControllerInstance;
        protected static InfoPromptController infoPromptControllerInstance;

        [Header("---ControllerBase---")]
        [SerializeField]
        protected string leaderName = "General Rick";

        [SerializeField]
        protected TimerController timerController;

        /// <summary>
        /// Icon with Button that informs Player Timer is complete.
        /// </summary>
        [Header("---UI---")]
        [SerializeField]
        [Tooltip("Icon with Button that informs Player Timer is complete.")]
        protected GameObject readyIcon;

        /// <summary>
        /// Icon with Button that informs Player Timer is complete.
        /// </summary>
        [SerializeField]
        [Tooltip("Icon with Button that informs Player Timer is complete.")]
        protected GameObject completeWindow;

        /// <summary>
        /// Amount it takes to do one timer cycle.
        /// </summary>
        [Header("---Timer---")]
        [SerializeField]
        [Tooltip("Amount it takes to do one timer cycle.")]
        protected int cooldownDelay = 5;

        /// <summary>
        /// Message to show above head during timer countdown.
        /// </summary>
        [SerializeField]
        [Tooltip("Message to show above head during timer countdown.")]
        protected string cooldownMessage;

        /// <summary>
        /// Message to show above head if it takes time to process (training, researching).
        /// </summary>
        [SerializeField]
        [Tooltip("Message to show above head if it takes time to process (training, researching).")]
        protected string inProcessMessage;

        [Header("---Button Text---")]
        [SerializeField]
        protected string leftButtonText = "Do the left thing.";

        [SerializeField]
        protected string rightButtonText = "Do the right thing.";

        //private member Component references
        protected Transform myTransform;

        protected virtual void Awake()
        {
            GatherStaticReferences();

            myTransform = transform;//cache to reduce excess garbage generation
        }

        /// <summary>
        /// Cache a handle on each of the major Controllers in the Scene, if it does not exist.
        /// </summary>
        protected static void GatherStaticReferences()
        {
            if (!armoryControllerInstance)//if has not been found and assigned already
                armoryControllerInstance = GameObject.FindGameObjectWithTag("ArmoryController").GetComponent<ArmoryController>();

            if (!battlefieldControllerInstance)//if has not been found and assigned already
                battlefieldControllerInstance = GameObject.FindGameObjectWithTag("BattlefieldController").GetComponent<BattlefieldController>();

            if (!researchControllerInstance)//if has not been found and assigned already
                researchControllerInstance = GameObject.FindGameObjectWithTag("ResearchController").GetComponent<ResearchController>();

            if (!tavernControllerInstance)//if has not been found and assigned already
                tavernControllerInstance = GameObject.FindGameObjectWithTag("TavernController").GetComponent<TavernController>();

            if (!infoPromptControllerInstance)//if has not been found and assigned already
                infoPromptControllerInstance = GameObject.FindGameObjectWithTag("UIPromptController").GetComponent<InfoPromptController>();
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void OnEnable()
        {
            StartTimerCycle();
        }

        protected virtual void OnDisable()
        {
            //unsubscribe from events
            timerController.onTimerComplete.RemoveListener(OnCooldownComplete);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnCooldownComplete()
        {
            Debug.Log("Click on the Leader!  " + this.name  + " thing is ready.", this);
            timerController.onTimerComplete.RemoveListener(OnCooldownComplete);
            ShowReadyIcon();
        }

        protected virtual void ShowReadyIcon()
        {
            readyIcon.SetActive(true);
        }

        protected virtual void ShowInfoPrompt()
        {
            infoPromptControllerInstance.ShowInfoPrompt(this, leftButtonText, rightButtonText, "GENERIC TEXT");
        }

        /// <summary>
        /// Where it all begins.
        /// </summary>
        protected virtual void StartTimerCycle()
        {
            //hide UI elements
            readyIcon.SetActive(false);
            completeWindow.SetActive(false);

            timerController.onTimerComplete.AddListener(OnCooldownComplete);
            timerController.StartTimer(cooldownDelay, cooldownMessage);//Gathering new research...
        }

        /// <summary>
        /// Called by Button. Starts new Timer Cycle.
        /// </summary>
        public virtual void OnLeftButton()
        {
            Debug.Log("Left Button Pressed.", this);
            StartTimerCycle();//get a different one
            readyIcon.SetActive(false);
            //do the left thing
        }

        /// <summary>
        /// Called by Button. Starts new Timer Cycle.
        /// </summary>
        public virtual void OnRightButton()
        {
            Debug.Log("Right Button Pressed.", this);
            StartTimerCycle();//get a different one
            readyIcon.SetActive(false);
            //do the right thing
        }
        
        /// <summary>
        /// Coroutine to show happy success image to player before restarting.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        protected virtual IEnumerator ShowCompleteWindow(float moment = 2)
        {
            completeWindow.SetActive(true);

            yield return new WaitForSeconds(moment);

            StartTimerCycle();
        }

        /// <summary>
        /// Called by Button.
        /// </summary>
        public virtual void OnReadyIconPressed()
        {
            // TODO: Move call to InfoPromptController.ShowInfoPrompt here?

            //Debug.Log("Ready icon pressed!", this);
            ShowInfoPrompt();
        }

        public string GetLeaderName()
        {
            return leaderName;
        }
    }
}
