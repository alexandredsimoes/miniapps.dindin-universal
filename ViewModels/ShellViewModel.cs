using GalaSoft.MvvmLight.Command;
using DinDinPro.Universal.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DinDinPro.Universal.ViewModels
{
    public class ShellViewModel : GalaSoft.MvvmLight.ObservableObject
    {
        private ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem>();
        private MenuItem selectedMenuItem;
        private bool isSplitViewPaneOpen;

        public ShellViewModel()
        {
            this.ToggleSplitViewPaneCommand = new RelayCommand(() => this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen);
        }

        public RelayCommand ToggleSplitViewPaneCommand { get; private set; }

        public bool IsSplitViewPaneOpen
        {
            get { return this.isSplitViewPaneOpen; }
            set { Set(ref this.isSplitViewPaneOpen, value); }
        }

        public MenuItem SelectedMenuItem
        {
            get { return this.selectedMenuItem; }
            set
            {
                if (Set(ref this.selectedMenuItem, value)) {
                    RaisePropertyChanged(() => SelectedPageType);

                    // auto-close split view pane
                    this.IsSplitViewPaneOpen = false;
                }
            }
        }

        public Type SelectedPageType
        {
            get
            {
                if (this.selectedMenuItem != null) {
                    return this.selectedMenuItem.PageType;
                }
                return null;
            }
            set
            {
                // select associated menu item
                this.SelectedMenuItem = this.menuItems.FirstOrDefault(m => m.PageType == value);
            }
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return this.menuItems; }
        }
    }
}
