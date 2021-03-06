﻿using Autofac;
using EnvDT.UI.ViewModel;

namespace EnvDT.UI.Startup
{
    public class ViewModelModule : Module
    {
		protected override void Load(ContainerBuilder builder)
		{
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<MainViewModelDesignTime>().AsSelf();
            builder.RegisterType<ProjectViewModel>().As<IProjectViewModel>();
            builder.RegisterType<ProjectDetailViewModel>().As<IProjectDetailViewModel>();
            builder.RegisterType<NavItemViewModel>().AsSelf();
            builder.RegisterType<MainTabViewModel>().As<IMainTabViewModel>();
            builder.RegisterType<SampleDetailViewModel>().As<ISampleDetailViewModel>();
            builder.RegisterType<LabReportViewModel>().As<ILabReportViewModel>();
            builder.RegisterType<SettingsDetailViewModel>().As<ISettingsDetailViewModel>();
            builder.RegisterType<InfoDetailViewModel>().As<IInfoDetailViewModel>();
            builder.RegisterType<MissingParamDialogViewModel>().As<IMissingParamDialogViewModel>();
            builder.RegisterType<SampleEditDialogViewModel>().As<ISampleEditDialogViewModel>();
            builder.RegisterType<LabViewModel>().As<ILabViewModel>();
            builder.RegisterType<LabDetailViewModel>().As<ILabDetailViewModel>();
            builder.RegisterType<ConfigXlsxDetailViewModel>().As<IConfigXlsxDetailViewModel>();
            builder.RegisterType<ConfigCsvDetailViewModel>().As<IConfigCsvDetailViewModel>();
        }
	}
}
