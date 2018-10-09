using System.Collections.Generic;
using UnityEngine;

namespace OLib
{
    public class ResourceManager<T> : MonoSingleton<T> where T : MonoBehaviour {
        private Dictionary<string, Object> m_resources = new Dictionary<string, Object>();

        public void unload(string path)
        {
            Object obj = findResource<Object>(path);
            if (null == obj)
                return;

            Resources.UnloadAsset(obj);
            m_resources.Remove(path);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                unloadAll();
            }
        }

        private void unloadAll()
        {
            Resources.UnloadUnusedAssets();
            m_resources.Clear();
        }

        public R getInstance<R>(string pathName, GameObject parent = null, bool isUi = false, string name = null, string _tag = null)
        {
            GameObject obj = getInstance(pathName, Vector3.one, Vector3.zero, parent, isUi, name, _tag);
            if (null == obj)
                return default(R);

            return obj.GetComponent<R>();
        }

        public GameObject getInstance(string pathName, GameObject parent = null, bool isUi = false, string name = null, string _tag = null)
        {
            return getInstance(pathName, Vector3.one, Vector3.zero, parent, isUi, name, _tag);
        }

        /**
         * @brief key의 프리팹의 인스턴스를 얻는다.
         */
        public GameObject getInstance(string pathName, Vector3 scaleFactor, Vector3 position, GameObject parent = null, bool isUi = false, string name = null, string _tag = null)
        {
            GameObject prefab = findResource<GameObject>(pathName);
            if (!prefab)
            {
                prefab = loadResource<GameObject>(pathName);
                if (!prefab)
                    return null;
            }
            GameObject inst = MonoBehaviour.Instantiate(prefab) as GameObject;
            if (!inst)
            {
                OLib.Console.debug("Failed GameObject Instantiate");
                return null;
            }

            Vector3 localScale = inst.transform.localScale;

            inst.name = (null == name) ? System.IO.Path.GetFileNameWithoutExtension(pathName) : name;

            if (null != parent)
            {
                if (isUi)
                    inst.transform.SetParent(parent.transform);
                else
                    inst.transform.parent = parent.transform;
            }
            if (null != _tag)
                inst.tag = _tag;

            inst.transform.localScale = new Vector3(localScale.x * scaleFactor.x, localScale.y * scaleFactor.y, localScale.z * scaleFactor.z);
            inst.transform.position = position;
            inst.transform.localRotation = new Quaternion();

            return inst;
        }

        private bool isResource(string key)
        {
            return m_resources.ContainsKey(key);
        }

        private R findResource<R>(string key) where R : Object
        {
            Object obj = null;
            if (!m_resources.TryGetValue(key, out obj))
                return default(R);

            return obj as R;
        }

        /**
         * @brief Resource 폴더에서 데이타를 로그한다.
         */
        public R loadResource<R>(string pathName) where R : Object
        {
            R prefab = findResource<R>(pathName);
            if (null != prefab)
                return prefab;

            prefab = Resources.Load(pathName) as R;
            if (prefab)
            {
                m_resources.Add(pathName, prefab);
            }
            else
            {
                OLib.Console.error(string.Format("Failed load prefab Data, path {0}", pathName));
            }
            return prefab;
        }

        /**
         *  @brief resource 원본의 텍스트를 구한다.
         * */
        public string loadResourceText(string path)
        {
            TextAsset asset = loadResource<TextAsset>(path);
            if (null == asset)
                return "";

            return asset.text;
        }
    }
}
