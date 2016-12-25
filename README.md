# SourceRandomizer
With this tool you can easily randomize your source code every build!

There are 2 important steps that you have to do before using it.
### 1. Setup a build event
[How to run an exe on a build event](http://stackoverflow.com/a/7704362)
#### 2. Arguments
Also you have to provide some arguments to make this tool work:
* -s "[path]" - Full directory path where SourceRandomizer will scan for all source files
* -e "[ext]" - File extension of your source files (cs, cpp etc.)
* -c "[tag]" - Comment tag that you are using (//)
* -crlf|-lf - Text format that you are using **IMPORTANT**

[Easy VS extension to force all code to one text format](http://www.grebulon.com/software/stripem.php)

#### Example of ready build event:
"{path}\SourceRandomizer.exe" -s "{path}\MyProject" -e ".cs" -c "//" -crlf
