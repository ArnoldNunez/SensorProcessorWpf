@rem
@rem Script to compile the project proto files.
@rem

if not exist "generated" md "generated"

protoc -I=protos --csharp_out=generated protos/*.proto protos/DiagnosticsService/*.proto protos/SessionService/*.proto