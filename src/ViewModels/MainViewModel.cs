using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ofxware.Models;

using ofxware.Commands;

namespace ofxware.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        Core _core;

        private ObservableCollection<Account> _accountCollection;
        private CollectionViewSource _accounts;

        string _datafile;

        #endregion

        #region Commands

        private DelegateCommand exitCommand;
        private DelegateCommand startCommand;
        private DelegateCommand processCommand;

        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new DelegateCommand(Exit);
                }
                return exitCommand;
            }
        }

        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                {
                    startCommand = new DelegateCommand(Start);
                }
                return startCommand;
            }
        }

        public ICommand ProcessCommand
        {
            get
            {
                if (processCommand == null)
                {
                    processCommand = new DelegateCommand(Process);
                }
                return processCommand;
            }
        }

        #endregion

        #region Properties

        public CollectionViewSource Accounts
        {
            get { return _accounts; }
        }

        public string Filename
        {
            get { return _datafile; }
            set { _datafile = value; }
        }

        public Account SelectedItem { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            _datafile = "";

            _core = new Core();

            _accounts = new CollectionViewSource();
            _accountCollection = new ObservableCollection<Account>();
            _core.Accounts.CollectionChanged += new NotificationCollectionChangedEventHandler(OnAccountCollectionChanged);
            _accounts.Source = _accountCollection;

            _core.Start();
        }

        #endregion

        #region Methods

        private void OnAccountCollectionChanged(object sender, NotificationCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate()
            {
                if (e.Action == NotificationCollectionChangedAction.Reset)
                {
                    _accountCollection.Clear();
                }
                else if (e.Action == NotificationCollectionChangedAction.Add)
                {
                    foreach (object obj in e.NewItems)
                    {
                        _accountCollection.Add((Account)obj);
                    }
                }
                else if (e.Action == NotificationCollectionChangedAction.Remove)
                {
                    foreach (object obj in e.OldItems)
                    {
                        _accountCollection.Remove(obj as Account);
                    }
                }
            }));
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void Start()
        {
            _core.Start();
        }

        private void Process()
        {
            _core.Process(Filename, SelectedItem);
        }

        #endregion
    }
}
