# Timetable Lambda
The new and improved backend for timetable talk.
runs expressjs with nodejs.

 **notes:** 
 - _requires serverless to be globally installed if you want to deploy, along with the /.aws/credettials file existing within your user directory_
 - _Might not have the bin/www file due to some gitnore issues, don't worry it's just the default express one_
 
### Features
- **api/test**
- **/events**
-   /addevent                   POST
-   /removeevent                POST
-   /editevent                  POST
-   /myevents/:userid           GET
-   /events?friends=id1,id2,id3 GET
- **/users**
-   /avcheck/:username          GET
-   /login                      POST
-   /signup                     POST
-   /changename                 POST
-   /changepassword             POST
-   /userid/:username           GET
- **/friends**
-   /friendrequest              POST
-   /updaterequest              POST
-   /removefriend               POST
-   /friends/:userid            GET


#### Objectives
- Provide a replacement for the currently not working C# backend due to the lack of mysQl.data support in xamarin :(
- Ensure that app has all stated functions, running free from error...



### To-do
- [x] Push code that actually compiles
- [x] Do midleware.
- [x] Everything.