using System;
using System.Collections.Generic;

namespace HH.DataDrivenFramework {

    public class DataQueue : Queue<object> { }

    public interface ISceneController {
        DataQueue RegisterScene(SceneType scene);
        void UnregisterScene(SceneType scene);
    }

    public class SceneControllerHub : SingleTon<SceneControllerHub>, ISceneController {

        Dictionary<SceneType, Func<SceneType, ISceneController>> ctrlerCreators = new Dictionary<SceneType, Func<SceneType, ISceneController>>();
        Dictionary<SceneType, ISceneController> ctrlers = new Dictionary<SceneType, ISceneController>();

        public void RegisterCtrler(SceneType scene, Func<SceneType, ISceneController> creator) {
            if (ctrlerCreators.ContainsKey(scene)) {
                ctrlerCreators[scene] = creator;
            } else {
                ctrlerCreators.Add(scene, creator);
            }
        }

        public void RegisterCtrler(SceneType[] scenes, Func<SceneType, ISceneController> creator) {
            foreach (var scene in scenes) {
                RegisterCtrler(scene, creator);
            }
        }

        public DataQueue RegisterScene(SceneType scene) {
            ISceneController ctrler = null;
            if (!ctrlers.TryGetValue(scene, out ctrler)) {
                Func<SceneType, ISceneController> creator = null;
                if (ctrlerCreators.TryGetValue(scene, out creator)) {
                    ctrler = creator(scene);
                } else {
                    return null;
                }
                ctrlers.Add(scene, ctrler);
            }
            return ctrler.RegisterScene(scene);
        }

        public void UnregisterScene(SceneType scene) {
            if (ctrlers.ContainsKey(scene)) {
                ctrlers[scene].UnregisterScene(scene);
                ctrlers.Remove(scene);
            }
        }
    }

    public abstract class SimpleSceneController : ISceneController {

        DataQueue queueView = new DataQueue();

        DataQueue ISceneController.RegisterScene(SceneType scene) {
            Init(scene);
            return queueView;
        }

        void ISceneController.UnregisterScene(SceneType scene) {
            Dispose(scene);
        }

        protected void PushToView(object model) {
            queueView.Enqueue(model);
        }

        protected abstract void Init(SceneType scene);
        protected virtual void Dispose(SceneType scene) { }
    }

}
