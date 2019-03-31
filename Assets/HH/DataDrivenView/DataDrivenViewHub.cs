using System;
using System.Collections;
using UnityEngine;

namespace HH.DataDrivenFramework {

    public class DataDrivenViewHub : MonoBehaviour {

        public SceneType scene;
        public DataDrivenView[] views;

        class Handlers : GroupedDictionary<DataDrivenView, Type, Func<object, float>> { }
        Handlers handlers;
        DataQueue queue;

        void Awake() {
            handlers = new Handlers();
            foreach (var view in views) {
                handlers.Add(view, view.DataType, view.UpdateView);
            }
            queue = SceneControllerHub.Instance.RegisterScene(scene);
            StartCoroutine(WorkLoop());
        }

        void OnDestroy() {
            queue = null;
            SceneControllerHub.Instance.UnregisterScene(scene);
            foreach (var view in views) {
                handlers.Remove(view);
            }
        }

        IEnumerator WorkLoop() {
            while (queue != null) {
                while (queue.Count > 0) {
                    var data = queue.Dequeue();
                    var tBlockAll = 0f;
                    //Debug.LogFormat("DataDrivenViewHub:{0}", data);
                    handlers.ForEach(data.GetType(), (view, handle) => {
                        var tBlock = 0f;
                        try {
                            tBlock = handle(data);
                        } catch (Exception ex) {
                            Debug.LogErrorFormat("ViewData {1} @ {0} : {2}", view, data, ex);
                        }
                        if (tBlock > tBlockAll) {
                            tBlockAll = tBlock;
                        }
                    });
                    if (tBlockAll > 0) {
                        //Debug.LogFormat("DataDrivenViewHub:{0}s", tBlockAll);
                        yield return new WaitForSeconds(tBlockAll);
                    }
                }
                yield return new WaitForSeconds(0);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("自动加载")]
        void 自动加载() {
            views = FindObjectsOfType<DataDrivenView>();
        }
#endif
    }
}