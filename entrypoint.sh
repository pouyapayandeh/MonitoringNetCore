#!/bin/bash

# Start the first process
cd /mediamtx/ && ./mediamtx &

# Start the second process
cd /app &&  dotnet MonitoringNetCore.dll

# Wait for any process to exit
wait -n

# Exit with status of process that exited first
exit $?