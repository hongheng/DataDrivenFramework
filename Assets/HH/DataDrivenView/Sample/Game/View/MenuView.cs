using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HH.DataDrivenFramework.Sample {

    #region ViewModel 显示语义

    public class ButtonViewModel {
        public string name;
        public Color color;
        public Action onClick;
    }

    public class MenuViewModel {
        public ButtonViewModel[] menus;
    }

    #endregion

    #region Model 状态语义 （讨论：使用ViewModel 和 Model 的优缺点 ）

    public enum MenuType {
        Start,
        Restart,
    }

    public class MenuModel {
        public MenuType[] menus;
        public Action<MenuType> onClick;
    }

    #endregion

    #region DataDrivenView （演示：DataDrivenView的实现）

    public class MenuView : DataDrivenView<MenuModel> {

        public UnitInfoView menu;
        ListView<UnitInfoView> menuList = new ListView<UnitInfoView>();

        protected override void UpdateView(MenuModel m, ref float blockTime) {
            menuList.Update((v, idx) => {
                var type = m.menus[idx];
                string name = "未知按钮";
                Color color = Color.white;
                switch (type) {
                case MenuType.Start:
                    name = "开始游戏";
                    break;
                case MenuType.Restart:
                    name = "再来一局";
                    color = Color.green;
                    break;
                default:
                    break;
                }
                v.Set(name)
                 .Set(color)
                 .Set(() => m.onClick(type));
            }, m.menus.Length, () => {
                var newView = Instantiate(menu, transform, false);
                return newView;
            });
        }
    }

    #endregion
}