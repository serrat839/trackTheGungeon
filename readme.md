# Track The Gungeon

## Purpose
The purpose of this mod is to give the game _**Enter The Gungeon**_ access to POST http requests.
By doing this, I will be able to send game metadata after each game session, tracking the status of a game, the items obtained, weapons obtained etc. After which, there will be a webapp developed for the app to post this data to, providing an interface for users to see trends in their gameplay to answer questions like:
* What weapons do I perform better with?
* Will obtaining certain weapons or items "doom" a run for me?
* Was this run extremely lucky or unlucky?

## Todo
As of right now, the todo list is as follows
1. Create a proof of concept mod/command: create an in-game command to "hello world" and then to send a simple GET request to a web server
  * This also will involve learning c#, but from what I can tell, it is java but made by microsoft
2. Hook game-over processes to a function that will bundle and POST data to a test web server
3. Create a webserver to handle and display all of this data. (This will be in a separate repo)
