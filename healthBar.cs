using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Slider[] sliders;

    public void SetSliderMax(float amount)
    {
        // O loop 'foreach' percorre todos os sliders que você colocou na lista
        foreach (Slider s in sliders)
        {
            if (s != null)
            {
                s.maxValue = amount;
                s.value = amount; // Começa cheio
            }
        }
    }

    public void SetSlider(float amount)
    {
        // Atualiza todos os sliders da lista ao mesmo tempo
        foreach (Slider s in sliders)
        {
            if (s != null)
            {
                s.value = amount;
            }
        }
    }
}