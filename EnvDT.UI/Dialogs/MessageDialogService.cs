﻿using EnvDT.UI.ViewModel;
using System.Windows;

namespace EnvDT.UI.Dialogs
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

        public MessageDialogResult ShowOkDialog(string title, string message)
        {
            return new OkDialog(title, message)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            }.ShowDialog().GetValueOrDefault()
                ? MessageDialogResult.OK
                : MessageDialogResult.OK;
        }

        public MessageDialogResult ShowOkCancelDialog(string title, string message)
        {
            return new OkCancelDialog(title, message)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            }.ShowDialog().GetValueOrDefault()
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        public MessageDialogResult ShowMissingParamDialog(string title, 
            IMissingParamDialogViewModel missingParamDetailVM)
        {
            return new MissingParamDialog(title, missingParamDetailVM)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            }.ShowDialog().GetValueOrDefault()
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        public MessageDialogResult ShowSampleEditDialog(string title,
            ISampleEditDialogViewModel sampleEditDialogViewModel)
        {
            return new SampleEditDialog(title, sampleEditDialogViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow
            }.ShowDialog().GetValueOrDefault()
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }
    }
}
