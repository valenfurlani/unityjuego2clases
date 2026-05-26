using UnityEngine;
using TMPro;

/// <summary>
/// Responsabilidad única: mostrar el mejor tiempo y el tiempo de la última
/// partida en la pantalla del menú principal.
/// Lee de RecordService (PlayerPrefs) — no depende de escena de juego.
/// </summary>
public class MainMenuRecordDisplay : MonoBehaviour
{
    [Header("Textos (opcionales — deja vacío si no los usas)")]
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI lastTimeText;

    [Header("Visibilidad")]
    [Tooltip("Objeto que se oculta si nunca se ha jugado (ej. el panel entero de récords).")]
    [SerializeField] private GameObject recordPanel;

    private void Start()
    {
        bool hasPlayed = RecordService.BestTime > 0f;

        if (recordPanel != null)
            recordPanel.SetActive(hasPlayed);

        if (!hasPlayed) return;

        if (bestTimeText != null)
            bestTimeText.text = RecordService.FormatTime(RecordService.BestTime);

        if (lastTimeText != null)
            lastTimeText.text = RecordService.FormatTime(RecordService.LastTime);
    }
}
