﻿#pragma checksum "C:\Users\Q\Documents\00000_EVERYTHING\20000_MEDIA\26000_PROGRAMMING\26100_PROJECTS\26110_C_SHARP\26111_UWP\UWP_PROJECT_06\UWP_PROJECT_06\MainPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5CFE67DCB1C717299D3D24FDB5A4678EFD6EAB4E95DBD6EAFB049AAD0D106378"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UWP_PROJECT_06
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(global::Windows.UI.Xaml.Controls.Primitives.ButtonBase obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.CommandParameter = value;
            }
            public static void Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.CommandParameter = value;
            }
            public static void Set_Microsoft_UI_Xaml_Controls_TabView_AddTabButtonCommandParameter(global::Microsoft.UI.Xaml.Controls.TabView obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.AddTabButtonCommandParameter = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class MainPage_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IMainPage_Bindings
        {
            private global::UWP_PROJECT_06.MainPage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.Button obj4;
            private global::Microsoft.UI.Xaml.Controls.TabView obj5;
            private global::Windows.UI.Xaml.Controls.Button obj7;
            private global::Windows.UI.Xaml.Controls.Button obj8;
            private global::Windows.UI.Xaml.Controls.Button obj9;
            private global::Windows.UI.Xaml.Controls.Button obj10;
            private global::Windows.UI.Xaml.Controls.Button obj11;
            private global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj13;
            private global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj14;
            private global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj15;
            private global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj16;
            private global::Windows.UI.Xaml.Controls.MenuFlyoutItem obj17;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj4CommandParameterDisabled = false;
            private static bool isobj5AddTabButtonCommandParameterDisabled = false;
            private static bool isobj7CommandParameterDisabled = false;
            private static bool isobj8CommandParameterDisabled = false;
            private static bool isobj9CommandParameterDisabled = false;
            private static bool isobj10CommandParameterDisabled = false;
            private static bool isobj11CommandParameterDisabled = false;
            private static bool isobj13CommandParameterDisabled = false;
            private static bool isobj14CommandParameterDisabled = false;
            private static bool isobj15CommandParameterDisabled = false;
            private static bool isobj16CommandParameterDisabled = false;
            private static bool isobj17CommandParameterDisabled = false;

            public MainPage_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 359 && columnNumber == 75)
                {
                    isobj4CommandParameterDisabled = true;
                }
                else if (lineNumber == 288 && columnNumber == 31)
                {
                    isobj5AddTabButtonCommandParameterDisabled = true;
                }
                else if (lineNumber == 170 && columnNumber == 71)
                {
                    isobj7CommandParameterDisabled = true;
                }
                else if (lineNumber == 182 && columnNumber == 66)
                {
                    isobj8CommandParameterDisabled = true;
                }
                else if (lineNumber == 194 && columnNumber == 65)
                {
                    isobj9CommandParameterDisabled = true;
                }
                else if (lineNumber == 231 && columnNumber == 96)
                {
                    isobj10CommandParameterDisabled = true;
                }
                else if (lineNumber == 118 && columnNumber == 76)
                {
                    isobj11CommandParameterDisabled = true;
                }
                else if (lineNumber == 54 && columnNumber == 110)
                {
                    isobj13CommandParameterDisabled = true;
                }
                else if (lineNumber == 61 && columnNumber == 110)
                {
                    isobj14CommandParameterDisabled = true;
                }
                else if (lineNumber == 68 && columnNumber == 110)
                {
                    isobj15CommandParameterDisabled = true;
                }
                else if (lineNumber == 75 && columnNumber == 109)
                {
                    isobj16CommandParameterDisabled = true;
                }
                else if (lineNumber == 44 && columnNumber == 90)
                {
                    isobj17CommandParameterDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 4: // MainPage.xaml line 358
                        this.obj4 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 5: // MainPage.xaml line 285
                        this.obj5 = (global::Microsoft.UI.Xaml.Controls.TabView)target;
                        break;
                    case 7: // MainPage.xaml line 168
                        this.obj7 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 8: // MainPage.xaml line 180
                        this.obj8 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 9: // MainPage.xaml line 192
                        this.obj9 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 10: // MainPage.xaml line 229
                        this.obj10 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 11: // MainPage.xaml line 117
                        this.obj11 = (global::Windows.UI.Xaml.Controls.Button)target;
                        break;
                    case 13: // MainPage.xaml line 54
                        this.obj13 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)target;
                        break;
                    case 14: // MainPage.xaml line 61
                        this.obj14 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)target;
                        break;
                    case 15: // MainPage.xaml line 68
                        this.obj15 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)target;
                        break;
                    case 16: // MainPage.xaml line 75
                        this.obj16 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)target;
                        break;
                    case 17: // MainPage.xaml line 44
                        this.obj17 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)target;
                        break;
                    default:
                        break;
                }
            }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IMainPage_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = (global::UWP_PROJECT_06.MainPage)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::UWP_PROJECT_06.MainPage obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_rightPane(obj.rightPane, phase);
                        this.Update_tabView(obj.tabView, phase);
                        this.Update_leftPane(obj.leftPane, phase);
                    }
                }
            }
            private void Update_rightPane(global::Windows.UI.Xaml.Controls.Grid obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // MainPage.xaml line 358
                    if (!isobj4CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj4, obj, null);
                    }
                    // MainPage.xaml line 68
                    if (!isobj15CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(this.obj15, obj, null);
                    }
                }
            }
            private void Update_tabView(global::Microsoft.UI.Xaml.Controls.TabView obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // MainPage.xaml line 285
                    if (!isobj5AddTabButtonCommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TabView_AddTabButtonCommandParameter(this.obj5, obj, null);
                    }
                    // MainPage.xaml line 168
                    if (!isobj7CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj7, obj, null);
                    }
                    // MainPage.xaml line 180
                    if (!isobj8CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj8, obj, null);
                    }
                    // MainPage.xaml line 192
                    if (!isobj9CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj9, obj, null);
                    }
                    // MainPage.xaml line 229
                    if (!isobj10CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj10, obj, null);
                    }
                    // MainPage.xaml line 54
                    if (!isobj13CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(this.obj13, obj, null);
                    }
                    // MainPage.xaml line 61
                    if (!isobj14CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(this.obj14, obj, null);
                    }
                    // MainPage.xaml line 44
                    if (!isobj17CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(this.obj17, obj, null);
                    }
                }
            }
            private void Update_leftPane(global::Windows.UI.Xaml.Controls.Grid obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // MainPage.xaml line 117
                    if (!isobj11CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Primitives_ButtonBase_CommandParameter(this.obj11, obj, null);
                    }
                    // MainPage.xaml line 75
                    if (!isobj16CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_MenuFlyoutItem_CommandParameter(this.obj16, obj, null);
                    }
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 249
                {
                    this.leftPane = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // MainPage.xaml line 376
                {
                    this.rightPane = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 5: // MainPage.xaml line 285
                {
                    this.tabView = (global::Microsoft.UI.Xaml.Controls.TabView)(target);
                    ((global::Microsoft.UI.Xaml.Controls.TabView)this.tabView).TabCloseRequested += this.tabView_TabCloseRequested;
                }
                break;
            case 6: // MainPage.xaml line 320
                {
                    this.wv = (global::Windows.UI.Xaml.Controls.WebView)(target);
                }
                break;
            case 12: // MainPage.xaml line 34
#pragma warning disable 0618  //   Warning on Deprecated usage
                {
                    this.FileMenu = (global::Microsoft.Toolkit.Uwp.UI.Controls.MenuItem)(target);
#pragma warning restore 0618
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1: // MainPage.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    MainPage_obj1_Bindings bindings = new MainPage_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Windows.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

