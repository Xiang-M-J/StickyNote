# StickyNote
一个简单的便签程序

## 项目组成

### 文件简介
1. resources文件夹

	存放图片文件
2. MainWindow.xaml (MainWindow.xmal.cs)

	主窗口

3. Setting.xaml(Setting.xaml.cs)
	
	设置主窗口背景颜色和文件颜色

### 项目组成

```
.
├── StickyNote
│   ├── App.config
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── bin
│   │   ├── Debug
│   │   │   ├── StickyNote.exe
│   │   │   ├── StickyNote.exe.config
│   │   │   └── StickyNote.pdb
│   │   └── Release
│   │       ├── configure.xml
│   │       ├── StickyNote.exe
│   │       ├── StickyNote.exe.config
│   │       └── StickyNote.pdb
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── MyMessageBox.xaml
│   ├── MyMessageBox.xaml.cs
|   |
│   ├── Properties
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   ├── resources
│   │   ├── cancel.ico
│   │   ├── close.png
│   │   ├── Infomation.png
│   │   ├── joy.png
│   │   ├── life.png
│   │   ├── main.png
│   │   ├── minlize.ico
│   │   ├── more.png
│   │   ├── other.png
│   │   ├── Setting.png
│   │   ├── StickyNote.ico
│   │   ├── StickyNote.png
│   │   ├── study.png
│   │   ├── test.xml
│   │   └── work.png
│   ├── Setting.xaml
│   ├── Setting.xaml.cs
│   ├── StickyNote.csproj
│   └── template.xml
└── StickyNote.sln

```

### 使用注意

1. 在使用本程序时，会在与.exe文件同路径的文件夹中生成configure.xml文件，该文件的作用是存储程序使用时的数据

以下为简单示例 `info`为待办事项内容 `time` 为截止时间
```xml
<?xml version="1.0" encoding="utf-8"?>
<ListBoxItem>
  <item id="0">
    <info>*****</info>
    <time>**</time>
    <PicIndex>0</PicIndex>
  </item>
  <item id="1">
    <info>*******</info>
    <time>***</time>
    <PicIndex>4</PicIndex>
  </item>
</ListBoxItem>
```

2. 使用鼠标选中一条待办事项后，使用delete键可以删除该事项

3. 双击一条代办事项，可以弹出该条事项的截止时间

4. 点击右下角的带有下载符号的按钮，可以导出待办事项

### 存在问题

1. 主窗口在向左拉伸时，会出现窗口抖动的问题

2. 由于个人水平有限，所以未能向主窗口中添加右键菜单

3. 该程序的调试的时间较短，所以可能存在一些bug还未发现

### 项目说明

| | |
:-:|:-:
IDE| Visual Studio 2017
语言|C#
用户界面框架|WPF
.Net版本|4.6.1
