# AOE2-No-Kiting
Common problem with the game AOE2 when the AI uses super micro control of ranged units. At first glance it looks cool and correct. But only until your entire squad runs after the archers, who will scatter in different directions infinitely far as the edge of the map allows.  
https://www.reddit.com/r/aoe2/comments/ojttq0/ai_kiting_is_ridiculous_and_makes_any_scenario/  
https://www.reddit.com/r/aoe2/comments/mx3w3d/has_the_kiting_by_ai_been_adjusted_yet/  
https://www.reddit.com/r/aoe2/comments/605254/ai_kiting_discusiion/  
https://steamcommunity.com/app/221380/discussions/0/154642321536364583/  
https://forums.ageofempires.com/t/please-please-please-give-us-an-option-to-disable-ai-kiting/85394  
etc.  

This tool need use with other tool for repack .cpn (company files) files [rge_campaign](https://github.com/withmorten/rge_campaign).  
The solution is to change the parameter "ability-to-maintain-distance" which depends on the complexity of the game and is contained in the company files (which is what this program is for) as well as in the file gamedata_x1.drs in scripts.  
To use need place this program with rge_campaign to CAMPAIGN folder and run. Make backup. File gamedata_x1.drs you can change self (need Turtle Pack, files 6002\6008\6029) or download and replace for classic Age of Empires II: The Conquerors.
