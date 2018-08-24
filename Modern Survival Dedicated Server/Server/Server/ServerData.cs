using Server.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ServerData
    {
        public static List<SyncdPrefab> spawnedPrefabs = new List<SyncdPrefab>();
        public static int SpawnedPrefabCount { get { return spawnedPrefabs.Count; } }
    }
}
