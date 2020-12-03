PS8 README
TankWars Client created by Ethan Duncan and Matt Mader
November of 2019

Implementation:
Uses recieved data from the server to draw the world and play
the tank game. Most data is based on the constants class which
holds lots of data about the state of the world and how it is 
doing. The Model contains all information about objects in the 
world such as Tanks, Projectiles, ect. The model is controlled
by the controller, which houses all the logic of the program.
The controller is invoked mostly by the view, which is in 
control of drawing and updating the actual look of the game.
In essence, we followed MVC protocol and ensured that separation
of concerns was followed.

Interesting features:
Death animation is an epic explosion created from seven frames
All tanks are different colors, all shots are different colors
Health Bar changes color when hits are taken

Potential Problems: 
Connect button requires a restart to reenable, this is due to us
not knowing how to trigger an event that would cause it to re
enable, despite hours of trying.

Note that on our machine there was a slight stutter to the program
occasionally, but when running on another machine it ran smoothly.
We are not sure what caused this but figured it was worth mentioning.

---------------------------------------------------------------------

PS9 README
TankWars Server created by Ethan Duncan and Matt Mader
November/December of 2019

Implementation: 
Our server constantly sends new data to all connected clients through
methods from PS7. The update loop works through the recommended method,
a busy loop that constantly invokes an update method. As for keeping 
track of data, it is based entirely off of dictionaries of objects
created at the beginning of the program. It is not based off of a 
World object as we have heard others are. Instead it relies on adding
and removing objects from these dictionaries as they are created and 
destroyed. At the beginning of the startup, the main method starts a
server using the StartServer method from PS7. Next, the main method
starts a separate thread that begins to run the update method, which
updates every objects location and then sends that data to the client.
This method is invoked through the update loop that is mentioned above. 
After the separate thread is started, a secondary server is started 
that handles HTTP connections and requests. This secondary server 
does not do anything until after a button press is recieved or until
a request is made to report game/player data. All data for the main
game is handled through a settings.xml file. We did tinker around 
with these data points somewhat, but eventually decided that we liked
the base data the most, as it seems to allow the game to flow the best
and did not cause any major bugs. All disconnections and interuptions
are handled gracefully by the server, and we did not find any major
bugs or crashes that needed to be reported. 

Concerns:
Our biggest concern was through how we accessed data of objects. We know
that most getters and setters for object properties should be privatized, but 
could not think of how to retrieve the data should this be the case. We could 
perhaps have created separate get and set methods, but that seemed somewhat
redundant and we weren't sure what other options we had. Therefore we 
simply made data that we needed to access public and moved on in order to 
complete the project on time. However, we do wish we could have thought of 
a better way to implement this had we had more time.

Things of note:
Every requirement was followed and no major bugs have appeared.
The database reporting works perfectly and was implemented exactly as requested.
All data is changable and will still be playable by the client.
The server performs well even with 8 people connected.
The server has very low latency. 