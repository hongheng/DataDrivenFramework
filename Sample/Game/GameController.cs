using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public class GameController : SimpleSceneController {

        public GameStatus status;
        public UnitId playerId;

        #region 控制器生命周期. (演示：一个控制器控制多个场景的思路.)

        public GameController() {
            Debug.Log("GameController create");
            InitGame();
        }

        protected override void Init(SceneType scene) {
            Debug.Log("GameController init in: " + scene);
            if (scene == SceneType.SampleInitScene) {
                SceneControllerHub.Instance.RegisterCtrler(scene, s => this);
                SceneLoader.Load(SceneType.SampleGameScene);
            }
        }

        protected override void Dispose(SceneType scene) {
            Debug.Log("GameController Dispose in: " + scene);
            base.Dispose(scene);
            if (scene == SceneType.SampleInitScene) {
                SceneControllerHub.Instance.RegisterCtrler(scene, null);
            }
        }

        #endregion

        #region 界面初始化。（演示：UI的更新）(讨论：MenuModel放入GameModel的必要性)

        void InitGame() {
            status = GameStatus.Init;
            UpdateMenu();
        }

        void UpdateMenu() {
            PushToView(new MenuModel() {
                menus = StatusToMenus(status),
                onClick = OnClickMenu,
            });
        }

        MenuType[] StatusToMenus(GameStatus status) {
            switch (status) {
            case GameStatus.Init:
                return new MenuType[] { MenuType.Start };
            case GameStatus.GameOver:
                return new MenuType[] { MenuType.Restart };
            default:
                return new MenuType[0];
            }
        }

        void OnClickMenu(MenuType type) {
            switch (type) {
            case MenuType.Start:
                StartGame();
                break;
            case MenuType.Restart:
                PushToView(new UnitLibCmd(null, UnitLibCmdType.Destroy));
                StartGame();
                break;
            default:
                break;
            }
        }

        #endregion

        #region 游戏初始化。(演示：游戏物体的更新)

        void StartGame() {
            status = GameStatus.Gaming;
            UpdateMenu();
            playerId = new UnitId(UnitType.Player);
            PushToView(new UnitLibCmd(playerId));
            MonoBehaviourHook.Instance.StartCoroutine(GenerateEnemy());
            PushToView(new InputModel() {
                OnClickSpace = OnClickSpace,
            });
            PushToView(new CollideTrigger() {
                id = playerId,
                onCollision = OnCollision,
            });
        }

        IEnumerator GenerateEnemy() {
            int idx = 0;
            while (status == GameStatus.Gaming) {
                var id = new UnitId(UnitType.Enemy, idx);
                idx++;
                var movable = new Movable() {
                    id = id,
                    type = MoveType.ToTarget,
                    targetId = playerId,
                    speed = 1f,
                };
                PushToView(new UnitLibCmd(id));
                PushToView(movable);
                yield return new WaitForSeconds(1f);
            }
        }

        void OnClickSpace(Vector3 position) {
            if (status != GameStatus.Gaming) {
                return;
            }
            var movable = new Movable() {
                id = playerId,
                type = MoveType.ToPosition,
                targetData = position,
                speed = 3f,
            };
            PushToView(movable);
        }

        void OnCollision(UnitId self, UnitId other) {
            status = GameStatus.GameOver;
            UpdateMenu();
            PushToView(new UnitLibCmd(playerId, UnitLibCmdType.Destroy));
        }

        #endregion
    }
}