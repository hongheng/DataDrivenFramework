using System;

namespace HH.DataDrivenScene.ViewCtrler
{
    public class ModelingViewUpdateTask
    {
        public object model;
        public OnUpdateDone onUpdateDone;
    }

    public delegate void OnUpdateDone(object model, Exception exception);
}

