using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Transforms
{
    public class Transform
    {
        public Vector3 position;
        public Quaternion rotation;

        public Transform(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
