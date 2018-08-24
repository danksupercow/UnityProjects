using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct SpawnedPrefab
{
    public string slug;
    public int assignedID;
    public Transform transform;

    public SpawnedPrefab(string slug, int assignedID, Transform transform)
    {
        this.slug = slug;
        this.assignedID = assignedID;
        this.transform = transform;

    }

    public SpawnedPrefab Copy()
    {
        return new SpawnedPrefab(slug, assignedID, transform);
    }
}
