using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsabilidad única: gestionar la navegación del menú principal.
/// Asigna los métodos PlayGame() y ExitGame() a los botones desde el Inspector.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre exacto de la escena jugable (debe estar en File → Build Settings).")]
    [SerializeField] private string gameSceneName = "GameScene";

    /// <summary>Carga la escena del juego. Asigna al botón Play.</summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>Cierra la aplicación. Asigna al botón Exit.</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
