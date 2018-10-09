using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OLib
{
    public class EntityCreateData
    {
        public GameObject parent = null;
        public string fileName = "";
        public string assetBundleName = "";
        public Vector3 localScale = Vector3.one;
        public Vector3 position = Vector3.zero;
        public int tableId = 0;
    }
}
