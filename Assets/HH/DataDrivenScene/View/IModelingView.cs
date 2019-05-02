using HH.DataDrivenScene.ViewCtrler;
using System;

namespace HH.DataDrivenScene.View
{
    public interface IModelingView : IModelingViewController
    {
        bool AutoRegister { get; }
        Type ModelType { get; }
    }
}
