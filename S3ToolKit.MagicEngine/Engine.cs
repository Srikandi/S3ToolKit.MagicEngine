﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using S3ToolKit.MagicEngine.Core;
using System.Collections.ObjectModel;
using S3ToolKit.Utils.Registry;

namespace S3ToolKit.MagicEngine
{

    // Singleton using Lazy<T> from http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
    public class Engine : INotifyPropertyChanged 
    {
        #region Singleton
        private static readonly Lazy<Engine> _instance = new Lazy<Engine>(() => new Engine());

        public static Engine Instance { get { return _instance.Value; } }
        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        // ActiveSharp property change notification implementation
        // use this layout to set properties:
        //     public int Foo
        //     {
        //       get { return _foo; }
        //       set { SetValue(ref _foo, value); }   // assigns value and does prop change notification, all in one line
        //     }
        protected void SetValue<T>(ref T field, T value)
        {
            field = value;   //Actually assign the new value
            PropertyInfo changedProperty = ActiveSharp.PropertyMapping.PropertyMap.GetProperty(this, ref field);

            OnPropertyChanged(changedProperty.Name);
        }
        #endregion

        #region Fields
        private ObservableCollection<GameVersionEntry> _GameInfo;
        #endregion

        #region Properties
        public ObservableCollection<GameVersionEntry> GameInfo { get { return GetGameInfo(); } }
        #endregion

        #region Private Helpers
        private ObservableCollection<GameVersionEntry> GetGameInfo()
        {
            if (_GameInfo == null)
            {
                _GameInfo = GenerateGameInfo();
            }

            return _GameInfo;
        }

        private ObservableCollection<GameVersionEntry> GenerateGameInfo()
        {
            ObservableCollection<GameVersionEntry> temp = new ObservableCollection<GameVersionEntry>();

            SortedDictionary<int, InstalledGameEntry> List = new SortedDictionary<int, InstalledGameEntry>();

            foreach (var entry in InstallationInfo.Instance.Packs)
            {
                if (entry.IsGame)
                {
                    List.Add(entry.ProductID, entry);
                }
            }

            foreach (var entry in List)
            {
                if (entry.Value.IsGame)
                {
                    temp.Add(new GameVersionEntry(entry.Value));
                }
            }

            return temp;
        }

        private string GetLauncherVersion()
        {
            Version Ver = Assembly.GetEntryAssembly().GetName().Version;
            return string.Format("CC Magic [{0}.{1}r{2} Build: {3}]", Ver.Major, Ver.Minor, Ver.Revision, Ver.Build);
        }
        private string GetWindowsVersion()
        {
            return System.Environment.OSVersion.ToString();
        }
        private string GetWindowsBits()
        {
            string temp;
            if (System.Environment.Is64BitProcess)
            {
                temp = "64-bit Application on ";
            }
            else
            {
                temp = "32-bit Application on ";
            }
            if (System.Environment.Is64BitOperatingSystem)
            {
                temp += "64-bit Windows";
            }
            else
            {
                temp += "32-bit Windows";
            }

            return temp;
        }
        #endregion
    }
}