using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameter
{
    public static bool DoesPositionExistInsideTheZone(Vector3 position, params Vector2[] zones)
    {
        foreach (var zone in zones)
        {
            if (position.x < zone.x && position.x > -zone.x && position.z < zone.y && position.z > -zone.y)
            {
                return true;
            }
        }

        return false;
    }
}