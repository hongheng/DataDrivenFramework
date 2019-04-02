using HH.DataDrivenScene.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HH.DataDrivenScene
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadScene(SceneConfig cfg, LoadSceneMode mode = LoadSceneMode.Single) {
            Instance.LoadScene(cfg, mode);
        }

        public static void UnloadAdditiveScene(SceneConfig cfg) {
            Instance.UnloadAdditiveScene(cfg);
        }

        static DataDrivenSceneHub instance;

        static DataDrivenSceneHub Instance {
            get {
                if (instance == null) {
                    var helper = FindObjectOfType<SceneLoader>();
                    if (helper == null) {
                        var prefab = Resources.Load("DataDrivenSceneLoader", typeof(SceneLoader));
                        helper = Instantiate(prefab) as SceneLoader;
                    }
                    DontDestroyOnLoad(helper.gameObject);
                    instance = new DataDrivenSceneHub(helper);
                }
                return instance;
            }
        }

        void Awake() {
            if (instance == null) {
                DontDestroyOnLoad(gameObject);
                instance = new DataDrivenSceneHub(this);
            }
        }

        void OnDestroy() {
            instance = null;
        }
    }
}