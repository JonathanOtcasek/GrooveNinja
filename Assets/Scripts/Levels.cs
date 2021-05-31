using UnityEngine;

public class Levels : ScriptableObject
{
    public Arrow[][] allLevels = new Arrow[][] { new Arrow[] { new Arrow(2, 1, 1, 2), new Arrow(2, 3, 1, 1), new Arrow (3, 1, 1, 2), new Arrow(3, 3, 1, 1) },
        new Arrow[] { new Arrow(2, 1, 1, 2), new Arrow(2, 2, 1, 1), new Arrow (2, 3, 1, 2), new Arrow(2, 3, 48, 0), new Arrow(2, 4, 1, 1) },
        new Arrow[] { new Arrow(2, 1, 1, 2), new Arrow(2, 1, 48, 0), new Arrow(2, 2, 1, 1), new Arrow(2, 2, 48, 0), new Arrow (2, 3, 1, 2), new Arrow(2, 3, 48, 2), new Arrow(2, 4, 1, 0),new Arrow(2, 4, 24, 2),new Arrow(2, 4, 48, 1) },
    };
}