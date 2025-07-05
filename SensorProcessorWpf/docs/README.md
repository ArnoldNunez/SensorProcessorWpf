# Developer Guide

## Install protoc

<p>
There are multiple ways of getting protoc such as building from source or 
using prebuilt artifacts. Here we will focus on the simple approach of using prebuilts.
Navigate to teh protocol buffers releases on github: https://github.com/protocolbuffers/protobuf/releases
The current target version for this project is protoc-30.20-win64. Download the 
zip and move the contents to any directory on the system. Add the /bin directory
to your windows systems PATH.
</p>

```
# Open a terminal and verify protoc is found:
> protoc --version
```

## Compiling Protos

<p>
With protoc installed, you can now compile the project proto files. The compile_protos.bat file
is provided for easy compilation, run this script to generate the CSharp classes._
</p>

```
> compile_protos.bat
```

<p>
This will place the generated classes in the root of the project under the "generated" folder.
</p>