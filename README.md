# Dig Fight (3D, Mobile)
<p align="center"><img src="https://i.imgur.com/2khGOgZ.png" width="150" title="icon"></p>

## Table of Content
- [Description](#description)
- [Mechanics](#mechanics)
- [Visuals](#visuals)

## Description
It's a Hypercasual game made with Unity.
Game is level based. Every level there is an Ai opponent. Player and Ai cannot cross to the other side. There are stones, TNTs, pushable boxes on each side. Player has to break the stones to win but pickaxe has HP. So it needs to be upgraded to play further.

There are also chests. They contain temporary buffs and some gold coin. Also pushable boxes can deny the opponent to proceed the level.

The first one to get to the bottom wins.

Ad system is implemented into the project. Project has no integrated 3rd party ad but it's events are ready for integration.

## Mechanics
- **Flying**\
  _Player flies according to the input. With horizontal input, player walks if grounded. With no input, player starts falling._
- **Breaking Stones**\
  _There are 3 types of stones. Default, copper and diamond. Copper and diamond stones give extra money if Player hits them. All stones have HP and pickaxe loses HP when it hits._
- **Pushable Box**\
  _There are boxes with one space on each side. These boxes can be pushed to delay or deny the opponent to dig further._
- **Ai**\
  _There is an Ai which can decide to walk, fly, push the box, break the stones or open the chests._
- **Chests**\
  _There are chests in some levels. Player can open it by hitting it with the pickaxe. Chests contain temporary buffs like speed, dig power or pickaxe health. Also chests give a gold coin._
- **Incremental Upgrade**\
  _There are 3 incremental style upgrades which are movement speed, dig speed, money value. They are related to player not the pickaxe. Player can upgrade at the start of each level._
- **TNT**\
  _Player can hit TNT to explode it. TNT breaks surrounding boxes except pushable box. Also TNT does a lot of damage to the pickaxe. Sometimes not touching the TNT is better._
- **Upgrade Screen**\
  _In this screen, there are pickaxe upgrades. Player can have normal, silver or gold pickaxes. Default pickaxe upgrades cost money. Silver and gold pickaxe upgrades cost gold coin. Player has to buy gold coin from Shop._
- **Shop Screen**\
  _Player can buy gold coins and new pickaxes from this menu. At some point Player has to buy silver and gold pickaxe or upgrade the current pickaxe stats to be able to beat the level._
- **Ads**\
  _At random point, pickaxe reward ui will pop up for a limited amount of time. If Player clicks the popup and watches the ad, gold pickaxe is rewarded for a limited time._
  _There is Interstitial ads. It triggers if timer is up and Player changes the screen._
  _There is also Rewarded ads. One of the ad triggers when player dies, watching rewarded makes the game continue._
  _Other rewarded is triggered when player purchases incremental upgrades. On some levels, game will ask Player to watch an ad to upgrade._
- **Level End Indicator**\
  _There is an indicator on the side. It shows how many blocks there are between character and finish line._

## Visuals
Here are some visuals of the game;

Flying                     | Ai 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExYjhrdXNrc3Z0d2RodHlmMGc0ODdka3J2ZHVqOG5peGt2OHNrdmo1ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/lM1x8iJzOOCaBdGDVT/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExOXVueXU3a3IyYWdxaGlzNHB1cGYzZ3VtdzI3cmh4ZnN3cndsMnZuZCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/H8xUMfsDP6Lho2sWA5/giphy.gif)

Breaking Stones            | TNT 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExendtbjR6a3Mxcmo2czkyd3FkNmpuZWMyM2l5dXp0eTloa3d4NXF6ciZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/GR8S0SiktQpkwzzdF3/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExZTRjanplcTdzdHF4eWFicGNlb3Vmajg2dWx0OW5yOXJ0Y3Uyejl2dyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/uaE1tHGavDSSydN3XB/giphy.gif)

Pushable Box               | Chest 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExZ3pubjJuNXkwenp0eWI2MmJmbnA4MXRhbm16bDh1NnNxeWhleW9wNyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/T5FjrOs4HTuX9A5DoM/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExdHNnMGlmYWQ1M2lkNGZ1M3E1OXIzYzV5dWVqOThvd296ZDM1YzY1ZyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/suwkkZNMeo4W5dNdlo/giphy.gif)

Level End Indicator        | Incremental Upgrade 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExaW1iOHBlNHM0eXB6YTdkd3ZsbzNxMWR1azFiZXNuaHluaHYyM2l6eCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3DtPWP8DA7iieBE2Bx/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExZHQxaWk5NTIxbGZiOXAwamFqNDV6MDY3aDN0bWVrbTQ3bDB1bXcxNyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/AmW7YT8sFJB5KewFAS/giphy.gif)

Shop Screen                | Upgrade Screen 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExOGRwYWhlNWgzY2l2OHdvZDBrc2N2eDR4NGo4cHYwd21iY2wxbHl6NiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/W6I48xMXl9WocIACq9/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExa24weDgyZnAwNmdhNjA1MmJlbXU1MG5nb2Q5aXhmMWRsbGZlNmFpayZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/kzAxpv8rTEoS9bBnIz/giphy.gif)

Revive                     | Try Pickaxe 
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExOWR1ano2YTVtN2JmdG16ancwbXF1YWRqaGluY2NlNmF4ZGJrd3o2NCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/0pDSqs1KhXkO3Eniel/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExYTM4Mm1rbGZsOHkzbmlvODNkemh1ZnF5cjN5ano1OHIzNnM0ZW5oeSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/OpBRVNnHQGLNCTyv3Z/giphy.gif)
