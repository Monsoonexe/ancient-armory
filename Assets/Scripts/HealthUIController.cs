using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AncientArmory
{
    public class HealthUIController : MonoBehaviour
    {
        [Header("---UI Elements---")]
        [SerializeField]
        private Slider healthSlider;

        [SerializeField]
        private Image healthSliderFill;

        [SerializeField]
        private TextMeshProUGUI healthTMP;

        [Header("---Health Colors---")]
        [SerializeField]
        private HealthColors healthColors;
        
        /// <summary>
        /// Match visuals to given values. Should be called any time health has changed.
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="maxHealth"></param>
        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            var healthPercent = (float)currentHealth / (float)maxHealth;

            if (healthSlider) healthSlider.value = healthPercent;

            if (healthSliderFill) healthSliderFill.color = healthColors.GetColor(healthPercent);

            if (healthTMP)
            {
                var healthOutput = new System.Text.StringBuilder();

                healthOutput.Append(currentHealth.ToString());
                healthOutput.Append(" / ");
                healthOutput.Append(maxHealth.ToString());

                healthTMP.text = healthOutput.ToString();
            }
        }

        public void ToggleTextReadout(bool active)
        {
            if (healthTMP) healthTMP.gameObject.SetActive(active);
        }

        public void ToggleSlider(bool active)
        {
            if (healthSlider) healthSlider.gameObject.SetActive(active);
        }
    }
}