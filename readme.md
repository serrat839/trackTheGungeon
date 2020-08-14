# Track The Gungeon

## Purpose
The purpose of this mod is to give the game _**Enter The Gungeon**_ access to POST http requests.
By doing this, I will be able to send game metadata after each game session, tracking the status of a game, the items obtained, weapons obtained etc. After which, there will be a webapp developed for the app to post this data to, providing an interface for users to see trends in their gameplay to answer questions like:
* What weapons do I perform better with?
* Will obtaining certain weapons or items "doom" a run for me?
* Was this run extremely lucky or unlucky?

## Packaging the mod
<b>in a zip folder...</b>
1. insert the .dll file that is your mod
2. insert your metadata.txt file
3. insert MonoMod.RuntimeDetour and MonoMod.Utils .dll files from [monomod](https://github.com/MonoMod/MonoMod/releases) tools

Once done, just slap the zip folder in your etg mods folder, then delete RelinkCache/ and mods.txt and you are good to go!

## Todo
As of right now, the todo list is as follows
1. ~~Create a proof of concept mod/command: create an in-game command to "hello world" and then to send a simple GET request to a web server~~
  * ~~This also will involve learning c#, but from what I can tell, it is java but made by microsoft~~
  * WE MADE A GET REQUEST FROM THE GAME BABAY!
2. Hook game-over processes to a function that will bundle and POST data to a test web server
3. Create a webserver to handle and display all of this data. (This will be in a separate repo)
