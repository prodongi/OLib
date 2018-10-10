using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OLib
{
    public class EntityModel : ObjectBasePool {
        [SerializeField] EntityModelDummy m_dummy = new EntityModelDummy();

        public virtual void update()
        {

        }
    }
}
