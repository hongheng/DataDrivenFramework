using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HH.DataDrivenFramework.Sample {

    public enum GameStatus {
        Null,
        Init,
        Gaming,
        GameOver,
    }

    public class UnitId {
        public UnitType type;
        public int index;

        public UnitId(UnitType type, int index = 0) {
            this.type = type;
            this.index = index;
        }
    }

    public enum UnitType {
        Player = 0,
        Enemy = 1,
    }

    public enum UnitLibCmdType {
        Create,
        Destroy,
    }

    public class UnitLibCmd {
        public UnitId id;
        public UnitLibCmdType type;

        public UnitLibCmd(UnitId id, UnitLibCmdType type = UnitLibCmdType.Create) {
            this.id = id;
            this.type = type;
        }
    }

    public class Movable {
        public UnitId id;
        public float speed;
        public MoveType type;
        public UnitId targetId;
        public Vector3 targetData;
    }

    public enum MoveType {
        ToPosition,
        ToTarget,
    }

    public class CollideTrigger {
        public UnitId id;
        public Action<UnitId, UnitId> onCollision;
    }

    public class InputModel {
        public Action<Vector3> OnClickSpace;
    }

    #region useless

    public class UnitCmd<T> {
        public UnitId id;
        public T component;

        public UnitCmd(UnitId id, T component) {
            this.id = id;
            this.component = component;
        }
    }

    #endregion

    #region useless

    public class GameModel {
        public GameStatus status;
        public Player player;
        public List<Enemy> enemies;
    }

    public class Player {
        public UnitId id;
        public Movable movable;
        public CollideTrigger Collide;
    }

    public class Enemy {
        public UnitId id;
        public Movable movable;
    }

    #endregion
}