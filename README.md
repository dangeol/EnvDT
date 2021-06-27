# EnvDT
> A simple tool for reading analytical values from laboratory reports and evaluating them according to environmental guidelines.

## Local development
Initialize the local database using the Package Manager Console of Visual Studio:

`Add-Migration InitialCreate`\
`Update-Database`

Please copy then `envdt.db` from the root into the debug folder, e.g.:
`EnvDT\EnvDT.UI\bin\Debug\net5.0-windows`

Data seeds for German guidelines are available, but are not part of the open source project.