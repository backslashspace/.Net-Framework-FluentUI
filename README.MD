# WinUI 3 for .Net-Framework

![Target Framework](https://img.shields.io/badge/.Net_Framework-4.8.1-green)
![Target Language](https://img.shields.io/badge/C%23_-12.0-green)
![GitHub License](https://img.shields.io/badge/License-GLP--2.0-blue)

## what is this

its in progress, but what the title says

## features

- not a good readme.md yet
- easy to use
- design time preview
- all controls are defined in code behind (no xaml just c#)
- design time dark and light mode (only need to change one value to switch (FluentUI.Theme.IsDarkMode)) 
- automatic theme and accent color change on registry update (for example when theme or color was changed in settings app)
- very very close to the original (1:1 is goal)
- relatively lightweight / performant
- tied to avoid abstraction as much as possible
- tried to avoid allocations as much as possible
- yes Int32 not int

as of now it uses unsafe for some stuff, shouldn't be too hard to use it without unsafe though (used to cast a Boolean to a UInt32 in DWM api wrapper)

## getting started

- detailed description later
	- copy the UI folder + include fonts found in the resources sub dir 
	- for now define custom entry point for your wpf app
	- add the initializer and wmi resource releaser
	- hook all windows to Theme changed event
	- add xaml namespace to xaml designer or similar
	
## resources (soon)

[Controls for Windows apps](https://learn.microsoft.com/en-us/windows/apps/design/controls/)

Controls sampled from

sampled from:
- Windows 11 Settings App