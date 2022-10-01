# Lobster Adventure

This is an adventure api providing a rest
API for creating Adventure , make user able to go through it and check his final result.

The entire application can be containerized by the exisiting `Dockerfile` file in the Api Folder.

you can run the Api by building docker image then run it , or by opening solution in visualstudio

`Api Contains three categories of endpoints:

## Token endpoint 

    -Just to simulate security in the api.
    -And you have to use it at first step to generate token ,
     then add that token to the request header [postman] or at swagger Authorize button. 
    
## Internal endpoints 

   -Which in normal case should not exposed as public endpoint as these endpoints developed for creating the adventure.
   -lease note, Create Adventure endpoint expected Array of string , which the adventure tree will be created based on it, so tree nodes text will get their values         from this string array parameter.

## User endpoints

    -These end points are the actual function requirements, Which give the user ability to
        -Start new adventure.
        -Go to next step.
        -And review his selections whenfinish.

