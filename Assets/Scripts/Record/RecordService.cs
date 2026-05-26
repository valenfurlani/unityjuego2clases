/// <summary>
/// Responsabilidad única: leer y escribir el mejor tiempo en PlayerPrefs.
/// Clase estática pura — sin dependencia de MonoBehaviour ni de Unity lifecycle.
///
/// SOLID:
///  S – Solo gestiona persistencia del record.
///  D – Accedido por cualquier clase sin depender de una instancia concreta.
/// </summary>
public static class RecordService
{
    private const string BestTimeKey    = "BestTimeSurvived";
    private const string LastTimeKey    = "LastTimeSurvived";

    /// <summary>Mejor tiempo guardado en segundos. 0 si nunca se ha jugado.</summary>
    public static float BestTime  => UnityEngine.PlayerPrefs.GetFloat(BestTimeKey, 0f);

    /// <summary>Tiempo de la última partida en segundos.</summary>
    public static float LastTime  => UnityEngine.PlayerPrefs.GetFloat(LastTimeKey, 0f);

    /// <summary>
    /// Guarda el tiempo de la sesión actual.
    /// Si supera el record, lo actualiza y devuelve true.
    /// </summary>
    public static bool SubmitTime(float sessionSeconds)
    {
        UnityEngine.PlayerPrefs.SetFloat(LastTimeKey, sessionSeconds);

        bool isNewRecord = sessionSeconds > BestTime;
        if (isNewRecord)
            UnityEngine.PlayerPrefs.SetFloat(BestTimeKey, sessionSeconds);

        UnityEngine.PlayerPrefs.Save();
        return isNewRecord;
    }

    /// <summary>Borra todos los records guardados.</summary>
    public static void ResetRecords()
    {
        UnityEngine.PlayerPrefs.DeleteKey(BestTimeKey);
        UnityEngine.PlayerPrefs.DeleteKey(LastTimeKey);
        UnityEngine.PlayerPrefs.Save();
    }

    /// <summary>Formatea segundos como mm:ss.</summary>
    public static string FormatTime(float seconds)
    {
        int m = UnityEngine.Mathf.FloorToInt(seconds / 60);
        int s = UnityEngine.Mathf.FloorToInt(seconds % 60);
        return $"{m:00}:{s:00}";
    }
}
