#pragma checksum "C:\Users\Q\Documents\00000_EVERYTHING\20000_MEDIA\26000_PROGRAMMING\26100_PROJECTS\26110_C_SHARP\26111_UWP\UWP_PROJECT_06\UWP_PROJECT_06\Views\Notes\SourcesPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D42AC5C67DFA96546AC2936AAB0035513EDCFD89E4198DE568FA32B3BEB955F6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UWP_PROJECT_06.Views.Notes
{
    partial class SourcesPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.CommandParameter = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class SourcesPage_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            ISourcesPage_Bindings
        {
            private global::UWP_PROJECT_06.Views.Notes.SourcesPage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj8;
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj16;
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj18;
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj22;
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj30;
            private global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction obj32;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj8CommandParameterDisabled = false;
            private static bool isobj16CommandParameterDisabled = false;
            private static bool isobj18CommandParameterDisabled = false;
            private static bool isobj22CommandParameterDisabled = false;
            private static bool isobj30CommandParameterDisabled = false;
            private static bool isobj32CommandParameterDisabled = false;

            public SourcesPage_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 384 && columnNumber == 112)
                {
                    isobj8CommandParameterDisabled = true;
                }
                else if (lineNumber == 373 && columnNumber == 116)
                {
                    isobj16CommandParameterDisabled = true;
                }
                else if (lineNumber == 354 && columnNumber == 113)
                {
                    isobj18CommandParameterDisabled = true;
                }
                else if (lineNumber == 189 && columnNumber == 109)
                {
                    isobj22CommandParameterDisabled = true;
                }
                else if (lineNumber == 176 && columnNumber == 109)
                {
                    isobj30CommandParameterDisabled = true;
                }
                else if (lineNumber == 157 && columnNumber == 106)
                {
                    isobj32CommandParameterDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 8: // Views\Notes\SourcesPage.xaml line 384
                        this.obj8 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
                        break;
                    case 16: // Views\Notes\SourcesPage.xaml line 373
                        this.obj16 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
                        break;
                    case 18: // Views\Notes\SourcesPage.xaml line 354
                        this.obj18 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
                        break;
                    case 22: // Views\Notes\SourcesPage.xaml line 189
                        this.obj22 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
                        break;
                    case 30: // Views\Notes\SourcesPage.xaml line 176
                        this.obj30 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
                        break;
                    case 32: // Views\Notes\SourcesPage.xaml line 157
                        this.obj32 = (global::Microsoft.Xaml.Interactions.Core.InvokeCommandAction)target;
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

            // ISourcesPage_Bindings

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
                    this.dataRoot = (global::UWP_PROJECT_06.Views.Notes.SourcesPage)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::UWP_PROJECT_06.Views.Notes.SourcesPage obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_UnknownSourcesList(obj.UnknownSourcesList, phase);
                        this.Update_UnknownSourceTypes(obj.UnknownSourceTypes, phase);
                        this.Update_AutosuggestUnknownSource(obj.AutosuggestUnknownSource, phase);
                        this.Update_SourcesList(obj.SourcesList, phase);
                        this.Update_SourceTypesComboBox(obj.SourceTypesComboBox, phase);
                        this.Update_Autosuggest(obj.Autosuggest, phase);
                    }
                }
            }
            private void Update_UnknownSourcesList(global::Windows.UI.Xaml.Controls.ListView obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 384
                    if (!isobj8CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj8, obj, null);
                    }
                }
            }
            private void Update_UnknownSourceTypes(global::Windows.UI.Xaml.Controls.ComboBox obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 373
                    if (!isobj16CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj16, obj, null);
                    }
                }
            }
            private void Update_AutosuggestUnknownSource(global::Windows.UI.Xaml.Controls.AutoSuggestBox obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 354
                    if (!isobj18CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj18, obj, null);
                    }
                }
            }
            private void Update_SourcesList(global::Windows.UI.Xaml.Controls.ListView obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 189
                    if (!isobj22CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj22, obj, null);
                    }
                }
            }
            private void Update_SourceTypesComboBox(global::Windows.UI.Xaml.Controls.ComboBox obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 176
                    if (!isobj30CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj30, obj, null);
                    }
                }
            }
            private void Update_Autosuggest(global::Windows.UI.Xaml.Controls.AutoSuggestBox obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\Notes\SourcesPage.xaml line 157
                    if (!isobj32CommandParameterDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_Xaml_Interactions_Core_InvokeCommandAction_CommandParameter(this.obj32, obj, null);
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
            case 2: // Views\Notes\SourcesPage.xaml line 22
                {
                    this.Sources = (global::Windows.UI.Xaml.Data.CollectionViewSource)(target);
                }
                break;
            case 3: // Views\Notes\SourcesPage.xaml line 23
                {
                    this.UnknownSources = (global::Windows.UI.Xaml.Data.CollectionViewSource)(target);
                }
                break;
            case 4: // Views\Notes\SourcesPage.xaml line 511
                {
                    this.DictionaryFrame = (global::Windows.UI.Xaml.Controls.Frame)(target);
                }
                break;
            case 5: // Views\Notes\SourcesPage.xaml line 328
                {
                    this.AutosuggestUnknownSource = (global::Windows.UI.Xaml.Controls.AutoSuggestBox)(target);
                    ((global::Windows.UI.Xaml.Controls.AutoSuggestBox)this.AutosuggestUnknownSource).KeyUp += this.AutosuggestUnknown_KeyDown;
                }
                break;
            case 6: // Views\Notes\SourcesPage.xaml line 360
                {
                    this.UnknownSourceTypes = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 7: // Views\Notes\SourcesPage.xaml line 379
                {
                    this.UnknownSourcesList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 15: // Views\Notes\SourcesPage.xaml line 366
                {
                    global::Windows.UI.Xaml.Input.KeyboardAccelerator element15 = (global::Windows.UI.Xaml.Input.KeyboardAccelerator)(target);
                    ((global::Windows.UI.Xaml.Input.KeyboardAccelerator)element15).Invoked += this.FocusOnUnknownSearch;
                }
                break;
            case 17: // Views\Notes\SourcesPage.xaml line 334
                {
                    global::Windows.UI.Xaml.Input.KeyboardAccelerator element17 = (global::Windows.UI.Xaml.Input.KeyboardAccelerator)(target);
                    ((global::Windows.UI.Xaml.Input.KeyboardAccelerator)element17).Invoked += this.FocusOnUnknownSearch;
                }
                break;
            case 19: // Views\Notes\SourcesPage.xaml line 132
                {
                    this.Autosuggest = (global::Windows.UI.Xaml.Controls.AutoSuggestBox)(target);
                    ((global::Windows.UI.Xaml.Controls.AutoSuggestBox)this.Autosuggest).KeyUp += this.Autosuggest_KeyDown;
                }
                break;
            case 20: // Views\Notes\SourcesPage.xaml line 163
                {
                    this.SourceTypesComboBox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 21: // Views\Notes\SourcesPage.xaml line 182
                {
                    this.SourcesList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 29: // Views\Notes\SourcesPage.xaml line 169
                {
                    global::Windows.UI.Xaml.Input.KeyboardAccelerator element29 = (global::Windows.UI.Xaml.Input.KeyboardAccelerator)(target);
                    ((global::Windows.UI.Xaml.Input.KeyboardAccelerator)element29).Invoked += this.FocusOnLanguages;
                }
                break;
            case 31: // Views\Notes\SourcesPage.xaml line 138
                {
                    global::Windows.UI.Xaml.Input.KeyboardAccelerator element31 = (global::Windows.UI.Xaml.Input.KeyboardAccelerator)(target);
                    ((global::Windows.UI.Xaml.Input.KeyboardAccelerator)element31).Invoked += this.FocusOnSearch;
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
            case 1: // Views\Notes\SourcesPage.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    SourcesPage_obj1_Bindings bindings = new SourcesPage_obj1_Bindings();
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

