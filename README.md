# BOG.CollectorEndPoint
REST Endpoint which just logs method, headers and body, and returns 200. For testing event and callback content.

### Arguments:
--EventLogFile %TEMP%\\BOG.CollectionEndPoit.Log.txt

Overrides the default location and name for a log file.  If not present, the entry is logged to
a  unique filename in the temp folder specified by the TEMP environment variable.

If the file exists, the output is appended.

### API interface:

http://{server}:{port}//swagger

