using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class Entity : ObjectBasePool {
        private int m_uuid = 0;
        private int m_tableId = 0;
        private AttributeVariable m_variable = new AttributeVariable();
        private Transform m_transform = null;
        private EntityModel m_model = null;
        private AI m_ai = null;

        public int uuid { get { return m_uuid; } }
        public virtual Vector3 position { get { return m_transform.position; } set { m_transform.position = value; } }
        public AttributeVariable variable { get { return m_variable; } }
        public AI ai { get { return m_ai; } protected set { m_ai = value; } }
        public EntityModel model
        {
            get
            {
                return m_model;
            }

            protected set
            {
                if (null != m_model)
                    m_model.Dispose();
                m_model = value;
            }
        }

        public override void initPool()
        {
            base.initPool();

            m_uuid = 0;
            m_tableId = 0;
            model = null;
            m_variable = new AttributeVariable();
        }

        public virtual void initialize(int _uuid, EntityCreateData createData)
        {
            OLib.Console.assert(0 < _uuid, "invalid uuid {0}", _uuid);
                
            m_uuid = _uuid;
            m_tableId = createData.tableId;
            m_transform = transform;
            position = createData.position;
        }

        void Update()
        {
            try
            {
                update();
            }
            catch(System.Exception e)
            {
                OLib.Console.exception(e);
            }
        }

        protected virtual void update()
        {
            if (null != m_ai)
                m_ai.update();
            if (null != m_model)
                m_model.update();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != m_model)
                {
                    m_model.Dispose();
                    m_model = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}