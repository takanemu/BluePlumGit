#region License
// <copyright>
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
// </copyright>
#endregion

namespace GitlabTool.Win32
{
    using System;
    
    [Flags]
    public enum SWP : int
    {
        NOSIZE = 0x1,
        NOMOVE = 0x2,
        NOZORDER = 0x4,
        NOREDRAW = 0x8,
        NOACTIVATE = 0x10,
        FRAMECHANGED = 0x20,
        SHOWWINDOW = 0x0040,
        NOOWNERZORDER = 0x200,
        NOSENDCHANGING = 0x0400,
        ASYNCWINDOWPOS = 0x4000,
    }
}
