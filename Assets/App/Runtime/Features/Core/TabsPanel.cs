using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Features.Core
{
    public class TabsPanel : MonoBehaviour
    {
        [SerializeField]
        private TabButtonComponent[] _tabs;

        private TabId _selectedTab;
        private bool _hasSelectedTab;
        private bool _isBound;
        private readonly List<IDisposable> _subscriptions = new();
        private readonly Subject<TabId> _selected = new();

        public Observable<TabId> Selected => _selected;

        public TabId SelectedTab => _selectedTab;

        public void Initialize(TabId initialTab)
        {
            BindTabs();
            Select(initialTab, notify: false);
        }

        public void Select(TabId tabId)
        {
            Select(tabId, notify: true);
        }

        private void OnEnable()
        {
            BindTabs();
            ApplySelection();
        }

        private void OnDisable()
        {
            UnbindTabs();
        }

        private void Select(TabId tabId, bool notify)
        {
            if (_hasSelectedTab && _selectedTab == tabId)
            {
                return;
            }

            _selectedTab = tabId;
            _hasSelectedTab = true;

            ApplySelection();

            if (notify)
            {
                _selected.OnNext(tabId);
            }
        }

        private void BindTabs()
        {
            if (_isBound)
            {
                return;
            }

            foreach (var tab in _tabs)
            {
                _subscriptions.Add(
                    tab
                        .Clicked
                        .Subscribe((this, tab), static (_, state) => state.Item1.Select(state.Item2.Id)));
            }

            _isBound = true;
        }

        private void UnbindTabs()
        {
            if (!_isBound)
            {
                return;
            }

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
            _isBound = false;
        }

        private void ApplySelection()
        {
            if (!_hasSelectedTab)
            {
                return;
            }

            foreach (var tab in _tabs)
            {
                tab.SetSelected(tab.Id == _selectedTab);
            }
        }

        private void OnDestroy()
        {
            UnbindTabs();
            _selected.Dispose();
        }
    }
}
