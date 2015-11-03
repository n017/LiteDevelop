LiteDevelop
===
**Light-weight and open source IDE**

LiteDevelop is a free Integrated Development Environment (IDE). It serves as a light-weight and open-source alternative to Visual Studio and is able to be launched from a removable drive such as USB sticks or SD-cards. Thanks to its extendibility, it provides you with the opportunity to add any features you like, and remove any features you don’t like. Thus giving you more control on how big and heavy the application will be. 

History
---
Originally the project was designed to have a light-weight IDE for computers with either too old hardware or restricted rights to open or install Microsoft Visual Studio. These computers include for instance thin clients, such as computers on many schools.

Features
---
The standard LiteDevelop suite has the following features:
-   Portable, can be run from a removable drive.
-    Very small compared to Visual Studio.
-	Full control over features. Very easy to extend with extra features, and very easy to remove features you don’t like. 
-	Create, open and build any of your Visual Studio (MSBuild) projects, and view them in the solution explorer.
-	Multilingual user interface.
-	Dockable GUI. 

Next to that the standard LiteDevelop comes with couple of essential extensions:
-	Code editor
    -	Syntax highlighting for several languages.
    -	Auto completion and suggestions.
-	Forms designer (alpha)
    -	Place, reposition and resize controls using the “What-you-see-is-what-you-get” interface.
    -	Edit properties in the properties window.
    -	Serialize and deserialize forms from source code (alpha). 
-   Resource editor
    -   Create and edit *.resx and *.resources files.
-   Debugger
    -   Debug .NET projects.
    -   Set breakpoints.
    -   Step through code.
    -   View variable values.
    -   View call stack.
    -   View running threads.
    -	View current MSIL code.
    -	View x86 code (alpha).

How to clone
---

In order to successfully clone the project, make sure you also clone the submodules in the Libraries folder. If you're using Git 1.6.5 or later, you can run the following command:
```
git clone --recursive https://github.com/JerreS/LiteDevelop.git
```

Else use:

```
git clone https://github.com/JerreS/LiteDevelop.git
cd LiteDevelop
git submodule update --init --recursive
```


How to compile
---
Open the project in visual studio and click build, or use one of the batch files included in the root directory.

| Batch file        | Description
| ----------------- |:-------------
| build-debug.bat   | Builds the application using the debug configuration, including all xml, help and pdb files.
| build-minimal.bat | Builds the application using the minimal configuration, only including the most necessary files. Choose this one if you have very little storage to spend.
| build-release.bat | Builds the application using the release configuration, including all xml, and help files.    


Requirements
---
-    Framework 4.0 (extended, not the client profile)

Todo
---
-	Project dependencies.
-   Multiple building platforms and configurations for msbuild (Debug, Release, AnyCPU, x86, x64 ...)
-	Clipboard operations in forms designer.
-	Auto completion of the code editor needs work.
-   Forms designer serializer needs work.
-	Forms designer event bindings.
-   Appearance mapping needs work.


Donate
---

[![Donate!](https://www.paypalobjects.com/en_US/GB/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VS2P6V5X85QHA)