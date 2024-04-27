
<img src="https://img.shields.io/badge/test-22_passing-blue"/> <img src="https://img.shields.io/badge/http_postman-test pass-green"/> <img src="https://img.shields.io/badge/build-passing-brightgreen"/>
# AGServer
- Multiple server oriented to games, handle Http Server + AG Server (Custom for games)
- (I'm still working on it, I don't have much time, it's currently only the web server deployed)
```C++ 
Project Require (NET6);
char[] Language = "C#";
```
___________

### How Test:
You can handle private console of AG Server.
```C#
  >> init [ags|http] [port] [d]
        //ags|http    initialize a ags server or http server
        //port        use a custom port (default is 5008 in ags and 8080 in http)
        //d           debug mode
        //example:    >>init ags 5008 d
  
  >> use [index] [m|-m]
        //index       index of server instance (default is 0)
        //m           set maintenance mode
        //-m          erase maintenance mode
        //example:    >>use 1 m
        
  >> exit
       //         exit the program.
```
