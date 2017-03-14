# SourceRandomizer
With this tool you can easily randomize your source-code on every build!

There are 2 important steps that you have to do before using it.

### 1. Setup a build event
[How to run an exe on a build event](http://stackoverflow.com/a/7704362)

### 2. Provide required arguments
* -s "[path]" - Directory path where SourceRandomizer will scan for all source files
* -e "[ext]" - File extension of your source files *(cs, cpp etc.)*
* -c "[tag]" - Comment tag that you are using *(//)*
* -crlf / -lf - Text format that you are using *(optional)*

[Easy VS extension to force all code to one text format](http://www.grebulon.com/software/stripem.php)

#### Example of ready build event arguments:
"..\SourceRandomizer.exe" -s "..\MyProject" -e ".cs" -c "//"

## Usage
Currently there are availible 2 tags that you can use to randomize your source-code:

### 1. s like 'swap'
```
//s
int var1 = 0;
int var2 = 1;
string var3 = "";
string var4 = string.Empty;
//sc
```

will be randomized into:
```
//s
string var4 = string.Empty;
int var1 = 0;
string var3 = "";
int var2 = 1;
//sc
```

### 2. b like 'block'
```
switch (stringValue)
{
  //s
  //b
  case "1":
    break;
  //bc
  //b
  case "2":
    break;
  //bc
  //b
  case "3":
    break;
  //bc
  //sc
}
```

will be randomized into:
```
switch (stringValue)
{
  //s
  //b
  case "2":
    break;
  //bc
  //b
  case "3":
    break;
  //bc
  //b
  case "1":
    break;
  //bc
  //sc
}
```
