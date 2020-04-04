namespace Fabric.Internal.Editor.Controller
{
    using Fabric.Internal.Editor.View;
    using System;

    public enum KitControllerStatus { NextPage, LastPage, CurrentPage };
    internal interface KitController
    {
        KitControllerStatus PageFromState(out Page page);
        string DisplayName();
        Version Version();
    }
}