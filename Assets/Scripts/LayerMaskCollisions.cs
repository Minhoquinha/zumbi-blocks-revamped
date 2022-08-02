using UnityEngine;
using System.Collections.Generic;

public static class LayerMaskCollisions
{
    private static Dictionary<int, int> MasksByLayer;

    public static void Set()
    {
        MasksByLayer = new Dictionary<int, int>();
        for (int i = 0; i < 32; i++)
        {
            int CurrentLayer = 0;
            for (int j = 0; j < 32; j++)
            {
                if (!Physics.GetIgnoreLayerCollision(i, j))
                {
                    CurrentLayer |= 1 << j;
                }
            }
            MasksByLayer.Add(i, CurrentLayer);
        }
    }

    public static int MaskForLayer (int CurrentLayer)
    {
        if (MasksByLayer == null)
        {
            Set();
        }

        return MasksByLayer [CurrentLayer];
    }
}