﻿using System.Windows.Input;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void DrawingSurface_OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            _camera.Distance += mouseWheelEventArgs.Delta;
        }
    }
}