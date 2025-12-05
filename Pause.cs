using UnityEngine;

public class Pause : gameOver
{
    // Sobrescreve o Update para ignorar a morte do player e usar o ESC
    protected override void Update()
    {
        // Se apertar ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Se estava ligado, desliga. Se estava desligado, liga.
            // O ! inverte o valor atual (True vira False e vice-versa)
            ToggleMenu(!isMenuActive);
        }
    }
}