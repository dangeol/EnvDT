﻿using FriendStorage.UI.Dialogs;
using System.Windows;

namespace EnvDT.UI.Data.Dialogs
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowYesNoDialog(string title, string message)
        {
            return new YesNoDialog(title, message)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            }.ShowDialog().GetValueOrDefault()
                ? MessageDialogResult.Yes
                : MessageDialogResult.No;
        }
    }
}