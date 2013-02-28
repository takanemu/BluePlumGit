#region Apache License
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace GitlabTool.Win32
{
    using System;
    using System.Runtime.InteropServices;
    
    public static class NativeMethods
    {
        public static WS GetWindowLong(this IntPtr hWnd)
        {
            return (WS)NativeMethods.GetWindowLong(hWnd, (int)GWL.STYLE);
        }
        
        public static WSEX GetWindowLongEx(this IntPtr hWnd)
        {
            return (WSEX)NativeMethods.GetWindowLong(hWnd, (int)GWL.EXSTYLE);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static WS SetWindowLong(this IntPtr hWnd, WS dwNewLong)
        {
            return (WS)NativeMethods.SetWindowLong(hWnd, (int)GWL.STYLE, (int)dwNewLong);
        }
        
        public static WSEX SetWindowLongEx(this IntPtr hWnd, WSEX dwNewLong)
        {
            return (WSEX)NativeMethods.SetWindowLong(hWnd, (int)GWL.EXSTYLE, (int)dwNewLong);
        }
        
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
