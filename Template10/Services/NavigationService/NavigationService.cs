﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Template10.Services.NavigationService
{
    public class NavigationService
    {
        private readonly NavigationFacade _frame;
        private const string OnNavigatedFrom = "OnNavigatedFrom";
        private const string OnNavigatedTo = "OnNavigatedTo";
        private const string EmptyNavigation = "1,0";

        string LastNavigationParameter { get; set; /* TODO: persist */ }

        string LastNavigationType { get; set; /* TODO: persist */ }

        public NavigationService(Frame frame)
        {
            _frame = new NavigationFacade(frame);
            _frame.Navigating += (s, e) => NavigateFrom(false);
            _frame.Navigated += (s, e) => NavigateTo(e.NavigationMode, e.Parameter);
        }

        void NavigateFrom(bool suspending)
        {
            var page = _frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext;
                if (dataContext != null)
                {
                    // navigationService will not depend on Mvvm namespace
                    var method = dataContext.GetType()
                        .GetRuntimeMethod(OnNavigatedFrom, new[] {
                            typeof(Dictionary<string, object>),
                            typeof(Boolean)
                        });
                    if (method != null)
                    {
                        // TODO: get existing state
                        method.Invoke(dataContext, new object[] { null, suspending });
                    }
                }
            }
        }

        void NavigateTo(NavigationMode mode, string parameter)
        {
            LastNavigationParameter = parameter;
            LastNavigationType = _frame.Content.GetType().FullName;

            if (mode == NavigationMode.New)
            {
                // TODO: clear existing state
            }

            var page = _frame.Content as FrameworkElement;
            if (page != null)
            {
                var dataContext = page.DataContext;
                if (dataContext != null)
                {
                    // navigationService will not depend on Mvvm namespace
                    var method = dataContext.GetType()
                        .GetRuntimeMethod(OnNavigatedTo, new[] {
                            typeof(string),
                            typeof(NavigationMode),
                            typeof(Dictionary<string, object>)
                        });
                    if (method != null)
                    {
                        // TODO: get existing state
                        method.Invoke(dataContext, new object[] { parameter, mode, null });
                    }
                }
            }
        }

        public bool Navigate(Type page, string parameter = null)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            if (page.FullName.Equals(LastNavigationType)
                && parameter == LastNavigationParameter)
                return false;
            return _frame.Navigate(page, parameter);
        }

        public void RestoreSavedNavigation() { /* TODO */ }

        public void GoBack() { _frame.GoBack(); }

        public bool CanGoBack { get { return _frame.CanGoBack; } }

        public void GoForward() { _frame.GoForward(); }

        public bool CanGoForward { get { return _frame.CanGoForward; } }

        public void ClearHistory() { _frame.SetNavigationState(EmptyNavigation); }

        public void Suspending() { NavigateFrom(true); }

        public void Show(SettingsFlyout flyout, string parameter = null)
        {
            if (flyout == null)
                throw new ArgumentNullException(nameof(flyout));
            var dataContext = flyout.DataContext;
            if (dataContext != null)
            {
                // navigationService will not depend on Mvvm namespace
                var method = dataContext.GetType()
                    .GetRuntimeMethod(OnNavigatedTo, new[] {
                        typeof(string),
                        typeof(NavigationMode),
                        typeof(Dictionary<string, object>)
                    });
                if (method != null)
                {
                    // TODO: get existing state
                    method.Invoke(dataContext, new object[] { parameter, NavigationMode.New, null });
                }
            }
            flyout.Show();
        }

        public Type CurrentPageType { get { return _frame.CurrentPageType; } }

        public string CurrentPageParam { get { return _frame.CurrentPageParam; } }
    }
}

