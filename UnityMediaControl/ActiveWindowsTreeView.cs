using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityMediaControl
{
    internal class ActiveWindowsTreeViewItem : TreeViewItem
    {
        public const int InvalidCounter = -1;

        public string WindowTitle;
        public string WindowClass;
        public bool Selected;
        public int Counter;

        public ActiveWindowsTreeViewItem(int id, int depth, string title, string windClass, bool selected = false) : base(id, depth)
        {
            WindowTitle = title;
            WindowClass = windClass;
            Counter = InvalidCounter;
            Selected = selected;
            UpdateName();
        }

        public void UpdateName()
        {
            displayName = string.Format("{0} [{1}] ({2})", WindowTitle, WindowClass, id);
        }
    }

    internal class ActiveWindowsTreeView : TreeView
    {
        public ActiveWindowsTreeView(TreeViewState state) : base(state)
        {
            showBorder = true;
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1);
            root.children = new List<TreeViewItem>();

            WindowDescription[] windows = User32Interop.EnumerateWindows();
            for (int i = 0; i < windows.Length; i++)
            {
                WindowDescription window = windows[i];
                ActiveWindowsTreeViewItem item = new ActiveWindowsTreeViewItem((int)window.Handle, 0, window.Name, window.ClassName);
                root.AddChild(item);
            }
            return root;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return false;
        }

        protected override void BeforeRowsGUI()
        {
            int counter = 0;
            foreach (TreeViewItem item in rootItem.children)
            {
                ActiveWindowsTreeViewItem awt = item as ActiveWindowsTreeViewItem;
                if (awt == null)
                    continue;

                awt.Counter = counter;
                counter++;
            }

            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            ActiveWindowsTreeViewItem windowItem = args.item as ActiveWindowsTreeViewItem;
            if (windowItem != null)
            {
                bool newState = windowItem.Selected;
                newState = GUI.Toggle(new Rect(args.rowRect.x, args.rowRect.y, 16f, 16f), newState, "");
                if (newState != windowItem.Selected)
                {
                    if (GetSelection().Contains(windowItem.id))
                    {
                        IList<int> selection = GetSelection();
                        foreach (int id in selection)
                        {
                            ActiveWindowsTreeViewItem item = FindItem(id, rootItem) as ActiveWindowsTreeViewItem;
                            item.Selected = newState;
                            // TODO: do something when a window is selected
                        }
                    }
                    else
                    {
                        windowItem.Selected = newState;
                    }
                }

                base.RowGUI(args);
            }
            else
                base.RowGUI(args);
        }

        private void ReloadAndSelect(IList<int> hashCodes)
        {
            Reload();
            SetSelection(hashCodes, TreeViewSelectionOptions.RevealAndFrame);
            SelectionChanged(hashCodes);
        }

    }
}
