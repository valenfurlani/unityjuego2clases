using UnityEngine;

[System.Serializable]
public class FearRange
{
    public int minFear;
    public int maxFear;
}

[CreateAssetMenu(fileName = "NewFearConfig", menuName = "FearSystem/FearConfig")]
public class FearConfigSO : ScriptableObject
{
    public FearRange[] ranges;

    public int GetRangeIndex(int fearLevel)
    {
        for (int i = 0; i < ranges.Length; i++)
        {
            if (fearLevel >= ranges[i].minFear && fearLevel <= ranges[i].maxFear)
                return i;
        }
        return 0;
    }
}
